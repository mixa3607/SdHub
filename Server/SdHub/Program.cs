using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Hangfire.PostgreSql;
using Hangfire.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Npgsql;
using SdHub.Attributes;
using SdHub.Constants;
using SdHub.Database;
using SdHub.Database.Shared;
using SdHub.Extensions;
using SdHub.Hangfire.BasicAuth;
using SdHub.Hangfire.Jobs;
using SdHub.Logging;
using SdHub.Options;
using SdHub.RequestFeatures;
using SdHub.Services;
using SdHub.Services.Captcha;
using SdHub.Services.ErrorHandling.Extensions;
using SdHub.Services.ErrorHandling.Handlers;
using SdHub.Services.FileProc;
using SdHub.Services.FileProc.Detectors;
using SdHub.Services.FileProc.Extractor;
using SdHub.Services.FileProc.Metadata;
using SdHub.Services.Mailing;
using SdHub.Services.Storage;
using SdHub.Services.Tokens;
using SdHub.Services.User;
using SdHub.Services.ValidatorsCheck;
using StackExchange.Redis;

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
    .Configure<ApiBehaviorOptions>(opts => { opts.SuppressModelStateInvalidFilter = true; })
    ;
//hangfire
builder.Services
    .AddScoped<IImageConvertRunnerV1, ImageConvertRunner>()
    .AddAndGetOptionsReflex<HangfireOptions>(builder.Configuration, out var hangfireOptions)
    .AddScoped<IHangfireUsersService, HangfireUsersService>()
    .AddHangfire(x =>
    {
        switch (hangfireOptions.StorageType)
        {
            case HangfireStorageType.InMemory:
                Console.WriteLine("HGF: Use memory storage");
                x.UseMemoryStorage();
                break;
            case HangfireStorageType.Postgres:
                Console.WriteLine("HGF: Use postgres storage");
                x.UsePostgreSqlStorage(hangfireOptions.PgConnectionString, new PostgreSqlStorageOptions()
                {
                    SchemaName = hangfireOptions.PgSchema,
                    PrepareSchemaIfNecessary = true,
                });
                break;
            case HangfireStorageType.Redis:
                Console.WriteLine("HGF: Use redis storage");
                x.UseRedisStorage(ConnectionMultiplexer.Connect(hangfireOptions.RedisConnectionString!),
                    new RedisStorageOptions()
                    {
                        Prefix = hangfireOptions.RedisPrefix,
                        SucceededListSize = 10_000,
                    });
                break;
        }
        x.UseConsole();
    })
    .Scan(x => x
        .FromAssemblyOf<IHangfireBackgroundJobRunner>()
        .AddClasses(c => c.AssignableTo<IHangfireBackgroundJobRunner>())
        .AsImplementedInterfaces()
        .WithScopedLifetime())
    .Scan(x => x
        .FromAssemblyOf<IHangfireRecurrentJob>()
        .AddClasses(c => c.AssignableTo<IHangfireRecurrentJob>())
        .AsImplementedInterfaces()
        .WithScopedLifetime())
    ;
;
if (hangfireOptions.RunServer)
{
    builder.Services.AddHangfireServer(x =>
    {
        //x.ServerTimeout = TimeSpan.FromSeconds(5);
        //x.ServerCheckInterval = TimeSpan.FromSeconds(10);
        //x.CancellationCheckInterval = TimeSpan.FromSeconds(10);
        //x.HeartbeatInterval = TimeSpan.FromSeconds(10);
        x.ServerName = hangfireOptions.ServerName;
        x.Queues = new[] { "default", hangfireOptions.ServerQueue };
    });
}

;
//users
builder.Services
    .AddScoped<ITempCodesService, TempCodesService>()
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
    .AddCustomCors(securityOptions)
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
    .Scan(x => x
        .FromAssemblyOf<IServerExceptionHandler>()
        .AddClasses(c => c.AssignableTo<IServerExceptionHandler>())
        .As<IServerExceptionHandler>()
        .WithSingletonLifetime())
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
builder.Services
    .AddAndGetOptionsReflex<MailingOptions>(builder.Configuration, out var mailingOptions)
    .AddScoped<IMailingService, MailingService>()
    .AddSingleton<IEmailCheckerService, EmailCheckerService>()
    .AddFluentEmail(mailingOptions.From).AddLiquidRenderer(c =>
    {
        c.FileProvider = new PhysicalFileProvider(Path.GetFullPath(mailingOptions.TemplatesDir!));
    })
    .AddSmtpSender(() =>
    {
        var smtpClient = new SmtpClient(mailingOptions.Host, mailingOptions.Port);
        smtpClient.DeliveryMethod = mailingOptions.UseMaildir
            ? SmtpDeliveryMethod.SpecifiedPickupDirectory
            : SmtpDeliveryMethod.Network;
        smtpClient.EnableSsl = !mailingOptions.UseMaildir && mailingOptions.EnableSsl;
        smtpClient.UseDefaultCredentials = false;
        smtpClient.Credentials = new NetworkCredential(mailingOptions.Username, mailingOptions.Password);
        smtpClient.PickupDirectoryLocation = Path.GetFullPath(mailingOptions.PathToMaildir);
        return smtpClient;
    })
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
                           ForwardedHeaders.XForwardedProto,
        ForwardLimit = 5,
        RequireHeaderSymmetry = false,
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
app.Use((HttpContext ctx, RequestDelegate next) =>
{
    var jwtFailedFeature = ctx.Features.Get<JwtAuthFailedFeature>();
    if (jwtFailedFeature == null)
        return next(ctx);

    var endpoint = ctx.GetEndpoint();
    var allowExpiredJwt = endpoint?.Metadata.GetMetadata<AllowExpiredJwtAttribute>() != null;
    if (jwtFailedFeature.Exception is SecurityTokenExpiredException && !allowExpiredJwt)
    {
        ctx.Response.Headers.Add("WWW-Authenticate", JwtBearerDefaults.AuthenticationScheme);
        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    }

    return next(ctx);
});

//hangfire
app.UseHangfireDashboard("/hgf", new DashboardOptions
{
    IsReadOnlyFunc = x => x.GetHttpContext().User.IsInRole(UserRoleTypes.HangfireRO),
    AsyncAuthorization = new[]
    {
        new HangfireBasicAuthenticationFilter(false),
    }
});

app.UseEndpoints(endpoints => { endpoints.MapControllers().RequireAuthorization(); });
app.UseMiddleware<OgInjectorMiddleware>();
app.UseSpa(c =>
{
    c.Options.SourcePath = "./spa_dist";
    c.Options.DefaultPageStaticFileOptions = new StaticFileOptions()
    {
        OnPrepareResponse = ctx =>
        {
            if (ctx.File.Name != "index.html")
                return;
            var headers = ctx.Context.Response.GetTypedHeaders();
            headers.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true,
                NoStore = true,
                MustRevalidate = true,
                MaxAge = TimeSpan.Zero
            };
        }
    };
    if (!string.IsNullOrWhiteSpace(appInfo.FrontDevServer))
    {
        Console.WriteLine("Use proxy to frontend!!!");
        c.UseProxyToSpaDevelopmentServer("http://localhost:4200/");
    }
});

app.Use((HttpContext ctx, RequestDelegate next) =>
{
    ctx.Response.StatusCode = StatusCodes.Status418ImATeapot;
    return Task.CompletedTask;
});


//#########################################################################

CheckValidators(app.Services);
ValidateAutomapper(app.Services);
PrepareHangfire(app.Services);
await MigrateDbAsync(app.Services);

await app.RunAsync();


//#########################################################################


static void PrepareHangfire(IServiceProvider serviceProvider)
{
    JobStorage.Current.GetConnection().RemoveTimedOutServers(new TimeSpan(0, 0, 30));
    using var scope = serviceProvider.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var recurrentJobs = scope.ServiceProvider.GetRequiredService<IEnumerable<IHangfireRecurrentJob>>().ToArray();
    logger.LogInformation("Found {count} recurrent jobs", recurrentJobs.Length);
    foreach (var recurrentJob in recurrentJobs)
    {
        try
        {
            logger.LogInformation("Update job {name}", recurrentJob.Name);
            recurrentJob.UpdateJob();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Cant update job {name}", recurrentJob.Name);
        }
    }
}

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