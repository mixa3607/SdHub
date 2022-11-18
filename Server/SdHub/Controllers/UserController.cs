using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using SdHub.Database;
using SdHub.Extensions;
using SdHub.Models;
using SdHub.Models.User;
using SdHub.Services.Captcha;
using SdHub.Services.Tokens;
using SdHub.Services.User;

namespace SdHub.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UserController : ControllerBase
{
    private readonly SdHubDbContext _db;
    private readonly IMapper _mapper;
    private readonly IUserFromTokenService _fromTokenService;
    private readonly IJwtGeneratorService _jwtGenerator;
    private readonly IUserPasswordService _passwordService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ILogger<UserController> _logger;
    private readonly ICaptchaValidator _captchaValidator;

    public UserController(SdHubDbContext db, IUserFromTokenService fromTokenService, IMapper mapper,
        IJwtGeneratorService jwtGenerator, IUserPasswordService passwordService,
        IRefreshTokenService refreshTokenService, ILogger<UserController> logger, ICaptchaValidator captchaValidator)
    {
        _db = db;
        _fromTokenService = fromTokenService;
        _mapper = mapper;
        _jwtGenerator = jwtGenerator;
        _passwordService = passwordService;
        _refreshTokenService = refreshTokenService;
        _logger = logger;
        _captchaValidator = captchaValidator;
    }

    [HttpGet("[action]")]
    public async Task<GetMeResponse> GetMe([FromQuery] GetMeRequest req, CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();
        var userJwt = _fromTokenService.Get()!;
        var user = await _db.Users.FirstOrDefaultAsync(x => x.DeletedAt == null && userJwt.Guid == x.Guid, ct);
        if (user == null)
            ModelState.AddError("User not exist").ThrowIfNotValid();

        var model = _mapper.Map<UserModel>(user);
        return new GetMeResponse()
        {
            User = model
        };
    }

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<RegisterResponse> Register([FromBody] RegisterRequest req,
        CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();

        if (!await _captchaValidator.ValidateCodeAsync(req.CaptchaCode!, req.CaptchaType))
            ModelState.AddError("Invalid captcha. Try again").ThrowIfNotValid();

        ModelState.AddError("Registration closed").ThrowIfNotValid();
        return new RegisterResponse();
    }

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<LoginResponse> LoginByPassword([FromBody] LoginByPasswordRequest req,
        CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();

        if (!await _captchaValidator.ValidateCodeAsync(req.CaptchaCode!, req.CaptchaType))
            ModelState.AddError("Invalid captcha. Try again").ThrowIfNotValid();

        req.Login = req.Login!.Normalize().ToUpper();
        var user = await _db.Users
            .FirstOrDefaultAsync(x => x.DeletedAt == null && x.LoginNormalized == req.Login, ct);
        if (user == null || !_passwordService.Validate(req.Password, user!.PasswordHash))
            ModelState.AddError("Login or password is wrong").ThrowIfNotValid();

        var userModel = _mapper.Map<UserModel>(user);
        var jwt = await _jwtGenerator.GenerateAsync(userModel, ct: ct);
        var remoteIp = Request.HttpContext.Connection.RemoteIpAddress!;
        var userAgent = Request.Headers[HeaderNames.UserAgent];
        var refreshToken =
            await _refreshTokenService.CreateNewAsync(userModel.Guid, remoteIp.ToString(), userAgent, ct);

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
    public async Task<LoginResponse> LoginByRefreshToken([FromBody] LoginByRefreshTokenRequest req,
        CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();

        var refreshTokenEntity = await _refreshTokenService.GetAsync(req.RefreshToken, ct);
        if (refreshTokenEntity == null)
        {
            ModelState.AddError("Refresh token not exist").ThrowIfNotValid();
        }
        else if (refreshTokenEntity.UsedAt != null)
        {
            _logger.LogError("Refresh token used more then one time: {rt}", refreshTokenEntity.Token);
        }

        var user = await _db.Users.FirstOrDefaultAsync(x =>
            x.DeletedAt == null && x.Guid == refreshTokenEntity!.UserGuid, ct);
        if (user == null)
            ModelState.AddError("User not exist").ThrowIfNotValid();

        var userModel = _mapper.Map<UserModel>(user);

        var jwt = await _jwtGenerator.GenerateAsync(userModel, ct: ct);
        var remoteIp = Request.HttpContext.Connection.RemoteIpAddress!;
        var userAgent = Request.Headers[HeaderNames.UserAgent];
        var refreshToken =
            await _refreshTokenService.CreateNewAsync(userModel.Guid, remoteIp.ToString(), userAgent, ct);

        if (!await _refreshTokenService.MarkAsUsedAsync(refreshTokenEntity.Token!, ct))
        {
            _logger.LogError("Не удалось обнулить рефреш токен {token}", refreshToken.Token);
        }

        return new LoginResponse()
        {
            JwtToken = jwt,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpired = refreshToken.ExpiredAt,
            User = userModel
        };
    }

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ConfirmEmailResponse> ConfirmEmail([FromBody] ConfirmEmailRequest req,
        CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();
        throw new NotImplementedException();
    }

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ResetPasswordResponse> ResetPassword([FromBody] ResetPasswordRequest req,
        CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();
        throw new NotImplementedException();
    }

    [HttpPost("[action]")]
    public async Task<ChangePasswordResponse> ChangePassword([FromBody] ChangePasswordRequest req,
        CancellationToken ct = default)
    {
        var userJwt = _fromTokenService.Get()!;
        var user = await _db.Users.FirstOrDefaultAsync(x => x.DeletedAt == null && userJwt.Guid == x.Guid, ct);
        if (user == null)
            ModelState.AddError("User not exist").ThrowIfNotValid();

        if (!_passwordService.Validate(req.Password, user.PasswordHash))
            ModelState.AddError("Login or password is wrong").ThrowIfNotValid();

        var newPasswdHash = _passwordService.CreatePasswordHash(req.NewPassword!);
        user.PasswordHash = newPasswdHash;
        await _db.SaveChangesAsync(CancellationToken.None);

        return new ChangePasswordResponse()
        {
            Success = true
        };
    }
}