using System.Buffers;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Exceptions;
using Serilog.Settings.Configuration;
using Serilog.Sinks.Elasticsearch;

namespace SdHub.Shared.Logging;

public static class CustomSerilogLoggingExtensions
{
    /// <summary>
    /// Configure serilog
    /// </summary>
    public static IServiceCollection ConfigureRbSerilog(this IServiceCollection services, IConfiguration cfg)
    {
        return services.Configure<SerilogOptions>(cfg);
    }

    /// <summary>
    /// Add serilog
    /// </summary>
    public static IHostBuilder AddRbSerilog(this IHostBuilder host,
        Action<HostBuilderContext, IServiceProvider, LoggerConfiguration>? applyAfter = null)
    {
        host.UseSerilog((ctx, services, l) =>
        {
            l
                .Enrich.WithEnvironmentName()
                .Enrich.WithEnvironmentUserName()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProcessName()
                .Enrich.WithProcessId()
                .Enrich.WithExceptionDetails()
                .Enrich.WithAssemblyName()
                .Enrich.WithAssemblyVersion()
                .Enrich.WithMemoryUsage();

            l.WriteTo.Console();

            var configurationAssemblies = new Assembly[]
            {
                typeof(LoggerConfigurationElasticsearchExtensions).Assembly,
                typeof(ConsoleLoggerConfigurationExtensions).Assembly,
                typeof(FileLoggerConfigurationExtensions).Assembly,
            };
            var options = new ConfigurationReaderOptions(configurationAssemblies);
            l.ReadFrom.Configuration(ctx.Configuration, options);

            applyAfter?.Invoke(ctx, services, l);
        });
        return host;
    }

    /// <summary>
    /// Если включено в конфигурации то логирует запрос. Горячая перезагрузка конфига не применяется
    /// </summary>
    public static WebApplication UseRbSerilogRequestLogging(this WebApplication app)
    {
        var serilogOptions = app.Services.GetRequiredService<IOptions<SerilogOptions>>().Value;
        if (serilogOptions.EnableRequestLogging)
        {
            const int inMemBuffLen = 50 * 1024;
            const int maxLoggingBodyLen = 100 * 1024;

            Console.WriteLine($"Request logging enabled with level {serilogOptions.RequestLogMessageLevel}");

            if (serilogOptions.EnableRequestBodyLogging)
            {
                Console.WriteLine($"Request body logging enabled");
                app.Use((context, next) =>
                {
                    context.Request.EnableBuffering(inMemBuffLen);
                    return next();
                });
            }


            app.UseSerilogRequestLogging(o =>
            {
                o.GetLevel = (httpContext, elapsed, ex) => serilogOptions.RequestLogMessageLevel;
                o.EnrichDiagnosticContext = (diagnosticContext, context) =>
                {
                    var bodyStr = (string?)null;
                    if (context.Request.ContentType == MediaTypeNames.Application.Json &&
                        serilogOptions.EnableRequestBodyLogging)
                    {
                        try
                        {
                            if (context.Request.ContentLength > maxLoggingBodyLen)
                            {
                                bodyStr = $"Body len more than {maxLoggingBodyLen}. Skip";
                            }
                            else
                            {
                                var blob = MemoryPool<byte>.Shared.Rent(maxLoggingBodyLen)
                                    .Memory[..(maxLoggingBodyLen)];
                                var origPos = context.Request.Body.Position;
                                context.Request.Body.Position = 0;
                                var read = context.Request.Body.ReadAsync(blob, CancellationToken.None).Result;
                                bodyStr = Encoding.UTF8.GetString(blob[..read].Span);
                                context.Request.Body.Position = origPos;
                            }
                        }
                        catch (Exception)
                        {
                            //ignore
                        }
                    }

                    var request = new
                    {
                        IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                        Host = context.Request.Host.ToString(),
                        Path = context.Request.Path.ToString(),
                        IsHttps = context.Request.IsHttps,
                        Scheme = context.Request.Scheme,
                        Method = context.Request.Method,
                        ContentType = context.Request.ContentType,
                        Protocol = context.Request.Protocol,
                        QueryString = context.Request.QueryString.ToString(),
                        Query = context.Request.Query.ToDictionary(x => x.Key, y => y.Value.ToString()),
                        Headers = context.Request.Headers.ToDictionary(x => x.Key, y => y.Value.ToString()),
                        Cookies = context.Request.Cookies.ToDictionary(x => x.Key, y => y.Value.ToString()),
                        BodyString = bodyStr,
                    };
                    diagnosticContext.Set("Request", request, true);
                };
            });
        }
        else
        {
            Console.WriteLine($"Request logging disabled");
        }

        return app;
    }
}
