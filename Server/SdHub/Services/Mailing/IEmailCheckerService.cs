using System.Threading.Tasks;

namespace SdHub.Services.Mailing;

public interface IEmailCheckerService
{
    Task<EmailTrustLevel> CheckEmailAsync(string email);
}