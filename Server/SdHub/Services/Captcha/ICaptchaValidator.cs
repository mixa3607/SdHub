using System.Threading.Tasks;
using SdHub.Models.Enums;

namespace SdHub.Services.Captcha;

public interface ICaptchaValidator
{
    Task<bool> ValidateCodeAsync(string code, CaptchaType type);
}