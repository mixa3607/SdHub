using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SdHub.Constants;
using SdHub.Database;
using SdHub.Database.Entities.Albums;
using SdHub.Extensions;
using SdHub.Models.Album;
using SdHub.Services.User;
using SimpleBase;

namespace SdHub.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AlbumController : ControllerBase
{
    private readonly SdHubDbContext _db;
    private readonly IMapper _mapper;
    private readonly IUserFromTokenService _fromTokenService;

    public AlbumController(SdHubDbContext db, IMapper mapper, IUserFromTokenService fromTokenService)
    {
        _db = db;
        _mapper = mapper;
        _fromTokenService = fromTokenService;
    }

    [HttpPost]
    [Route("[action]")]
    [AllowAnonymous]
    public async Task<SearchAlbumResponse> Search([FromBody] SearchAlbumRequest req, CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();

        var emptyQuery = _db.Albums.AsQueryable();
        var query = emptyQuery;
        if (!string.IsNullOrWhiteSpace(req.SearchText) && req.Fields.Count > 0)
        {
            var searchText = req.SearchText;
            if (!searchText.StartsWith('%'))
                searchText = "%" + searchText;
            if (!searchText.EndsWith('%'))
                searchText += "%";

            var predicate = PredicateBuilder.New<AlbumEntity>();
            foreach (var fieldType in req.Fields)
            {
                if (fieldType == SearchAlbumInFieldType.Name)
                {
                    predicate = predicate.Or(x => EF.Functions.ILike(x.Name!, searchText));
                }
                else if (fieldType == SearchAlbumInFieldType.Description)
                {
                    predicate = predicate.Or(x => EF.Functions.ILike(x.Description!, searchText));
                }
                else if (fieldType == SearchAlbumInFieldType.User)
                {
                    predicate = predicate.Or(x => EF.Functions.ILike(x.Owner!.Login!, searchText));
                }
            }

            query = query.Where(predicate);
        }

        if (!string.IsNullOrWhiteSpace(req.Owner))
        {
            var owner = req.Owner.Normalize().ToUpper();
            query = query.Where(x => x.Owner!.LoginNormalized! == owner);
        }

        if (req.OrderByField == SearchAlbumOrderByFieldType.UploadDate)
        {
            if (req.OrderBy == SearchAlbumOrderByType.Asc)
                query = query.OrderByDescending(x => x.CreatedAt);
            else if (req.OrderBy == SearchAlbumOrderByType.Desc)
                query = query.OrderBy(x => x.CreatedAt);
        }
        else if (req.OrderByField == SearchAlbumOrderByFieldType.UserName)
        {
            if (req.OrderBy == SearchAlbumOrderByType.Asc)
                query = query.OrderBy(x => x.Owner!.Login);
            else if (req.OrderBy == SearchAlbumOrderByType.Desc)
                query = query.OrderByDescending(x => x.Owner!.Login);
        }

        query = query
            .Include(x => x.ThumbImage)
            .Include(x => x.Owner)
            .Where(x => x.DeletedAt == null);
        var total = await query.CountAsync(ct);
        var albums = await query.Skip(req.Skip).Take(req.Take).ToArrayAsync(ct);
        var albumModels = _mapper.Map<AlbumModel[]>(albums);

        return new SearchAlbumResponse()
        {
            Albums = albumModels,
            Total = total,
        };
    }

    [HttpPost]
    [Route("[action]")]
    [AllowAnonymous]
    public async Task<GetAlbumResponse> Get([FromBody] GetAlbumRequest req, CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();
        var album = await _db.Albums
            .Include(x => x.ThumbImage)
            .Include(x => x.Owner)
            .Where(x => x.DeletedAt == null && x.ShortToken == req.ShortToken)
            .FirstOrDefaultAsync(ct);
        if (album == null)
            ModelState.AddError(ModelStateErrors.AlbumNotFound).ThrowIfNotValid();

        var totalImgs = await _db.AlbumImages.Where(x => x.AlbumId == album!.Id).CountAsync(ct);

        return new GetAlbumResponse()
        {
            Album = _mapper.Map<AlbumModel>(album),
            ImagesCount = totalImgs
        };
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<AlbumModel> Create([FromBody] CreateAlbumRequest req, CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();
        var userT = _fromTokenService.Get()!;
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Guid == userT.Guid, ct);

        var album = new AlbumEntity()
        {
            ShortToken = GenerateShortToken(),
            OwnerId = user!.Id,
            Name = req.Name,
            Description = req.Description,
        };
        _db.Albums.Add(album);
        await _db.SaveChangesAsync(CancellationToken.None);
        return _mapper.Map<AlbumModel>(album);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<DeleteAlbumResponse> Delete([FromBody] DeleteAlbumRequest req, CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();
        var userT = _fromTokenService.Get()!;

        var album = await _db.Albums
            .Where(x => x.DeletedAt == null && x.Owner!.Guid == userT.Guid && x.ShortToken == req.ShortToken)
            .FirstOrDefaultAsync(ct);
        if (album == null)
            ModelState.AddError(ModelStateErrors.AlbumNotFound).ThrowIfNotValid();

        album!.DeletedAt = DateTimeOffset.Now;
        await _db.SaveChangesAsync(CancellationToken.None);
        return new DeleteAlbumResponse()
        {
        };
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<AlbumModel> Edit([FromBody] EditAlbumRequest req, CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();
        var userJwt = _fromTokenService.Get()!;

        var album = await _db.Albums
            .Include(x => x.ThumbImage)
            .Include(x => x.Owner)
            .Where(x => x.DeletedAt == null && x.ShortToken == req.ShortToken)
            .FirstOrDefaultAsync(ct);

        if (album == null)
            ModelState.AddError(ModelStateErrors.AlbumNotFound).ThrowIfNotValid();

        if (album!.Owner!.Guid != userJwt.Guid)
            ModelState.AddError(ModelStateErrors.NotAlbumOwner).ThrowIfNotValid();


        if (req.Description != null)
            album.Description = req.Description;
        if (req!.Name != null)
            album!.Name = req.Name;

        await _db.SaveChangesAsync(CancellationToken.None);
        return _mapper.Map<AlbumModel>(album);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<AddAlbumImagesResponse> AddImages([FromBody] AddAlbumImagesRequest req,
        CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();
        var album = await _db.Albums.FirstAsync(x => x.ShortToken == req.AlbumShortToken, ct);
        var images = await _db.Images
            .Where(x =>
                x.DeletedAt == null && req.Images.Contains(x.ShortToken) &&
                x.AlbumImages!.All(y => y.AlbumId != album.Id))
            .ToArrayAsync(ct);
        foreach (var image in images)
        {
            var albumImage = new AlbumImageEntity()
            {
                AlbumId = album.Id,
                ImageId = image.Id,
            };
            _db.AlbumImages.Add(albumImage);
        }

        await _db.SaveChangesAsync(CancellationToken.None);
        return new AddAlbumImagesResponse
        {
            AddedImages = images.Select(x => x.ShortToken!).ToArray()
        };
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<DeleteAlbumImagesResponse> DeleteImages([FromBody] DeleteAlbumImagesRequest req,
        CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();
        var album = await _db.Albums.FirstAsync(x => x.ShortToken == req.AlbumShortToken, ct);
        var images = await _db.Images
            .Where(x =>
                x.DeletedAt == null && req.Images.Contains(x.ShortToken) &&
                x.AlbumImages!.All(y => y.AlbumId != album.Id))
            .ToArrayAsync(ct);

        foreach (var image in images)
        {
            var albumImage = new AlbumImageEntity()
            {
                AlbumId = album.Id,
                ImageId = image.Id,
            };
            _db.AlbumImages.Remove(albumImage);
        }

        await _db.SaveChangesAsync(CancellationToken.None);
        return new DeleteAlbumImagesResponse
        {
            DeletedImages = images.Select(x => x.ShortToken!).ToArray()
        };
    }


    private string GenerateShortToken()
    {
        var max = long.MaxValue;
        var rng = Random.Shared.NextInt64(max);
        var rngBytes = BitConverter.GetBytes(rng);
        var b58 = Base58.Bitcoin.Encode(rngBytes);
        return b58;
    }
}