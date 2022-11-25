using System.Threading;
using System.Threading.Tasks;

namespace SdHub.Hangfire.Jobs;

public interface IHangfireRecurrentJob
{
    string Name { get; }
    string CronExpression { get; }
    string? Queue { get; }
    void UpdateJob();

    Task ExecuteAsync(CancellationToken ct = default);
}