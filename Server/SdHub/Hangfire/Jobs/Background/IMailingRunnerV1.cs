using Hangfire;
using System.Threading;
using System.Threading.Tasks;

namespace SdHub.Hangfire.Jobs;

public interface IMailingRunnerV1 : IHangfireBackgroundJobRunner
{
    [JobDisplayName("Send confirm email: {0} => {1}")]
    Task SendConfirmEmailCodeAsync(string to, string code, CancellationToken ct = default);

    [JobDisplayName("Send reset passwd: {0} => {1}")]
    Task SendResetPasswordCodeAsync(string to, string code, CancellationToken ct = default);
}