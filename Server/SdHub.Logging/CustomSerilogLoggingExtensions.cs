using System.Buffers;
using System.Dynamic;
using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

namespace SdHub.Logging;

public static class CustomSerilogLoggingExtensions
{
    public static ConfigureHostBuilder AddCustomSerilog(this ConfigureHostBuilder host, SerilogOptions serilogOptions)
    {
        host.UseSerilog((ctx, l) =>
        {
            var assemblyName = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name.ToLower();
            l
                .Enrich.WithEnvironmentName()
                .Enrich.WithEnvironmentUserName()
                .Enrich.WithThreadId()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProcessName()
                .Enrich.WithThreadName()
                .Enrich.WithProcessId()
                .Enrich.WithExceptionDetails()
                .Enrich.WithAssemblyName()
                .Enrich.WithAssemblyVersion()
                .Enrich.WithMemoryUsage()
                .Destructure.ByTransforming<ExpandoObject>(e => new Dictionary<string, object>(e));

            if (!serilogOptions.DisableElastic)
            {
                var indexFormat = $"serilogs-sink-{assemblyName}-{{0:yyyy.MM.dd}}";
                if (serilogOptions.ElasticIndexPrefix != null)
                    indexFormat = $"{serilogOptions.ElasticIndexPrefix}-{indexFormat}";

                Console.WriteLine($"Elastic logging enabled. Index format: {indexFormat}");
                var options = new ElasticsearchSinkOptions(serilogOptions.ElasticUris)
                {
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                    FailureCallback = e =>
                        Console.WriteLine(
                            $"Unable to submit event template {e.MessageTemplate} with error {e.Exception}"),
                    EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
                                       EmitEventFailureHandling.WriteToFailureSink |
                                       EmitEventFailureHandling.RaiseCallback,
                    RegisterTemplateFailure = RegisterTemplateRecovery.IndexToDeadletterIndex,
                    NumberOfShards = 1,
                    NumberOfReplicas = 0,
                    IndexFormat = indexFormat,
                    DeadLetterIndexName = "deadletter-" + indexFormat,
                    BufferLogShippingInterval = TimeSpan.FromSeconds(5),
                    TemplateCustomSettings = new Dictionary<string, string>()
                    {
                        { "index.mapping.total_fields.limit", "2000" }
                    }
                };
                l.WriteTo.Elasticsearch(options);
            }

            l.WriteTo.Console();

            Console.WriteLine($"Used log level preset: {serilogOptions.LevelPreset}");
            if (serilogOptions.LevelPreset == SerilogOptionsLevelPresetType.Prod)
            {
                l.MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Verbose)
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning);
            }
            else if (serilogOptions.LevelPreset == SerilogOptionsLevelPresetType.Dev)
            {
                l.MinimumLevel.Verbose()
                    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Verbose)
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .MinimumLevel.Override("Hangfire", LogEventLevel.Information)
                    .MinimumLevel.Override("System", LogEventLevel.Verbose);
            }
        });
        return host;
    }

    public static WebApplication UseCustomSerilogging(this WebApplication app, SerilogOptions serilogOptions)
    {
        if (serilogOptions.EnableRequestLogging)
        {
            const int inMemBuffLen = 50 * 1024;
            const int maxLoggingBodyLen = 100 * 1024;

            Console.WriteLine($"Request logging enabled with level {serilogOptions.RequestLogMessageLevel}");
            app.Use((context, next) =>
            {
                if (context.Request.ContentLength > maxLoggingBodyLen)
                {
                    context.Request.EnableBuffering(inMemBuffLen);
                }

                return next();
            });

            app.UseSerilogRequestLogging(o =>
            {
                o.GetLevel = (httpContext, elapsed, ex) => serilogOptions.RequestLogMessageLevel;
                o.EnrichDiagnosticContext = (diagnosticContext, context) =>
                {
                    var bodyStr = (string?)null;
                    var body = (dynamic?)null;
                    if (context.Request.ContentType == MediaTypeNames.Application.Json)
                    {
                        try
                        {
                            if (context.Request.ContentLength > maxLoggingBodyLen)
                            {
                                bodyStr = $"Body len more than {maxLoggingBodyLen}. Cant deserialize";
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
                                try
                                {
                                    body = JsonConvert.DeserializeObject<ExpandoObject?>(bodyStr);
                                }
                                catch (Exception e)
                                {
                                    body = e.ToString();
                                }
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
                        Body = body,
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