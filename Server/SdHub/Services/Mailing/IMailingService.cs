using System;
using System.Threading;
using System.Threading.Tasks;

namespace SdHub.Services.Mailing;

public interface IMailingService
{
    Task SendConfirmEmailCodeAsync(string to, string code, CancellationToken ct = default);
    Task SendResetPasswordCodeAsync(string to, string code, CancellationToken ct = default);
}