using Hangfire;
using System.Threading;
using System.Threading.Tasks;

namespace SdHub.Hangfire.Jobs;

public interface IImageConvertRunnerV1 : IHangfireBackgroundJobRunner
{
    [JobDisplayName("Gen images for {0}")]
    Task GenerateImagesAsync(long imageId, bool force, CancellationToken ct = default);
}