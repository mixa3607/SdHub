using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SdHub.Services.Mailing;

public class EmailCheckerService : IEmailCheckerService
{
    private readonly IReadOnlyList<string> _blackList;
    private readonly IReadOnlyList<string> _whiteList;

    public EmailCheckerService()
    {
        _blackList = File.ReadAllLines("./files/mailing/emails-blacklist.txt").Select(x => x.Trim()).ToArray();
        _whiteList = File.ReadAllLines("./files/mailing/emails-whitelist.txt").Select(x => x.Trim()).ToArray();
    }

    public Task<EmailTrustLevel> CheckEmailAsync(string email)
    {
        var domain = email.Split('@').Last();
        if (_whiteList.Contains(domain))
        {
            return Task.FromResult(EmailTrustLevel.Allow);
        }
        else if (_blackList.Contains(domain))
        {
            return Task.FromResult(EmailTrustLevel.Deny);
        }
        else
        {
            return Task.FromResult(EmailTrustLevel.Suspicious);
        }
    }
}