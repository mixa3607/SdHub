using System.Threading;
using System.Threading.Tasks;
using Hangfire;

namespace SdHub.Hangfire.Jobs;

public interface IBinUpdaterRunnerV1 : IHangfireBackgroundJobRunner
{
    [JobDisplayName("Update files for model: {0}")]
    Task UpdateModelVersionFilesAsync(long modelVersionId, bool force, CancellationToken ct = default);
}