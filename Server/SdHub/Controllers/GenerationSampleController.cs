using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SdHub.Constants;
using SdHub.Database;
using SdHub.Database.Entities.Bins;
using SdHub.Extensions;
using SdHub.Models;
using SdHub.Models.Samples;
using SdHub.Services.User;
using SdHub.Shared.AspErrorHandling.ModelState;

namespace SdHub.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class GenerationSampleController : ControllerBase
{
    private readonly SdHubDbContext _db;
    private readonly IMapper _mapper;
    private readonly IUserFromTokenService _fromTokenService;

    public GenerationSampleController(SdHubDbContext db, IMapper mapper, IUserFromTokenService fromTokenService)
    {
        _db = db;
        _mapper = mapper;
        _fromTokenService = fromTokenService;
    }

    [HttpPost]
    [Route("[action]")]
    [AllowAnonymous]
    public async Task<PaginationResponse<GenerationSampleModel>> Search([FromBody] SearchSampleRequest req,
        CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();

        var emptyQuery = _db.GenerationSamples.AsQueryable();
        var query = emptyQuery;
        if (req.ModelId != default)
            query = query.Where(x => x.ModelVersion!.ModelId == req.ModelId);
        if (req.VaeId != default)
            query = query.Where(x => x.VaeVersion!.VaeId == req.VaeId);
        if (req.EmbeddingId != default)
            query = query.Where(x => x.EmbeddingVersion!.EmbeddingId == req.EmbeddingId);
        if (req.HypernetId != default)
            query = query.Where(x => x.HypernetVersion!.HypernetId == req.HypernetId);

        if (req.ModelVersionId != default)
            query = query.Where(x => x.ModelVersionId == req.ModelVersionId);
        if (req.VaeVersionId != default)
            query = query.Where(x => x.VaeVersionId == req.VaeVersionId);
        if (req.EmbeddingVersionId != default)
            query = query.Where(x => x.EmbeddingVersionId == req.EmbeddingVersionId);
        if (req.HypernetVersionId != default)
            query = query.Where(x => x.HypernetVersionId == req.HypernetVersionId);

        var take = 100;
        query = IncludeEntities(query);
        var entities = await query.Take(take).OrderBy(x => x.Id).ToArrayAsync(ct);
        var models = _mapper.Map<GenerationSampleModel[]>(entities);

        return new PaginationResponse<GenerationSampleModel>()
        {
            Items = models,
            Total = models.Length,
            Skip = 0,
            Take = take,
        };
    }

    [HttpPost]
    [Route("[action]")]
    [Authorize(Roles = UserRoleTypes.Admin)]
    public async Task<GenerationSampleModel> Add([FromBody] CreateGenerationSampleRequest req,
        CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();
        var image = await _db.Images.FirstOrDefaultAsync(x => x.ShortToken == req.ImageShortToken, ct);
        if (image == null)
            ModelState.AddError(ModelStateErrors.ImageNotFound).ThrowIfNotValid();

        var entity = new GenerationSampleEntity()
        {
            EmbeddingVersionId = req.EmbeddingVersionId,
            HypernetVersionId = req.HypernetVersionId,
            ModelVersionId = req.ModelVersionId,
            VaeVersionId = req.VaeVersionId,
            ImageId = image!.Id
        };
        _db.GenerationSamples.Add(entity);
        await _db.SaveChangesAsync(CancellationToken.None);

        entity = await IncludeEntities(_db.GenerationSamples.Where(x => x.Id == entity.Id)).FirstAsync(ct);
        return _mapper.Map<GenerationSampleModel>(entity);
    }

    [HttpPost]
    [Route("[action]")]
    [Authorize(Roles = UserRoleTypes.Admin)]
    public async Task Delete([FromBody] DeleteGenerationSampleRequest req, CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();
        var entity = await _db.GenerationSamples
            .FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (entity == null)
            ModelState.AddError("GS not found").ThrowIfNotValid();

        _db.GenerationSamples.Remove(entity!);
        await _db.SaveChangesAsync(CancellationToken.None);
    }

    private IQueryable<GenerationSampleEntity> IncludeEntities(IQueryable<GenerationSampleEntity> query)
    {
        return query
                .Include(x => x.ModelVersion!).ThenInclude(x => x.Model)
                .Include(x => x.VaeVersion!).ThenInclude(x => x.Vae)
                .Include(x => x.EmbeddingVersion!).ThenInclude(x => x.Embedding)
                .Include(x => x.HypernetVersion!).ThenInclude(x => x.Hypernet)
            ;
    }
}