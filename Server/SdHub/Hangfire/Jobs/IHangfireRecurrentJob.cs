using System.Threading.Tasks;

namespace SdHub.Hangfire.Jobs;

public interface IHangfireRecurrentJob
{
    string Name { get; }
    void UpdateJob();

    Task ExecuteAsync();
}