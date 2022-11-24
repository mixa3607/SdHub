namespace SdHub.Hangfire.Jobs;

public interface IHangfireBackgroundJobRunner
{
    string Name { get; }
}