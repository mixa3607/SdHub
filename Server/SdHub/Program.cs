using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql;
using SdHub.Database;
using SdHub.Database.Shared;
using SdHub.Extensions;
using SdHub.Logging;
using SdHub.Options;
using SdHub.Services;
using SdHub.Services.Captcha;
using SdHub.Services.ErrorHandling.Extensions;
using SdHub.Services.FileProc;
using SdHub.Services.FileProc.Detectors;
using SdHub.Services.FileProc.Extractor;
using SdHub.Services.FileProc.Metadata;
using SdHub.Services.Mailing;
using SdHub.Services.Storage;
using SdHub.Services.User;
using SdHub.Services.ValidatorsCheck;

//не круто но как есть
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
NpgsqlConnection.GlobalTypeMapper.UseJsonNet(settings: new JsonSerializerSettings()
{
    TypeNameHandling = TypeNameHandling.None,
    Converters = new List<JsonConverter>()
    {
        new ImageMetadataTagValueConverter()
    }
});


//#########################################################################


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//logging
var serilogOptions = builder.Configuration.GetOptionsReflex<SerilogOptions>();
builder.Host
    .AddCustomSerilog(serilogOptions)
    ;
//misc
builder.Services
    .AddAndGetOptionsReflex<AppInfoOptions>(builder.Configuration, out var appInfo)
    ;
//users
builder.Services
    .AddScoped<IUserFromTokenService, UserFromTokenService>()
    ;
//swagger
builder.Services
    .AddAndGetOptionsReflex<SwaggerOptions>(builder.Configuration, out var swaggerOptions)
    .AddCustomSwagger(swaggerOptions)
    ;
//security
builder.Services
    .AddAndGetOptionsReflex<RecaptchaOptions>(builder.Configuration, out _)
    .AddSingleton<ICaptchaValidator, CaptchaValidator>()
    .AddAndGetOptionsReflex<WebSecurityOptions>(builder.Configuration, out var securityOptions)
    //.AddCustomCors(securityOptions)
    .AddCustomSecurity(securityOptions)
    ;
//database
builder.Services
    .AddAndGetOptionsReflex<DatabaseOptions>(builder.Configuration, out var dbOptions)
    .AddScoped(typeof(IDbMigrator<>), typeof(DbMigrator<>))
    .AddAndGetOptionsReflex<SdHubSeederOptions>(builder.Configuration, out _)
    .AddSingleton<IDbSeeder<SdHubDbContext>, SdHubDbSeeder>()
    .AddDbContext<SdHubDbContext>(x =>
        x.UseNpgsql(dbOptions.ConnectionString, y => y.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)))
    ;
//error handling
builder.Services
    .AddCustomErrorHandling(typeof(Program).Assembly)
    ;
//controllers
builder.Services
    .AddControllers()
    .AddNewtonsoftJson(o =>
    {
        /*o.SerializerSettings.Converters.Add(new StringEnumConverter());*/
    })
    ;
//validation
builder.Services
    .AddFluentValidationAutoValidation(o =>
    {
        o.ImplicitlyValidateChildProperties = true;
        o.ImplicitlyValidateRootCollectionElements = true;
    })
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssembly(typeof(Program).Assembly)
    .AddScoped<AllRequiredValidatorsRegisteredService>()
    ;
//automapper
builder.Services
    .AddAutoMapper(typeof(Program).Assembly)
    ;
//mem cache
builder.Services
    .AddMemoryCache()
    ;
//spa
builder.Services
    .AddSpaStaticFiles(configuration => { configuration.RootPath = "./spa_dist"; })
    ;
//mailing
var mailingOptions = builder.Configuration.GetOptionsReflex<MailingOptions>();
var smtpClient = new SmtpClient(mailingOptions.Host, mailingOptions.Port);
smtpClient.DeliveryMethod = mailingOptions.UseMaildir
    ? SmtpDeliveryMethod.SpecifiedPickupDirectory
    : SmtpDeliveryMethod.Network;
smtpClient.EnableSsl = mailingOptions.EnableSsl;
smtpClient.Credentials = new NetworkCredential(mailingOptions.Username, mailingOptions.Password);
smtpClient.PickupDirectoryLocation = mailingOptions.PathToMaildir;
builder.Services
    .AddSingleton<IMailingService, MailingService>()
    .AddSingleton<IEmailCheckerService, EmailCheckerService>()
    .AddFluentEmail(mailingOptions.From).AddLiquidRenderer(c =>
    {
        c.FileProvider = new PhysicalFileProvider(mailingOptions.TemplatesDir);
    })
    .AddSmtpSender(smtpClient)
    ;
//storage
builder.Services
    .AddAndGetOptionsReflex<FileProcessorOptions>(builder.Configuration, out _)
    .Scan(x => x
        .FromAssemblyOf<IMetadataSoftwareDetector>()
        .AddClasses(c => c.AssignableTo<IMetadataSoftwareDetector>())
        .AsImplementedInterfaces()
        .WithSingletonLifetime())
    .AddSingleton<IImageMetadataExtractor, ImageMetadataExtractor>()
    .AddScoped<IFileProcessor, FileProcessor>()
    .AddSingleton<IFileStorageFactory, FileStorageFactory>();


var app = builder.Build();

app.UseCustomSerilogging(serilogOptions);

if (securityOptions.EnableForwardedHeaders)
{
    //forwarded headers
    var forwardOptions = new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                           ForwardedHeaders.XForwardedProto |
                           ForwardedHeaders.XForwardedHost,
        RequireHeaderSymmetry = false
    };
    forwardOptions.KnownNetworks.Clear();
    forwardOptions.KnownProxies.Clear();
    app.UseForwardedHeaders(forwardOptions);
}

if (securityOptions.EnableHttpsRedirections)
{
    app.UseHttpsRedirection();
}

app.UseCustomErrorHandling();
app.UseCustomSwagger(swaggerOptions);

app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true,
    DefaultContentType = MediaTypeNames.Application.Octet
});
app.UseSpaStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints => { endpoints.MapControllers().RequireAuthorization(); });
app.UseMiddleware<OgInjectorMiddleware>();
app.UseSpa(c =>
{
    c.Options.SourcePath = "./spa_dist";
    if (!string.IsNullOrWhiteSpace(appInfo.FrontDevServer))
    {
        Console.WriteLine("Use proxy to frontend!!!");
        c.UseProxyToSpaDevelopmentServer("http://localhost:4200/");
    }
});


//#########################################################################


CheckValidators(app.Services);
ValidateAutomapper(app.Services);
await MigrateDbAsync(app.Services);

await app.RunAsync();


//#########################################################################


static void CheckValidators(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var service = scope.ServiceProvider.GetRequiredService<AllRequiredValidatorsRegisteredService>();
    service.Check();
}

static async Task MigrateDbAsync(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var migrator = scope.ServiceProvider.GetRequiredService<IDbMigrator<SdHubDbContext>>();
    await migrator.Migrate();
    await migrator.Seed();
}

static void ValidateAutomapper(IServiceProvider serviceProvider)
{
    var scope = serviceProvider.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var environment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();

    if (!environment.IsDevelopment())
    {
        logger.LogWarning("Skip Automapper check in non Development env");
        return;
    }

    try
    {
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
        mapper.ConfigurationProvider.AssertConfigurationIsValid();
        logger.LogInformation("Automapper profiles is valid");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error during automapper validation");
        throw;
    }
}