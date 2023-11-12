using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using SdHub.Attributes;
using SdHub.Constants;
using SdHub.Models;
using SdHub.Models.Enums;
using SdHub.Models.User;
using SdHub.Services.Captcha;
using SdHub.Services.Refresh;
using SdHub.Services.Tokens;
using SdHub.Services.User;
using SdHub.Shared.AspErrorHandling.ModelState;

namespace SdHub.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LoginController : ControllerBase
{
    private readonly ILogger<LoginController> _logger;
    private readonly ICaptchaValidator _captchaValidator;
    private readonly IUserService _userService;
    private readonly IJwtGeneratorService _jwtGenerator;
    private readonly IUserPasswordService _passwordService;
    private readonly IMapper _mapper;
    private readonly IRefreshTokenService _refreshTokenService;

    public LoginController(ILogger<LoginController> logger, ICaptchaValidator captchaValidator,
        IUserService userService, IJwtGeneratorService jwtGenerator, IUserPasswordService passwordService,
        IMapper mapper, IRefreshTokenService refreshTokenService)
    {
        _logger = logger;
        _captchaValidator = captchaValidator;
        _userService = userService;
        _jwtGenerator = jwtGenerator;
        _passwordService = passwordService;
        _mapper = mapper;
        _refreshTokenService = refreshTokenService;
    }


    [HttpPost("[action]")]
    [AllowAnonymous]
    [AllowExpiredJwt]
    public async Task<LoginResponse> LoginByPassword([FromBody] LoginByPasswordRequest req,
        CancellationToken ct = default)
    {
        if (!await _captchaValidator.ValidateCodeAsync(req.CaptchaCode!, req.CaptchaType))
            ModelState.AddError(ModelStateErrors.InvalidCaptcha).ThrowIfNotValid();

        var user = await _userService.GetUserByLoginOrEmailAsync(req.Login, ct);
        if (user == null)
            ModelState.AddError(ModelStateErrors.UserNotFound).ThrowIfNotValid();

        if (!_passwordService.Validate(req.Password, user!.PasswordHash))
            ModelState.AddError(ModelStateErrors.BadCreds).ThrowIfNotValid();

        if (user.EmailConfirmedAt == null)
            ModelState.AddError(ModelStateErrors.EmailNotConfirmed).ThrowIfNotValid();

        var userModel = _mapper.Map<UserModel>(user);
        var jwt = await _jwtGenerator.GenerateAsync(userModel, ct: ct);
        var userAgent = Request.Headers[HeaderNames.UserAgent].ToString();
        var refreshToken =
            await _refreshTokenService.CreateNewAsync(userModel.Guid, new[] { AudienceType.SdHub }, "not_used",
                userAgent, ct);

        return new LoginResponse()
        {
            User = userModel,
            JwtToken = jwt,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpired = refreshToken.ExpiredAt
        };
    }

    [HttpPost("[action]")]
    [AllowAnonymous]
    [AllowExpiredJwt]
    public async Task<LoginResponse> LoginByRefreshToken([FromBody] LoginByRefreshTokenRequest req,
        CancellationToken ct = default)
    {
        var refreshTokenEntity = await _refreshTokenService.GetAsync(req.RefreshToken, ct);
        if (refreshTokenEntity == null)
            ModelState.AddError(ModelStateErrors.RefreshTokenNotExist).ThrowIfNotValid();

        if (refreshTokenEntity!.UsedAt != null)
            ModelState.AddError("Refresh token used").ThrowIfNotValid();

        var user = await _userService.GetUserByGuidAsync(refreshTokenEntity.UserGuid, ct);
        if (user == null)
            ModelState.AddError(ModelStateErrors.UserNotFound).ThrowIfNotValid();

        var userModel = _mapper.Map<UserModel>(user);
        var jwt = await _jwtGenerator.GenerateAsync(userModel, ct: ct);
        var userAgent = Request.Headers[HeaderNames.UserAgent].ToString();
        var refreshToken =
            await _refreshTokenService.CreateNewAsync(userModel.Guid, new[] { AudienceType.SdHub }, "not_used",
                userAgent, ct);

        if (!await _refreshTokenService.MarkAsUsedAsync(refreshTokenEntity.Token!, ct))
            _logger.LogError("Не удалось обнулить рефреш токен {token}", refreshToken.Token);

        return new LoginResponse()
        {
            JwtToken = jwt,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpired = refreshToken.ExpiredAt,
            User = userModel
        };
    }
}