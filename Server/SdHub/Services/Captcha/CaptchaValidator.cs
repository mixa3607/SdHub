using System;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.Extensions.Options;
using SdHub.Models.Enums;
using SdHub.Options;

namespace SdHub.Services.Captcha;

public class CaptchaValidator : ICaptchaValidator
{
    private readonly RecaptchaOptions _options;

    public CaptchaValidator(IOptions<RecaptchaOptions> options)
    {
        _options = options.Value;
    }

    public async Task<bool> ValidateCodeAsync(string code, CaptchaType type)
    {
        if (_options.Bypass)
            return true;

        if (type != CaptchaType.ReCaptchaV2)
            throw new NotImplementedException();

        var req = new { secret = _options.SecretKey, response = code };
        var respC = await "https://www.google.com/recaptcha/api/siteverify".PostUrlEncodedAsync(req);
        var resp = await respC.GetJsonAsync<ReCaptchaResponse>();
        return resp.Success;
    }
}