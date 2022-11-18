using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SdHub.Database.Entities;
using SdHub.Extensions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SdHub.Constants;
using SdHub.Database;
using SdHub.Services.User;
using System.Linq;
using SdHub.Models.RatesLimit;
using SdHub.Database.Entities.Images;
using SdHub.Models;

namespace SdHub.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[AllowAnonymous]
public class RatesLimitController : ControllerBase
{
    private readonly SdHubDbContext _db;
    private readonly IUserFromTokenService _userFromToken;
    private readonly IMapper _mapper;

    public RatesLimitController(SdHubDbContext db, IUserFromTokenService userFromToken, IMapper mapper)
    {
        _db = db;
        _userFromToken = userFromToken;
        _mapper = mapper;
    }

    [HttpGet("[action]")]
    public async Task<GetRatesResponse> Get(CancellationToken ct = default)
    {
        ImageUploaderEntity uploader;

        var jwtUser = _userFromToken.Get();
        if (jwtUser != null)
        {
            var regUser = await _db.Users
                .Include(x => x.Plan)
                .FirstOrDefaultAsync(x => x.DeletedAt == null && x.Guid == jwtUser.Guid, ct);

            if (regUser!.FileUploader == null)
            {
                regUser!.FileUploader = new ImageUploaderEntity()
                {
                    RatesPlan = await _db.UploadRatesPlans.FirstAsync(x => x.Name == RatesPlanTypes.RegUserPlan, ct),
                };
                await _db.SaveChangesAsync(CancellationToken.None);
            }

            uploader = regUser.FileUploader;
        }
        else
        {
            var anonUser = await _db.AnonUsers
                .Include(x => x.FileUploader)
                .ThenInclude(x => x!.RatesPlan)
                .FirstOrDefaultAsync(x => x.SourceIp == HttpContext.Connection.RemoteIpAddress!.ToString(), ct);
            if (anonUser == null)
            {
                anonUser = new AnonymousUserEntity()
                {
                    SourceIp = HttpContext.Connection.RemoteIpAddress!.ToString(),
                };
                _db.AnonUsers.Add(anonUser);
                await _db.SaveChangesAsync(CancellationToken.None);
            }

            if (anonUser!.FileUploader == null)
            {
                anonUser!.FileUploader = new ImageUploaderEntity()
                {
                    RatesPlan = await _db.UploadRatesPlans.FirstAsync(x => x.Name == RatesPlanTypes.AnonUserPlan, ct),
                };
                await _db.SaveChangesAsync(CancellationToken.None);
            }

            uploader = anonUser.FileUploader;
        }

        var uploadedLastHour = await _db.Images
            .Where(x => x.DeletedAt == null &&
                        x.UploaderId == uploader.Id &&
                        x.CreatedAt > DateTimeOffset.Now.AddHours(-1))
            .CountAsync(ct);

        return new GetRatesResponse()
        {
            CurrentPlan = _mapper.Map<UserPlanModel>(uploader.RatesPlan),
            Spend = new UserPlanSpendModel()
            {
                ImagesPerHour = uploadedLastHour
            }
        };
    }
}