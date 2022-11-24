using System.Threading;
using System.Threading.Tasks;

namespace SdHub.Hangfire.Jobs;

public interface IImageConvertRunnerV1 : IHangfireBackgroundJobRunner
{
    Task GenerateImagesAsync(long imageId, CancellationToken ct = default);
}