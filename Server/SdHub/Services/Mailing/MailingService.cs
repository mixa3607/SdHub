using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SdHub.Options;
using SdHub.Services.ErrorHandling.Exceptions;

namespace SdHub.Services.Mailing;

public class MailingService : IMailingService
{
    private readonly MailingOptions _options;
    private readonly AppInfoOptions _appInfo;
    private readonly ILogger<MailingService> _logger;
    private readonly IEmailCheckerService _emailChecker;
    private readonly IFluentEmail _fluentEmail;

    public MailingService(IOptions<MailingOptions> options,
        ILogger<MailingService> logger, IEmailCheckerService emailChecker, IFluentEmail fluentEmail,
        IOptions<AppInfoOptions> appInfo)
    {
        _options = options.Value;
        _logger = logger;
        _emailChecker = emailChecker;
        _fluentEmail = fluentEmail;
        _appInfo = appInfo.Value;
        InitMaildir();
    }

    public async Task SendConfirmEmailCodeAsync(string to, string code, CancellationToken ct = default)
    {
        var templatePath = Path.Combine(_options.TemplatesDir!, "SendConfirmEmailLink.liquid");
        await _fluentEmail
            .To(to)
            .Subject("Email confirmation")
            .UsingTemplateFromFile(templatePath, new { Code = $"\"{code}\"", SiteUrl = _appInfo.BaseUrl })
            .SendAsync(ct);
    }

    public async Task SendResetPasswordCodeAsync(string to, string code, CancellationToken ct = default)
    {
        var templatePath = Path.Combine(_options.TemplatesDir!, "SendResetPasswordCode.liquid");
        await _fluentEmail
            .To(to)
            .Subject("Password reset")
            .UsingTemplateFromFile(templatePath, new { Code = $"\"{code}\"", SiteUrl = _appInfo.BaseUrl })
            .SendAsync(ct);
    }

    private async Task ThrowIfEmailNotAllowAsync(string? email)
    {
        var modelState = new Dictionary<string, List<string>>() { { "", new List<string>() } };
        if (string.IsNullOrWhiteSpace(email))
        {
            modelState[""].Add("Email must be not empty");
            throw new BadHttpRequestModelStateException(modelState);
        }

        var checkResult = await _emailChecker.CheckEmailAsync(email);
        if (checkResult > _options.MailTrustLevel)
        {
            modelState[""].Add($"Email {email} not allowed");
            throw new BadHttpRequestModelStateException(modelState);
        }
        else if (checkResult <= EmailTrustLevel.Suspicious)
        {
            _logger.LogWarning("Suspicious email: {email}", email);
        }
    }

    #region maildir

    private string GetMaildirRoot() => Path.GetFullPath(_options.PathToMaildir);
    private string GetMaildirTmp() => Path.Combine(GetMaildirRoot(), "tmp");
    private string GetMaildirCur() => Path.Combine(GetMaildirRoot(), "cur");
    private string GetMaildirNew() => Path.Combine(GetMaildirRoot(), "new");

    private void InitMaildir()
    {
        var dirs = new[] { GetMaildirTmp(), GetMaildirCur(), GetMaildirNew() };
        foreach (var dir in dirs)
        {
            if (Directory.Exists(dir))
                continue;
            _logger.LogInformation("Create dir {dir} for maildir", dir);
            Directory.CreateDirectory(dir);
        }
    }

    #endregion
}