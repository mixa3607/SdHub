using System.Threading;
using System.Threading.Tasks;

namespace SdHub.Services.Mailing;

public interface IMailingService
{
    Task SendConfirmEmailLinkAsync(string to, string code, CancellationToken ct = default);
}