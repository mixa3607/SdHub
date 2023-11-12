using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SdHub.Models;
using SdHub.Options;

namespace SdHub.Controllers;

[Route("api/v1/[controller]")]
[Produces("application/json")]
public class AppInfoController : ControllerBase
{
    private readonly AppInfoOptions _appInfo;
    private readonly RecaptchaOptions _recaptcha;

    public AppInfoController(IOptions<AppInfoOptions> appInfo, IOptions<RecaptchaOptions> recaptcha)
    {
        _recaptcha = recaptcha.Value;
        _appInfo = appInfo.Value;
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("[action]")]
    [AllowAnonymous]
    public FrontendSettings Get()
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