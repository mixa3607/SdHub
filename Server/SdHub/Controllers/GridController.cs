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
using SdHub.Database.Entities.Grids;
using SdHub.Database.Extensions;
using SdHub.Extensions;
using SdHub.Models;
using SdHub.Models.Grid;
using SdHub.Services.User;
using SdHub.Shared.AspErrorHandling.ModelState;

namespace SdHub.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class GridController : ControllerBase
{
    private readonly SdHubDbContext _db;
    private readonly IMapper _mapper;
    private readonly IUserFromTokenService _fromTokenService;

    public GridController(SdHubDbContext db, IMapper mapper, IUserFromTokenService fromTokenService)
    {
        _db = db;
        _mapper = mapper;
        _fromTokenService = fromTokenService;
    }

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<PaginationResponse<GridModel>> Search([FromBody] SearchGridRequest req, CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();

        var emptyQuery = _db.Grids.AsQueryable();
        var query = emptyQuery;

        if (!string.IsNullOrWhiteSpace(req.SearchText) && req.Fields.Count > 0)
        {
            var searchText = req.SearchText;
            if (!searchText.StartsWith('%'))
                searchText = "%" + searchText;
            if (!searchText.EndsWith('%'))
                searchText += "%";

            var predicate = PredicateBuilder.New<GridEntity>();
            foreach (var fieldType in req.Fields)
            {
                if (fieldType == SearchGridInFieldType.Name)
                {
                    predicate = predicate.Or(x => EF.Functions.ILike(x.Name!, searchText));
                }
                else if (fieldType == SearchGridInFieldType.Description)
                {
                    predicate = predicate.Or(x => EF.Functions.ILike(x.Description!, searchText));
                }
                else if (fieldType == SearchGridInFieldType.User)
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

        if (!string.IsNullOrWhiteSpace(req.Album))
        {
            query = query.Where(x => x.AlbumImages!.Any(y => y.Album!.ShortToken == req.Album));
        }


        if (req.OrderByField == SearchGridOrderByFieldType.UploadDate)
        {
            if (req.OrderBy == SearchGridOrderByType.Asc)
                query = query.OrderByDescending(x => x.CreatedAt);
            else if (req.OrderBy == SearchGridOrderByType.Desc)
                query = query.OrderBy(x => x.CreatedAt);
        }
        else if (req.OrderByField == SearchGridOrderByFieldType.UserName)
        {
            if (req.OrderBy == SearchGridOrderByType.Asc)
                query = query.OrderBy(x => x.Owner!.Login);
            else if (req.OrderBy == SearchGridOrderByType.Desc)
                query = query.OrderByDescending(x => x.Owner!.Login);
        }

        query = query
            .Include(x => x.ThumbImage)
            .Include(x => x.Owner);

        query = query.ApplyFilter();

        var total = await query.CountAsync(ct);
        var grids = await query.Skip(req.Skip).Take(req.Take).ToArrayAsync(ct);
        var gridModels = _mapper.Map<GridModel[]>(grids);


        return new PaginationResponse<GridModel>()
        {
            Values = gridModels,
            Total = total,
            Skip = req.Skip,
            Take = req.Take,
        };
    }

    [HttpGet("[action]")]
    [AllowAnonymous]
    public async Task<GetGridResponse> Get([FromQuery] GetGridRequest req, CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();

        var grid = await IncludeImages(_db.Grids)
            .Include(x => x.LayersDirectory)
            .Include(x => x.ThumbImage)
            .Include(x => x.Owner)
            .ApplyFilter(shortCode: req.ShortToken!.Trim())
            .FirstOrDefaultAsync(ct);
        if (grid == null)
            ModelState.AddError(ModelStateErrors.GridNotFound).ThrowIfNotValid();

        return new GetGridResponse()
        {
            Grid = _mapper.Map<GridModel>(grid)
        };
    }

    [HttpPost("[action]")]
    public async Task<DeleteGridResponse> Delete([FromBody] DeleteGridRequest req, CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();
        var userJwt = _fromTokenService.Get()!;

        var grid = await _db.Grids
            .Include(x => x.GridImages!).ThenInclude(x => x.Image)
            .Include(x => x.Owner)
            .ApplyFilter(shortCode: req.ShortToken!.Trim())
            .FirstOrDefaultAsync(ct);

        if (grid == null)
            ModelState.AddError(ModelStateErrors.GridNotFound).ThrowIfNotValid();
        if (grid!.Owner!.Guid != userJwt.Guid)
            ModelState.AddError(ModelStateErrors.NotGridOwner).ThrowIfNotValid();

        var now = DateTimeOffset.UtcNow;
        grid.DeletedAt = now;
        grid.GridImages!.ForEach(x => x.Image!.DeletedAt = now);
        await _db.SaveChangesAsync(CancellationToken.None);

        return new DeleteGridResponse()
        {
            Success = true
        };
    }

    [HttpPost("[action]")]
    public async Task<EditGridResponse> Edit([FromBody] EditGridRequest req, CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();
        var userJwt = _fromTokenService.Get()!;

        var grid = await IncludeImages(_db.Grids)
            .Include(x => x.LayersDirectory)
            .Include(x => x.ThumbImage)
            .Include(x => x.Owner)
            .ApplyFilter(shortCode: req.ShortToken!.Trim())
            .FirstOrDefaultAsync(ct);

        if (grid == null)
            ModelState.AddError(ModelStateErrors.GridNotFound).ThrowIfNotValid();
        if (grid!.Owner!.Guid != userJwt.Guid)
            ModelState.AddError(ModelStateErrors.NotGridOwner).ThrowIfNotValid();

        if (req.Description != null)
            grid!.Description = req.Description;
        if (req.Name != null)
            grid!.Name = req.Name;

        await _db.SaveChangesAsync(CancellationToken.None);
        return new EditGridResponse()
        {
            Success = true,
            Grid = _mapper.Map<GridModel>(grid)
        };
    }

    private IQueryable<GridEntity> IncludeImages(IQueryable<GridEntity> query)
    {
        query = query
            .Include(x => x.GridImages!.OrderBy(y => y.Order));
        query = query
            .Include(x => x.GridImages!)
            .ThenInclude(x => x.Image!)
            .ThenInclude(x => x.Owner);
        query = query
            .Include(x => x.GridImages!)
            .ThenInclude(x => x.Image!)
            .ThenInclude(x => x.CompressedImage);
        query = query
            .Include(x => x.GridImages!)
            .ThenInclude(x => x.Image!)
            .ThenInclude(x => x.ThumbImage);
        query = query
            .Include(x => x.GridImages!)
            .ThenInclude(x => x.Image!)
            .ThenInclude(x => x.OriginalImage);
        query = query
            .Include(x => x.GridImages!)
            .ThenInclude(x => x.Image!)
            .ThenInclude(x => x.ParsedMetadata!)
            .ThenInclude(x => x.Tags);
        return query;
    }
}