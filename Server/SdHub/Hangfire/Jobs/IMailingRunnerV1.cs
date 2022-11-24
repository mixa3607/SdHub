using System.Threading;
using System.Threading.Tasks;

namespace SdHub.Hangfire.Jobs;

public interface IMailingRunnerV1 : IHangfireBackgroundJobRunner
{
    Task SendConfirmEmailCodeAsync(string to, string code, CancellationToken ct = default);
    Task SendResetPasswordCodeAsync(string to, string code, CancellationToken ct = default);
}