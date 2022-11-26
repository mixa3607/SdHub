using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using SdHub.Constants;
using SdHub.Database;
using SdHub.Database.Entities.Images;
using SdHub.Extensions;
using SdHub.Models;
using SdHub.Models.Image;
using SdHub.Services.User;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

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

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<SearchImageResponse> Search([FromBody] SearchImageRequest req, CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();

        var emptyQuery = _db.Images.AsQueryable();
        var query = emptyQuery;

        if (!string.IsNullOrWhiteSpace(req.SearchText) && req.Fields.Count > 0)
        {
            //var searchText = req.SearchText.ToLower();
            //var byName = req.Fields.Contains(SearchImageInFieldType.Name);
            //var byDesc = req.Fields.Contains(SearchImageInFieldType.Description);
            //var byPrompt = req.Fields.Contains(SearchImageInFieldType.Prompt);
            //var byUser = req.Fields.Contains(SearchImageInFieldType.User);
            //query = query.Where(x =>
            //    (byName && EF.Functions.ILike(x.Name!, searchText)) ||
            //    (byDesc && EF.Functions.ILike(x.Description!, searchText)) ||
            //    (byUser && EF.Functions.ILike(x.Owner!.Login!, searchText)) ||
            //    (byPrompt && x.ParsedMetadata!.Tags!.Any(y => EF.Functions.ILike(y.Value!, searchText))) ||
            //    false
            //);
            var searchText = req.SearchText;
            if (!searchText.StartsWith('%') && !req.SearchAsRegexp)
                searchText = "%" + searchText;
            if (!searchText.EndsWith('%') && !req.SearchAsRegexp)
                searchText += "%";

            var predicate = PredicateBuilder.New<ImageEntity>();
            foreach (var fieldType in req.Fields)
            {
                if (fieldType == SearchImageInFieldType.Name)
                {
                    predicate = req.SearchAsRegexp
                        ? predicate.Or(x => Regex.IsMatch(x.Name!, searchText,
                            RegexOptions.Singleline | RegexOptions.IgnoreCase))
                        : predicate.Or(x => EF.Functions.ILike(x.Name!, searchText));
                }
                else if (fieldType == SearchImageInFieldType.Description)
                {
                    predicate = req.SearchAsRegexp
                        ? predicate.Or(x => Regex.IsMatch(x.Description!, searchText,
                            RegexOptions.Singleline | RegexOptions.IgnoreCase))
                        : predicate.Or(x => EF.Functions.ILike(x.Description!, searchText));
                }
                else if (fieldType == SearchImageInFieldType.User)
                {
                    predicate = req.SearchAsRegexp
                        ? predicate.Or(x => Regex.IsMatch(x.Owner!.Login!, searchText,
                            RegexOptions.Singleline | RegexOptions.IgnoreCase))
                        : predicate.Or(x => EF.Functions.ILike(x.Owner!.Login!, searchText));
                }
                else if (fieldType == SearchImageInFieldType.Prompt)
                {
                    predicate = req.SearchAsRegexp
                        ? predicate.Or(x => x.ParsedMetadata!.Tags!.Any(y => Regex.IsMatch(y.Value!, searchText,
                            RegexOptions.Singleline | RegexOptions.IgnoreCase)))
                        : predicate.Or(x => x.ParsedMetadata!.Tags!.Any(y => EF.Functions.ILike(y.Value!, searchText)));
                }
            }

            query = query.Where(predicate);
        }

        var str = query.ToQueryString();

        if (req.Softwares.Count > 0)
        {
            var sw = req.Softwares.Where(x => x != SoftwareGeneratedTypes.Unknown).ToArray();
            var unknownSoft = sw.Length != req.Softwares.Count;

            var predicate = PredicateBuilder.New<ImageEntity>();

            if (unknownSoft)
            {
                predicate = predicate.Or(x => !x.ParsedMetadata!.Tags!.Any());
            }

            predicate.Or(x => x.ParsedMetadata!.Tags!.Any(y => sw.Contains(y.Software)));

            query = query.Where(predicate);
        }

        if (req.OnlyFromRegisteredUsers)
        {
            query = query.Where(x => !x.Owner!.IsAnonymous);
        }

        if (req.OrderByField == SearchImageOrderByFieldType.UploadDate)
        {
            if (req.OrderBy == SearchImageOrderByType.Asc)
                query = query.OrderByDescending(x => x.CreatedAt);
            else if (req.OrderBy == SearchImageOrderByType.Desc)
                query = query.OrderBy(x => x.CreatedAt);
        }
        else if (req.OrderByField == SearchImageOrderByFieldType.UserName)
        {
            if (req.OrderBy == SearchImageOrderByType.Asc)
                query = query.OrderBy(x => x.Owner!.Login);
            else if (req.OrderBy == SearchImageOrderByType.Desc)
                query = query.OrderByDescending(x => x.Owner!.Login);
        }

        query = query
            .Include(x => x.ParsedMetadata).ThenInclude(x => x!.Tags)
            .Include(x => x.OriginalImage)
            .Include(x => x.CompressedImage)
            .Include(x => x.ThumbImage)
            .Include(x => x.Owner)
            .Where(x => x.DeletedAt == null);
        var total = await query.CountAsync(ct);
        var images = await query.Skip(req.Skip).Take(req.Take).ToArrayAsync(ct);
        var imageModels = _mapper.Map<ImageModel[]>(images);


        return new SearchImageResponse()
        {
            Images = imageModels,
            Total = total,
        };
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
            ModelState.AddError(ModelStateErrors.ImageNotFound).ThrowIfNotValid();
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
            ModelState.AddError(ModelStateErrors.ImageNotFound).ThrowIfNotValid();

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
            .Include(x => x.OriginalImage)
            .Include(x => x.CompressedImage)
            .Include(x => x.ThumbImage)
            .Include(x => x.Owner)
            .Where(x => x.DeletedAt == null && x.ShortToken == req.ShortToken)
            .FirstOrDefaultAsync(ct);
        if (imageEnt == null)
            ModelState.AddError(ModelStateErrors.ImageNotFound).ThrowIfNotValid();

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
            ModelState.AddError(ModelStateErrors.ImageNotFound).ThrowIfNotValid();

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
            .Include(x => x.OriginalImage)
            .Include(x => x.CompressedImage)
            .Include(x => x.ThumbImage)
            .Include(x => x.Owner)
            .Where(x => x.DeletedAt == null && x.ShortToken == req.Image!.ShortToken)
            .FirstOrDefaultAsync(ct);

        if (imageEnt == null)
            ModelState.AddError(ModelStateErrors.ImageNotFound).ThrowIfNotValid();

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