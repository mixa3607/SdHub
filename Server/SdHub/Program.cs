using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Hangfire.PostgreSql;
using Hangfire.Redis.StackExchange;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Npgsql;
using SdHub.Attributes;
using SdHub.Constants;
using SdHub.Database;
using SdHub.Extensions;
using SdHub.Hangfire.BasicAuth;
using SdHub.Hangfire.Jobs;
using SdHub.Options;
using SdHub.RequestFeatures;
using SdHub.Services;
using SdHub.Services.Captcha;
using SdHub.Services.FileProc;
using SdHub.Services.FileProc.Detectors;
using SdHub.Services.FileProc.Extractor;
using SdHub.Services.FileProc.Metadata;
using SdHub.Services.Mailing;
using SdHub.Services.Storage;
using SdHub.Services.TempCodes;
using SdHub.Services.User;
using SdHub.Shared.AspAutomapper;
using SdHub.Shared.AspErrorHandling;
using SdHub.Shared.AspErrorHandling.Exceptions;
using SdHub.Shared.AspValidation;
using SdHub.Shared.EntityFramework;
using SdHub.Shared.Logging;
using StackExchange.Redis;
using static MetadataExtractor.Formats.Exif.Makernotes.CanonMakernoteDirectory;


var builder = WebApplication.CreateBuilder(args);

Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"Application: {builder.Environment.ApplicationName}");
Console.WriteLine($"ContentRoot: {builder.Environment.ContentRootPath}");


//logging
builder.Services.ConfigureRbSerilog(builder.Configuration.GetSection("Serilog"));
builder.Host.AddRbSerilog();

//database
{
    var dbOptions = builder.Configuration
        .GetSection("Database")
        .Get<PgDatabaseOptions>()!;
    var dataSourceBuilder = new NpgsqlDataSourceBuilder(dbOptions.ConnectionString);
    dataSourceBuilder.UseJsonNet(settings: new JsonSerializerSettings()
    {
        TypeNameHandling = TypeNameHandling.None,
        Converters = new List<JsonConverter>()
        {
            new ImageMetadataTagValueConverter()
        }
    });
    var dataSource = dataSourceBuilder.Build();
    builder.Services
        .Configure<SdHubSeederOptions>(builder.Configuration.GetSection("SdHubSeeder"))
        .AddScoped<IDbMigrator<SdHubDbContext>, DbMigrator<SdHubDbContext>>()
        .AddSingleton<IDbSeeder<SdHubDbContext>, SdHubDbSeeder>()
        .AddDbContext<SdHubDbContext>(x =>
            x.UseNpgsql(dataSource, y => y
                .MigrationsHistoryTable(SdHubDbContext.HistoryTable, SdHubDbContext.SchemaName)
                .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
        );
}

//security
var securityOptions = builder.Configuration.GetSection("WebSecurity").Get<WebSecurityOptions>()!;
builder.Services
    .Configure<RecaptchaOptions>(builder.Configuration.GetSection("Recaptcha"))
    .Configure<WebSecurityOptions>(builder.Configuration.GetSection("WebSecurity"))
    .AddSingleton<ICaptchaValidator, CaptchaValidator>()
    .AddCustomCors(securityOptions)
    .AddCustomSecurity(securityOptions)
    ;

//controllers
builder.Services
    .Configure<ApiBehaviorOptions>(opts => { opts.SuppressModelStateInvalidFilter = true; })
    .AddRbErrorHandlersBuiltin()
    .AddControllers(c => c.Filters.Add<BadModelStateActionFilter>())
    .AddNewtonsoftJson()
    ;

//validation
builder.Services
    .AddFluentValidationAutoValidation()
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

//swagger
var swaggerOptions = builder.Configuration.GetSection("Swagger").Get<SwaggerOptions>();
builder.Services
    .Configure<SwaggerOptions>(builder.Configuration.GetSection("Swagger"))
    .AddSwaggerGenNewtonsoftSupport()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(o =>
    {
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    })
    ;

//app info
var appInfo = builder.Configuration.GetSection("AppInfo").Get<AppInfoOptions>();
builder.Services
    .Configure<AppInfoOptions>(builder.Configuration.GetSection("AppInfo"))
    ;

//hangfire
var hangfireOptions = builder.Configuration.GetSection("Hangfire").Get<HangfireOptions>() ?? new HangfireOptions();
builder.Services
    //.AddScoped<IImageConvertRunnerV1, ImageConvertRunner>()
    //.AddScoped<IHangfireUsersService, HangfireUsersService>()
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
                x.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(hangfireOptions.PgConnectionString),
                    new PostgreSqlStorageOptions()
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

//mailing
var mailingOptions = builder.Configuration.GetSection("Mailing").Get<MailingOptions>() ?? new MailingOptions();
builder.Services
    .Configure<MailingOptions>(builder.Configuration.GetSection("Mailing"))
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
        smtpClient.EnableSsl = mailingOptions is { UseMaildir: false, EnableSsl: true };
        smtpClient.UseDefaultCredentials = false;
        smtpClient.Credentials = new NetworkCredential(mailingOptions.Username, mailingOptions.Password);
        smtpClient.PickupDirectoryLocation = Path.GetFullPath(mailingOptions.PathToMaildir);
        return smtpClient;
    })
    ;

//storage
builder.Services
    .Configure<FileProcessorOptions>(builder.Configuration.GetSection("FileProcessor"))
    .Scan(x => x
        .FromAssemblyOf<IMetadataSoftwareDetector>()
        .AddClasses(c => c.AssignableTo<IMetadataSoftwareDetector>())
        .AsImplementedInterfaces()
        .WithSingletonLifetime())
    .AddSingleton<IImageMetadataExtractor, ImageMetadataExtractor>()
    .AddScoped<IFileProcessor, FileProcessor>()
    .AddSingleton<IFileStorageFactory, FileStorageFactory>()
    ;

//spa
builder.Services
    .AddSpaStaticFiles(configuration => { configuration.RootPath = "./spa_dist"; })
    ;

//users
builder.Services
    .AddScoped<ITempCodesService, TempCodesService>()
    .AddScoped<IUserFromTokenService, UserFromTokenService>()
    ;


//#########################################################################


var app = builder.Build();

//forwarded headers
if (securityOptions.EnableForwardedHeaders)
{
    var forwardOptions = new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
        ForwardLimit = 10,
        RequireHeaderSymmetry = false
    };
    forwardOptions.KnownNetworks.Clear();
    forwardOptions.KnownProxies.Clear();
    app.UseForwardedHeaders(forwardOptions);
}

//error handling
app.UseRbErrorsHandling();

//logging
app.UseRbSerilogRequestLogging();

// https redirection
if (securityOptions.EnableHttpsRedirections)
{
    app.UseHttpsRedirection();
}

//swagger
if (swaggerOptions?.Enable == true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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

app.MapControllers().RequireAuthorization();

app.UseMiddleware<OgInjectorMiddleware>();
app.MapWhen(x => !x.Request.Path.Value!.StartsWith("/api"), builder =>
{
    builder.UseSpa(c =>
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
});

//app.Use((HttpContext ctx, RequestDelegate next) =>
//{
//    ctx.Response.StatusCode = StatusCodes.Status418ImATeapot;
//    return Task.CompletedTask;
//});


//#########################################################################

app.Services
    .RbCheckActionsValidators()
    .RbCheckTz()
    .RbCheckLocale()
    .RbValidateAutomapper()
    ;

await app.Services.RbEfMigrateAsync<SdHubDbContext>();
PrepareHangfire(app.Services);

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