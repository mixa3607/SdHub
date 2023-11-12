using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SdHub.Constants;
using SdHub.Database.Entities.Users;
using SdHub.Hangfire.Jobs;
using SdHub.Models;
using SdHub.Models.User;
using SdHub.Options;
using SdHub.Services.Captcha;
using SdHub.Services.Mailing;
using SdHub.Services.RatesPlan;
using SdHub.Services.TempCodes;
using SdHub.Services.Tokens;
using SdHub.Services.User;
using SdHub.Shared.AspErrorHandling.ModelState;

namespace SdHub.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UserController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IUserFromTokenService _fromTokenService;
    private readonly IUserPasswordService _passwordService;
    private readonly ILogger<UserController> _logger;
    private readonly ICaptchaValidator _captchaValidator;
    private readonly IEmailCheckerService _emailChecker;
    private readonly ITempCodesService _tempCodesService;
    private readonly AppInfoOptions _appInfo;
    private readonly IUserService _userService;
    private readonly IUserPlanService _userPlanService;

    public UserController(IUserFromTokenService fromTokenService, IMapper mapper,
        IUserPasswordService passwordService, ILogger<UserController> logger, ICaptchaValidator captchaValidator,
        IEmailCheckerService emailChecker, ITempCodesService tempCodesService, IOptions<AppInfoOptions> appInfo,
        IUserService userService, IUserPlanService userPlanService)
    {
        _fromTokenService = fromTokenService;
        _mapper = mapper;
        _passwordService = passwordService;
        _logger = logger;
        _captchaValidator = captchaValidator;
        _emailChecker = emailChecker;
        _tempCodesService = tempCodesService;
        _userService = userService;
        _userPlanService = userPlanService;
        _appInfo = appInfo.Value;
    }

    [HttpGet("[action]")]
    public async Task<UserModel> GetMe(CancellationToken ct = default)
    {
        var userJwt = _fromTokenService.Get()!;
        var user = await _userService.GetUserByGuidAsync(userJwt.Guid, ct);
        if (user == null)
            ModelState.AddError(ModelStateErrors.UserNotFound).ThrowIfNotValid();

        return _mapper.Map<UserModel>(user);
    }

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<RegisterResponse> Register([FromBody] RegisterRequest req,
        CancellationToken ct = default)
    {
        if (_appInfo.DisableUsersRegistration)
            ModelState.AddError(ModelStateErrors.UserRegistrationDisabled).ThrowIfNotValid();

        if (!await _captchaValidator.ValidateCodeAsync(req.CaptchaCode!, req.CaptchaType))
            ModelState.AddError(ModelStateErrors.InvalidCaptcha).ThrowIfNotValid();

        if (await _emailChecker.CheckEmailAsync(req.Email) != EmailTrustLevel.Allow)
            ModelState.AddError("Not acceptable email. Try gmail?").ThrowIfNotValid();

        if (await _userService.LoginIsUsedAsync(req.Login!, ct))
            ModelState.AddError(ModelStateErrors.UserWithEmailExist).ThrowIfNotValid();

        if (await _userService.EmailIsUsedAsync(req.Email, ct))
            ModelState.AddError(ModelStateErrors.UserWithLoginExist).ThrowIfNotValid();

        var user = new UserEntity()
        {
            Email = req.Email,
            Login = req.Login,
            PasswordHash = _passwordService.CreatePasswordHash(req.Password!),
            PlanId = (await _userPlanService.GetDefaultAsync(ct)).Id,
            Roles = new List<string>() { UserRoleTypes.User },
        };
        user = await _userService.AddUserAsync(user, ct);

        return new RegisterResponse()
        {
            User = _mapper.Map<UserModel>(user),
            SendToEmail = req.Email,
        };
    }


    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<SendEmailConfirmationEmailResponse> SendEmailConfirmationEmail(
        [FromBody] SendEmailConfirmationEmailRequest req,
        CancellationToken ct = default)
    {
        if (!await _captchaValidator.ValidateCodeAsync(req.CaptchaCode!, req.CaptchaType))
            ModelState.AddError(ModelStateErrors.InvalidCaptcha).ThrowIfNotValid();

        var user = await _userService.GetUserByLoginOrEmailAsync(req.Login, ct);
        if (user == null)
            ModelState.AddError(ModelStateErrors.UserNotFound).ThrowIfNotValid();

        var codeKey = $"email_confirm_{user!.Guid}_{user.EmailNormalized}";
        var code = await _tempCodesService.CreateAsync(codeKey, 5, TimeSpan.FromMinutes(30), ct);
        BackgroundJob.Enqueue<IMailingRunnerV1>(x => x.SendConfirmEmailCodeAsync(user.Email, code, default));

        return new SendEmailConfirmationEmailResponse()
        {
            Success = true
        };
    }

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ConfirmEmailResponse> ConfirmEmail([FromBody] ConfirmEmailRequest req,
        CancellationToken ct = default)
    {
        var user = await _userService.GetUserByLoginOrEmailAsync(req.Login, ct);

        if (user == null)
            ModelState.AddError(ModelStateErrors.UserNotFound).ThrowIfNotValid();

        var codeKey = $"email_confirm_{user!.Guid}_{user.EmailNormalized}";
        var checkCodeResult = await _tempCodesService.ActivateAsync(codeKey, req.Code!, true, ct);
        if (checkCodeResult != TempCodeActivateResult.Ok)
            ModelState.AddError(ModelStateErrors.BadConfirmationCode).ThrowIfNotValid();

        user = await _userService.ConfirmEmailAsync(user, ct);

        return new ConfirmEmailResponse()
        {
            User = _mapper.Map<UserModel>(user),
            Success = true
        };
    }

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<SendResetPasswordEmailResponse> SendResetPasswordEmail(
        [FromBody] SendResetPasswordEmailRequest req, CancellationToken ct = default)
    {
        if (!await _captchaValidator.ValidateCodeAsync(req.CaptchaCode!, req.CaptchaType))
            ModelState.AddError(ModelStateErrors.InvalidCaptcha).ThrowIfNotValid();

        var user = await _userService.GetUserByLoginOrEmailAsync(req.Login, ct);
        if (user == null)
            ModelState.AddError(ModelStateErrors.UserNotFound).ThrowIfNotValid();

        var codeKey = $"password_reset_{user!.Guid}";
        var code = await _tempCodesService.CreateAsync(codeKey, 5, TimeSpan.FromMinutes(30), ct);
        BackgroundJob.Enqueue<IMailingRunnerV1>(x => x.SendResetPasswordCodeAsync(user.Email!, code, default));

        return new SendResetPasswordEmailResponse()
        {
            Success = true
        };
    }

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ResetPasswordResponse> ResetPassword([FromBody] ResetPasswordRequest req,
        CancellationToken ct = default)
    {
        var user = await _userService.GetUserByLoginOrEmailAsync(req.Login, ct);
        if (user == null)
            ModelState.AddError(ModelStateErrors.UserNotFound).ThrowIfNotValid();

        var codeKey = $"password_reset_{user!.Guid}";
        var checkCodeResult = await _tempCodesService.ActivateAsync(codeKey, req.Code, true, ct);
        if (checkCodeResult != TempCodeActivateResult.Ok)
            ModelState.AddError(ModelStateErrors.BadConfirmationCode).ThrowIfNotValid();

        var newPasswdHash = _passwordService.CreatePasswordHash(req.NewPassword);
        user = await _userService.UpdateUserAsync(new UserUpdate() { PasswordHash = newPasswdHash }, user.Guid, ct);
        user = await _userService.ConfirmEmailAsync(user.Guid, ct);

        return new ResetPasswordResponse()
        {
            User = _mapper.Map<UserModel>(user),
            Success = true
        };
    }

    [HttpGet("[action]")]
    [AllowAnonymous]
    public async Task<GetUserResponse> Get([FromQuery] GetUserRequest req,
        CancellationToken ct = default)
    {
        var user = await _userService.GetUserByLoginOrEmailAsync(req.Login, ct);
        if (user == null)
            ModelState.AddError(ModelStateErrors.UserNotFound).ThrowIfNotValid();

        return new GetUserResponse()
        {
            User = _mapper.Map<UserModel>(user)
        };
    }

    [HttpPost("[action]")]
    public async Task<EditUserResponse> Edit([FromBody] EditUserRequest req,
        CancellationToken ct = default)
    {
        var userJwt = _fromTokenService.Get()!;
        var user = await _userService.GetUserByGuidAsync(userJwt.Guid, ct);

        if (user == null)
            ModelState.AddError(ModelStateErrors.UserNotFound).ThrowIfNotValid();

        var update = _mapper.Map<UserUpdate>(req);
        user = await _userService.UpdateUserAsync(update, userJwt.Guid, ct);

        return new EditUserResponse()
        {
            User = _mapper.Map<UserModel>(user)
        };
    }
}