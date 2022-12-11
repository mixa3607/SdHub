using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Options;
using SdHub.Options;
using SdHub.Services.FileProc;

namespace SdHub.Hangfire.Jobs;

public class ClearCacheRecurrentJob : IHangfireRecurrentJob
{
    private readonly HangfireOptions _options;
    private readonly IFileProcessor _fileProcessor;
    public ClearCacheRecurrentJob(IOptions<HangfireOptions> options, IFileProcessor fileProcessor)
    {
        _fileProcessor = fileProcessor;
        _options = options.Value;
    }

    public string Name => "ClearCache";
    public string CronExpression => "*/5 * * * *"; //every 5 minutes
    public string Queue => _options.ServerQueue!;

    public void UpdateJob()
    {
        RecurringJob.AddOrUpdate<ClearCacheRecurrentJob>(Name,
            x => x.ExecuteAsync(default),
            CronExpression,
            TimeZoneInfo.Utc, Queue);
    }

    public async Task ExecuteAsync(CancellationToken ct = default)
    {
        await _fileProcessor.PruneCacheAsync(DateTime.UtcNow.AddMinutes(-30), ct);
    }
}