using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SdHub.Services.Mailing;

namespace SdHub.Hangfire.Jobs;

public class MailingRunner : IMailingRunnerV1
{
    public string Name => "Mailer";

    private readonly IMailingService _mailingService;

    public MailingRunner(IMailingService mailingService)
    {
        _mailingService = mailingService;
    }

    [DisplayName("Send confirm email: {0} => {1}")]
    public Task SendConfirmEmailCodeAsync(string to, string code, CancellationToken ct = default)
    {
        return _mailingService.SendConfirmEmailCodeAsync(to, code, ct);
    }

    [DisplayName("Send reset passwd: {0} => {1}")]
    public Task SendResetPasswordCodeAsync(string to, string code, CancellationToken ct = default)
    {
        return _mailingService.SendResetPasswordCodeAsync(to, code, ct);
    }
}