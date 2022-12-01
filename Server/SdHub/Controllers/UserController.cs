using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using SdHub.Constants;
using SdHub.Database;
using SdHub.Database.Entities.User;
using SdHub.Database.Extensions;
using SdHub.Extensions;
using SdHub.Hangfire.Jobs;
using SdHub.Models;
using SdHub.Models.User;
using SdHub.Options;
using SdHub.Services.Captcha;
using SdHub.Services.Mailing;
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
    private readonly IEmailCheckerService _emailChecker;
    private readonly ITempCodesService _tempCodesService;
    private readonly AppInfoOptions _appInfo;

    public UserController(SdHubDbContext db, IUserFromTokenService fromTokenService, IMapper mapper,
        IJwtGeneratorService jwtGenerator, IUserPasswordService passwordService,
        IRefreshTokenService refreshTokenService, ILogger<UserController> logger, ICaptchaValidator captchaValidator,
        IEmailCheckerService emailChecker, ITempCodesService tempCodesService, IOptions<AppInfoOptions> appInfo)
    {
        _db = db;
        _fromTokenService = fromTokenService;
        _mapper = mapper;
        _jwtGenerator = jwtGenerator;
        _passwordService = passwordService;
        _refreshTokenService = refreshTokenService;
        _logger = logger;
        _captchaValidator = captchaValidator;
        _emailChecker = emailChecker;
        _tempCodesService = tempCodesService;
        _appInfo = appInfo.Value;
    }

    [HttpGet("[action]")]
    public async Task<GetMeResponse> GetMe([FromQuery] GetMeRequest req, CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();
        var userJwt = _fromTokenService.Get()!;
        var user = await _db.Users.ApplyFilter(guid: userJwt.Guid).FirstOrDefaultAsync(ct);
        if (user == null)
            ModelState.AddError(ModelStateErrors.UserNotFound).ThrowIfNotValid();

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

        if (_appInfo.DisableUsersRegistration)
            ModelState.AddError(ModelStateErrors.UserRegistrationDisabled).ThrowIfNotValid();

        if (await _db.Users.ApplyFilter(email: req.Email).FirstOrDefaultAsync(ct) != null)
            ModelState.AddError(ModelStateErrors.UserWithEmailExist).ThrowIfNotValid();
        if (await _db.Users.ApplyFilter(login: req.Login).FirstOrDefaultAsync(ct) != null)
            ModelState.AddError(ModelStateErrors.UserWithLoginExist).ThrowIfNotValid();
        if (!await _captchaValidator.ValidateCodeAsync(req.CaptchaCode!, req.CaptchaType))
            ModelState.AddError(ModelStateErrors.InvalidCaptcha).ThrowIfNotValid();
        if (await _emailChecker.CheckEmailAsync(req.Email!) != EmailTrustLevel.Allow)
            ModelState.AddError("Not acceptable email. Try gmail?").ThrowIfNotValid();

        var user = new UserEntity()
        {
            Guid = Guid.NewGuid(),
            Email = req.Email,
            Login = req.Login,
            PasswordHash = _passwordService.CreatePasswordHash(req.Password!),
            Plan = await _db.UserPlans.FirstOrDefaultAsync(x => x.Name == RatesPlanTypes.RegUserPlan, ct),
            Roles = new List<string>() { UserRoleTypes.User },
            EmailConfirmationLastSend = DateTimeOffset.Now,
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync(CancellationToken.None);

        var code = await _tempCodesService.CreateAsync(user.EmailNormalized!, 10, TimeSpan.FromMinutes(30),
            TempCodeType.EmailConfirmation, ct);
        BackgroundJob.Enqueue<IMailingRunnerV1>(x => x.SendConfirmEmailCodeAsync(req.Email!, code, default));

        return new RegisterResponse()
        {
            SendToEmail = req.Email,
        };
    }

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<LoginResponse> LoginByPassword([FromBody] LoginByPasswordRequest req,
        CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();

        if (!await _captchaValidator.ValidateCodeAsync(req.CaptchaCode!, req.CaptchaType))
            ModelState.AddError(ModelStateErrors.InvalidCaptcha).ThrowIfNotValid();

        var user = await _db.Users.ApplyFilter(loginOrEmail: req.Login).FirstOrDefaultAsync(ct);

        if (user == null)
            ModelState.AddError(ModelStateErrors.UserNotFound).ThrowIfNotValid();
        if (!_passwordService.Validate(req.Password, user!.PasswordHash))
            ModelState.AddError(ModelStateErrors.BadCreds).ThrowIfNotValid();
        if (user.EmailConfirmedAt == null)
            ModelState.AddError(ModelStateErrors.EmailNotConfirmed).ThrowIfNotValid();

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
            ModelState.AddError(ModelStateErrors.RefreshTokenNotExist).ThrowIfNotValid();
        if (refreshTokenEntity!.UsedAt != null)
            _logger.LogError("Refresh token used more then one time: {rt}", refreshTokenEntity.Token);

        var user = await _db.Users.ApplyFilter(guid: refreshTokenEntity!.UserGuid).FirstOrDefaultAsync(ct);
        if (user == null)
            ModelState.AddError(ModelStateErrors.UserNotFound).ThrowIfNotValid();

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
        var user = await _db.Users.ApplyFilter(loginOrEmail: req.Login!).FirstOrDefaultAsync(ct);

        if (user == null)
            ModelState.AddError(ModelStateErrors.UserNotFound).ThrowIfNotValid();

        var checkCodeResult = await _tempCodesService.ActivateAsync(user!.EmailNormalized!, req.Code!, true, ct);
        if (checkCodeResult != TempCodeActivateResult.Ok)
            ModelState.AddError(ModelStateErrors.BadConfirmationCode).ThrowIfNotValid();

        user.EmailConfirmedAt = DateTimeOffset.Now;
        await _db.SaveChangesAsync(CancellationToken.None);

        return new ConfirmEmailResponse()
        {
            Success = true
        };
    }

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<SendResetPasswordEmailResponse> SendResetPasswordEmail(
        [FromBody] SendResetPasswordEmailRequest req,
        CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();

        if (!await _captchaValidator.ValidateCodeAsync(req.CaptchaCode!, req.CaptchaType))
            ModelState.AddError(ModelStateErrors.InvalidCaptcha).ThrowIfNotValid();

        var user = await _db.Users.ApplyFilter(loginOrEmail: req.Login).FirstOrDefaultAsync(ct);
        if (user == null)
            ModelState.AddError(ModelStateErrors.UserNotFound).ThrowIfNotValid();

        var code = await _tempCodesService.CreateAsync(user!.EmailNormalized!, 10, TimeSpan.FromMinutes(30),
            TempCodeType.PasswordReset, ct);
        BackgroundJob.Enqueue<IMailingRunnerV1>(x => x.SendResetPasswordCodeAsync(user.Email!, code, default));

        return new SendResetPasswordEmailResponse()
        {
            Success = true
        };
    }

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<SendEmailConfirmationEmailResponse> SendEmailConfirmationEmail(
        [FromBody] SendEmailConfirmationEmailRequest req,
        CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();

        if (!await _captchaValidator.ValidateCodeAsync(req.CaptchaCode!, req.CaptchaType))
            ModelState.AddError(ModelStateErrors.InvalidCaptcha).ThrowIfNotValid();

        var user = await _db.Users.ApplyFilter(loginOrEmail: req.Login).FirstOrDefaultAsync(ct);
        if (user == null)
            ModelState.AddError(ModelStateErrors.UserNotFound).ThrowIfNotValid();

        var code = await _tempCodesService.CreateAsync(user!.EmailNormalized!, 10, TimeSpan.FromMinutes(30),
            TempCodeType.EmailConfirmation, ct);
        BackgroundJob.Enqueue<IMailingRunnerV1>(x => x.SendConfirmEmailCodeAsync(user.Email!, code, default));

        return new SendEmailConfirmationEmailResponse()
        {
            Success = true
        };
    }

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ResetPasswordResponse> ResetPassword([FromBody] ResetPasswordRequest req,
        CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();

        var user = await _db.Users.ApplyFilter(loginOrEmail: req.Login).FirstOrDefaultAsync(ct);
        if (user == null)
            ModelState.AddError(ModelStateErrors.UserNotFound).ThrowIfNotValid();

        var checkCodeResult = await _tempCodesService.ActivateAsync(user!.EmailNormalized!, req.Code!, true, ct);
        if (checkCodeResult != TempCodeActivateResult.Ok)
            ModelState.AddError(ModelStateErrors.BadConfirmationCode).ThrowIfNotValid();

        var newPasswdHash = _passwordService.CreatePasswordHash(req.NewPassword!);
        user.PasswordHash = newPasswdHash;
        user.EmailConfirmedAt ??= DateTimeOffset.Now;
        await _db.SaveChangesAsync(CancellationToken.None);

        return new ResetPasswordResponse()
        {
            Success = true
        };
    }

    [HttpGet("[action]")]
    [AllowAnonymous]
    public async Task<GetUserResponse> Get([FromQuery] GetUserRequest req,
        CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();
        var user = await _db.Users.ApplyFilter(loginOrEmail: req.Login, anonymous: null).FirstOrDefaultAsync(ct);
        if (user == null)
            ModelState.AddError(ModelStateErrors.UserNotFound).ThrowIfNotValid();

        var model = _mapper.Map<UserModel>(user);
        return new GetUserResponse()
        {
            User = model
        };
    }

    [HttpPost("[action]")]
    public async Task<EditUserResponse> Edit([FromBody] EditUserRequest req,
        CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();

        var userJwt = _fromTokenService.Get()!;
        var user = await _db.Users.ApplyFilter(loginOrEmail: req.Login).FirstOrDefaultAsync(ct);

        if (user == null)
            ModelState.AddError(ModelStateErrors.UserNotFound).ThrowIfNotValid();
        if (userJwt.Guid != user!.Guid && !user.Roles.Contains(UserRoleTypes.Admin))
            ModelState.AddError(ModelStateErrors.UserEditNotAllowed).ThrowIfNotValid();

        if (req.About?.Trim() == "")
            user.About = null;
        else if (user.About != req.About)
            user.About = req.About;

        await _db.SaveChangesAsync(CancellationToken.None);

        var model = _mapper.Map<UserModel>(user);
        return new EditUserResponse()
        {
            User = model
        };
    }
}