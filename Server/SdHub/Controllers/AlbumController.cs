﻿using System;
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
using SdHub.Database.Extensions;
using SdHub.Extensions;
using SdHub.Models;
using SdHub.Models.Album;
using SdHub.Services.User;
using SdHub.Shared.AspErrorHandling.ModelState;
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
    public async Task<PaginationResponse<AlbumModel>> Search([FromBody] SearchAlbumRequest req, CancellationToken ct = default)
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
            .ApplyFilter();

        query = IncludeFirstImageOrGrid(query);

        var total = await query.CountAsync(ct);
        var albums = await query.Skip(req.Skip).Take(req.Take).ToArrayAsync(ct);
        foreach (var albumEntity in albums)
        {
            if (!(albumEntity.AlbumImages?.Count > 0))
                continue;
            albumEntity.ThumbImage ??= albumEntity.AlbumImages[0].Image?.CompressedImage;
            albumEntity.ThumbImage ??= albumEntity.AlbumImages[0].Grid?.ThumbImage;
        }

        var albumModels = _mapper.Map<AlbumModel[]>(albums);

        return new PaginationResponse<AlbumModel>()
        {
            Values = albumModels,
            Total = total,
            Skip = req.Skip,
            Take = req.Take,
        };
    }

    [HttpGet]
    [Route("[action]")]
    [AllowAnonymous]
    public async Task<GetAlbumResponse> Get([FromQuery] GetAlbumRequest req, CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();
        var query = _db.Albums
            .Include(x => x.Owner)
            .Where(x => x.DeletedAt == null && x.ShortToken == req.ShortToken);

        query = IncludeFirstImageOrGrid(query);

        var album = await query.FirstOrDefaultAsync(ct);
        if (album == null)
            ModelState.AddError(ModelStateErrors.AlbumNotFound).ThrowIfNotValid();

        if (album!.AlbumImages?.Count > 0)
        {
            album.ThumbImage ??= album.AlbumImages[0].Image?.CompressedImage;
            album.ThumbImage ??= album.AlbumImages[0].Grid?.ThumbImage;
        }

        var totalImgs = await _db.AlbumImages.Where(x => x.AlbumId == album!.Id
                                                         && (x.Grid != null && x.Grid.DeletedAt == null ||
                                                             x.Image != null && x.Image.DeletedAt == null))
            .CountAsync(ct);

        return new GetAlbumResponse()
        {
            Album = _mapper.Map<AlbumModel>(album),
            ImagesCount = totalImgs
        };
    }

    private IQueryable<AlbumEntity> IncludeFirstImageOrGrid(IQueryable<AlbumEntity> query)
    {
        query = query.Include(x =>
            x.AlbumImages!.Where(y =>
                y.Grid != null && y.Grid.DeletedAt == null || y.Image != null && y.Image.DeletedAt == null).Take(1));
        query = query.Include(x => x.AlbumImages!)
            .ThenInclude(x => x.Image)
            .ThenInclude(x => x!.CompressedImage);
        query = query.Include(x => x.AlbumImages!)
            .ThenInclude(x => x.Grid)
            .ThenInclude(x => x!.ThumbImage);
        return query;
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

        album!.DeletedAt = DateTimeOffset.UtcNow;
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
        var userJwt = _fromTokenService.Get()!;

        var album = await _db.Albums
            .Include(x => x.Owner)
            .FirstOrDefaultAsync(x => x.ShortToken == req.AlbumShortToken, ct);
        if (album == null)
            ModelState.AddError(ModelStateErrors.AlbumNotFound).ThrowIfNotValid();
        if (album!.Owner!.Guid != userJwt.Guid)
            ModelState.AddError(ModelStateErrors.NotAlbumOwner).ThrowIfNotValid();

        var images = await _db.Images
            .Where(x =>
                x.DeletedAt == null
                && req.Images.Contains(x.ShortToken)
                && (x.AlbumImages!.Count == 0 || x.AlbumImages!.All(y => y.AlbumId != album.Id)))
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
        var userJwt = _fromTokenService.Get()!;

        var album = await _db.Albums
            .Include(x => x.Owner)
            .FirstOrDefaultAsync(x => x.ShortToken == req.AlbumShortToken, ct);
        if (album == null)
            ModelState.AddError(ModelStateErrors.AlbumNotFound).ThrowIfNotValid();
        if (album!.Owner!.Guid != userJwt.Guid)
            ModelState.AddError(ModelStateErrors.NotAlbumOwner).ThrowIfNotValid();

        var albImages = await _db.AlbumImages
            .Include(x => x.Image)
            .Where(x =>
                x.Image!.DeletedAt == null
                && req.Images.Contains(x.Image.ShortToken)
                && x.AlbumId == album.Id)
            .ToArrayAsync(ct);


        _db.AlbumImages.RemoveRange(albImages);

        await _db.SaveChangesAsync(CancellationToken.None);
        return new DeleteAlbumImagesResponse
        {
            DeletedImages = albImages.Select(x => x.Image!.ShortToken!).ToArray()
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