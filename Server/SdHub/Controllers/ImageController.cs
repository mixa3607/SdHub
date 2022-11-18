using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using SdHub.Database;
using SdHub.Database.Entities.Images;
using SdHub.Extensions;
using SdHub.Models;
using SdHub.Models.Image;
using SdHub.Services.User;

namespace SdHub.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ImageController : ControllerBase
{
    private readonly SdHubDbContext _db;
    private readonly IMapper _mapper;
    private readonly IUserFromTokenService _fromTokenService;

    public ImageController(SdHubDbContext db, IMapper mapper, IUserFromTokenService fromTokenService)
    {
        _db = db;
        _mapper = mapper;
        _fromTokenService = fromTokenService;
    }
    
    [HttpGet("[action]")]
    [AllowAnonymous]
    public async Task<CanEditResponse> CanEdit([FromQuery] CanEditRequest req, CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();

        var userJwt = _fromTokenService.Get();

        var imageEnt = await _db.Images
            .Include(x => x.Owner)
            .Where(x => x.DeletedAt == null && x.ShortToken == req.ShortToken)
            .FirstOrDefaultAsync(ct);
        if (imageEnt == null)
            ModelState.AddError("Image not found").ThrowIfNotValid();
        var response = new CanEditResponse()
        {
            ShortToken = req.ShortToken,
        };

        if (imageEnt!.Owner!.IsAnonymous)
        {
            response.CanEdit = true;
            response.ManageTokenRequired = true;
        }
        else if (imageEnt!.Owner!.Guid == userJwt?.Guid)
        {
            response.CanEdit = true;
            response.ManageTokenRequired = false;
        }
        else
        {
            response.CanEdit = false;
            response.ManageTokenRequired = false;
        }

        return response;
    }

    [HttpGet("[action]")]
    [AllowAnonymous]
    public async Task<CheckManageTokenResponse> CheckManageToken([FromQuery] CheckManageTokenRequest req,
        CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();

        var imageEnt = await _db.Images
            .Include(x => x.Owner)
            .Where(x => x.DeletedAt == null && x.ShortToken == req.ShortToken)
            .FirstOrDefaultAsync(ct);
        if (imageEnt == null)
            ModelState.AddError("Image not found").ThrowIfNotValid();

        return new CheckManageTokenResponse()
        {
            IsValid = imageEnt!.Owner!.IsAnonymous && imageEnt.ManageToken == req.ManageToken,
            ManageToken = req.ManageToken,
            ShortToken = req.ShortToken
        };
    }

    [HttpGet("[action]")]
    [AllowAnonymous]
    public async Task<GetImageResponse> Get([FromQuery] GetImageRequest req, CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();

        var imageEnt = await _db.Images
            .Include(x => x.ParsedMetadata).ThenInclude(x => x!.Tags)
            .Include(x => x.OriginalImage).ThenInclude(x => x!.Storage)
            .Include(x => x.ThumbImage).ThenInclude(x => x!.Storage)
            .Include(x => x.Owner)
            .Where(x => x.DeletedAt == null && x.ShortToken == req.ShortToken)
            .FirstOrDefaultAsync(ct);
        if (imageEnt == null)
            ModelState.AddError("Image not found").ThrowIfNotValid();

        return new GetImageResponse()
        {
            Image = _mapper.Map<ImageModel>(imageEnt)
        };
    }

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<DeleteImageResponse> Delete([FromBody] DeleteImageRequest req, CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();

        var userJwt = _fromTokenService.Get();

        var imageEnt = await _db.Images
            .Include(x => x.Owner)
            .Where(x => x.DeletedAt == null && x.ShortToken == req.ShortToken)
            .FirstOrDefaultAsync(ct);
        if (imageEnt == null)
            ModelState.AddError("Image not found").ThrowIfNotValid();

        if (!CanManage(imageEnt!, userJwt, req.ManageToken, ModelState))
            ModelState.Throw();

        imageEnt!.DeletedAt = DateTimeOffset.Now;
        await _db.SaveChangesAsync(CancellationToken.None);
        return new DeleteImageResponse()
        {
            Success = true
        };
    }

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<EditImageResponse> Edit([FromBody] EditImageRequest req, CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();

        var userJwt = _fromTokenService.Get();

        var imageEnt = await _db.Images
            .Include(x => x.ParsedMetadata).ThenInclude(x => x!.Tags)
            .Include(x => x.OriginalImage).ThenInclude(x => x!.Storage)
            .Include(x => x.ThumbImage).ThenInclude(x => x!.Storage)
            .Include(x => x.Owner)
            .Where(x => x.DeletedAt == null && x.ShortToken == req.Image!.ShortToken)
            .FirstOrDefaultAsync(ct);

        if (imageEnt == null)
            ModelState.AddError("Image not found").ThrowIfNotValid();

        if (!CanManage(imageEnt!, userJwt, req.ManageToken, ModelState))
            ModelState.Throw();

        if (req.Image!.Description != null)
            imageEnt!.Description = req.Image!.Description;
        if (req.Image!.Name != null)
            imageEnt!.Name = req.Image!.Name;

        await _db.SaveChangesAsync(CancellationToken.None);
        return new EditImageResponse()
        {
            Success = true,
            Image = _mapper.Map<ImageModel>(imageEnt)
        };
    }

    private bool CanManage(ImageEntity image, UserModel? user, string? manageCode,
        ModelStateDictionary? modelState = null)
    {
        if (image.Owner!.IsAnonymous && image.ManageToken != manageCode)
        {
            modelState?.AddError("Manage token is wrong for image");
            return false;
        }

        if (!image.Owner!.IsAnonymous && user == null)
        {
            modelState?.AddError("Image uploaded by registered user. Please login");
            return false;
        }

        if (!image.Owner!.IsAnonymous && user?.Guid != image.Owner.Guid)
        {
            modelState?.AddError("Image uploaded by another user. Please re login");
            return false;
        }

        return true;
    }
}