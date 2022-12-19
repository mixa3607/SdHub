using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using SdHub.Database;

namespace SdHub.Hangfire.Jobs;

public class FixImagesRecurrentJob : IHangfireRecurrentJob
{
    public FixImagesRecurrentJob(SdHubDbContext db)
    {
        _db = db;
    }

    public string Name => "FixImagesThumbs_" + Queue;
    public string CronExpression => "*/5 * * * *"; //every 5 minutes
    public string Queue => "default";

    private readonly SdHubDbContext _db;

    public void UpdateJob()
    {
        RecurringJob.AddOrUpdate<FixImagesRecurrentJob>(Name,
            x => x.ExecuteAsync(default),
            CronExpression,
            TimeZoneInfo.Utc, Queue);
    }

    [JobDisplayName("Fix images thumbs")]
    public async Task ExecuteAsync(CancellationToken ct = default)
    {
        var batchSize = 100;

        var query = _db.Images
            .OrderBy(x => x.Id)
            .Where(x => x.ThumbImageId == null || x.CompressedImageId == null)
            .Where(x => x.DeletedAt == null);
        var readRows = 0;
        var offset = 0;

        do // тут могут быть минорные пропуски но т.к. джоба регулярная и как бы хрен с ней
        {
            var images = await query
                .Skip(offset)
                .Take(batchSize)
                .ToArrayAsync(ct);
            readRows = images.Length;
            offset += images.Length;
            foreach (var imageEntity in images)
            {
                BackgroundJob.Enqueue<IImageConvertRunnerV1>(x => x.GenerateImagesAsync(imageEntity.Id, false, default));
            }
        } while (readRows > 0);
    }
}