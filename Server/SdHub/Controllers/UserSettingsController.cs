using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SdHub.Constants;
using SdHub.Database;
using SdHub.Database.Entities.Users;
using SdHub.Database.Extensions;
using SdHub.Extensions;
using SdHub.Models;
using SdHub.Models.User;
using SdHub.Services.User;
using SdHub.Shared.AspErrorHandling.ModelState;

namespace SdHub.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UserApiTokensController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IUserFromTokenService _fromTokenService;
    private readonly IUserService _userService;

    public UserApiTokensController(IMapper mapper, IUserFromTokenService fromTokenService, IUserService userService)
    {
        _mapper = mapper;
        _fromTokenService = fromTokenService;
        _userService = userService;
    }

    [HttpGet("[action]")]
    public async Task<PaginationResponse<UserApiTokenModel>> GetApiKeys(CancellationToken ct = default)
    {
        var userJwt = _fromTokenService.Get()!;
        var apiTokens = await _userService.GetApiTokensAsync(userJwt.Guid, ct);
        var keys = _mapper.Map<UserApiTokenModel[]>(apiTokens);
        return new PaginationResponse<UserApiTokenModel>()
        {
            Skip = 0,
            Take = keys.Length,
            Total = keys.Length,
            Values = keys
        };
    }

    [HttpPost("[action]")]
    public async Task<UserApiTokenModel> AddApiKey(string name, CancellationToken ct = default)
    {
        var userJwt = _fromTokenService.Get()!;
        var token = await _userService.AddApiTokenAsync(userJwt.Guid, name, DateTimeOffset.UtcNow.AddYears(1), ct);

        //var buff = new byte[24];
        //Random.Shared.NextBytes(buff);
        //var b64 = Convert.ToBase64String(buff);
        //var entity = new UserApiTokenEntity()
        //{
        //    Token = b64,
        //    UserId = user!.Id,
        //    CreatedAt = DateTimeOffset.UtcNow,
        //    ExpiredAt = DateTimeOffset.UtcNow.AddYears(1)
        //};
        //_db.ApiTokens.Add(entity);
        //await _db.SaveChangesAsync(CancellationToken.None);
        return _mapper.Map<UserApiTokenModel>(token);
    }

    [HttpPost("[action]")]
    public async Task<UserApiTokenModel> RevokeApiKey([FromBody] RevokeApiKeyRequest req,
        CancellationToken ct = default)
    {
        var userJwt = _fromTokenService.Get()!;
        var user = await _userService.GetUserByGuidAsync(userJwt.Guid, ct);
        var token = await _userService.GetApiTokenByNameAsync(req.Name, ct);
        if (token.UserId != user!.Id)
        {
            models
        }
        //var user = await _db.Users
        //    .Include(x => x.ApiTokens!.Where(y => y.Token == req.Token))
        //    .ApplyFilter(guid: userJwt.Guid)
        //    .FirstOrDefaultAsync(ct);
        //if (user == null)
        //    ModelState.AddError(ModelStateErrors.UserNotFound).ThrowIfNotValid();
        //if (user!.ApiTokens!.Count == 0)
        //    ModelState.AddError("Api token not found").ThrowIfNotValid();
        //
        //user.ApiTokens[0].ExpiredAt = default;
        //await _db.SaveChangesAsync(CancellationToken.None);
        return _mapper.Map<UserApiTokenModel>(user.ApiTokens[0]);
    }
}