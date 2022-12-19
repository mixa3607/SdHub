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

namespace SdHub.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UserSettingsController : ControllerBase
{
    private readonly SdHubDbContext _db;
    private readonly IMapper _mapper;
    private readonly IUserFromTokenService _fromTokenService;

    public UserSettingsController(SdHubDbContext db, IMapper mapper, IUserFromTokenService fromTokenService)
    {
        _db = db;
        _mapper = mapper;
        _fromTokenService = fromTokenService;
    }

    [HttpGet("[action]")]
    public async Task<PaginationResponse<UserApiTokenModel>> GetApiKeys(CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();
        var userJwt = _fromTokenService.Get()!;
        var user = await _db.Users
            .Include(x => x.ApiTokens)
            .ApplyFilter(guid: userJwt.Guid)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(ct);
        if (user == null)
            ModelState.AddError(ModelStateErrors.UserNotFound).ThrowIfNotValid();
        var keys = _mapper.Map<UserApiTokenModel[]>(user!.ApiTokens!);
        return new PaginationResponse<UserApiTokenModel>()
        {
            Skip = 0,
            Take = keys.Length,
            Total = keys.Length,
            Items = keys
        };
    }

    [HttpPost("[action]")]
    public async Task<UserApiTokenModel> AddApiKey(CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();
        var userJwt = _fromTokenService.Get()!;
        var user = await _db.Users
            .ApplyFilter(guid: userJwt.Guid)
            .FirstOrDefaultAsync(ct);
        if (user == null)
            ModelState.AddError(ModelStateErrors.UserNotFound).ThrowIfNotValid();

        var buff = new byte[24];
        Random.Shared.NextBytes(buff);
        var b64 = Convert.ToBase64String(buff);
        var entity = new UserApiTokenEntity()
        {
            Token = b64,
            UserId = user!.Id,
            CreatedAt = DateTimeOffset.Now,
            ExpiredAt = DateTimeOffset.Now.AddYears(1)
        };
        _db.ApiTokens.Add(entity);
        await _db.SaveChangesAsync(CancellationToken.None);
        return _mapper.Map<UserApiTokenModel>(entity);
    }

    [HttpPost("[action]")]
    public async Task<UserApiTokenModel> RevokeApiKey([FromBody] RevokeApiKeyRequest req,
        CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();
        var userJwt = _fromTokenService.Get()!;
        var user = await _db.Users
            .Include(x => x.ApiTokens!.Where(y => y.Token == req.Token))
            .ApplyFilter(guid: userJwt.Guid)
            .FirstOrDefaultAsync(ct);
        if (user == null)
            ModelState.AddError(ModelStateErrors.UserNotFound).ThrowIfNotValid();
        if (user!.ApiTokens!.Count == 0)
            ModelState.AddError("Api token not found").ThrowIfNotValid();

        user.ApiTokens[0].ExpiredAt = default;
        await _db.SaveChangesAsync(CancellationToken.None);
        return _mapper.Map<UserApiTokenModel>(user.ApiTokens[0]);
    }
}