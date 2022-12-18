using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using SdHub.Models;
using SdHub.Options;

namespace SdHub.Controllers;

[AllowAnonymous]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class SettingsController : ControllerBase
{
    private readonly AppInfoOptions _appInfo;
    private readonly RecaptchaOptions _recaptcha;
    private readonly IJsonHelper _jsonHelper;

    public SettingsController(IOptions<AppInfoOptions> appInfo, IOptions<RecaptchaOptions> recaptcha,
        IJsonHelper jsonHelper)
    {
        _jsonHelper = jsonHelper;
        _recaptcha = recaptcha.Value;
        _appInfo = appInfo.Value;
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet]
    public FrontendSettings Get(CancellationToken ct = default)
    {
        var settings = new FrontendSettings()
        {
            DisableUsersRegistration = _appInfo.DisableUsersRegistration,
            DisableCaptcha = _recaptcha.Bypass,
            RecaptchaSiteKey = _recaptcha.SiteKey,
            DisableGridUploadAuth = _appInfo.DisableGridUploadAuth,
            DisableImageUploadAnon = _appInfo.DisableImageUploadAnon,
            DisableImageUploadAuth = _appInfo.DisableImageUploadAuth,
        };
        return settings;
    }
}