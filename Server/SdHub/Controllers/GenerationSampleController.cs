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
using SdHub.Models;
using SdHub.Models.Samples;
using SdHub.Shared.AspErrorHandling.ModelState;

namespace SdHub.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class GenerationSampleController : ControllerBase
{
    private readonly SdHubDbContext _db;
    private readonly IMapper _mapper;

    public GenerationSampleController(SdHubDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    [HttpPost]
    [Route("[action]")]
    [AllowAnonymous]
    public async Task<PaginationResponse<GenerationSampleModel>> Search([FromBody] SearchSampleRequest req,
        CancellationToken ct = default)
    {
        var emptyQuery = _db.GenerationSamples.AsQueryable();
        var query = emptyQuery;
        if (req.ModelId != default)
            query = query.Where(x => x.Model!.Id == req.ModelId);
        if (req.VaeId != default)
            query = query.Where(x => x.Vae!.Id == req.VaeId);
        if (req.EmbeddingId != default)
            query = query.Where(x => x.Embedding!.Id == req.EmbeddingId);
        if (req.HypernetId != default)
            query = query.Where(x => x.Hypernet!.Id == req.HypernetId);

        var take = 100;
        query = IncludeEntities(query);
        var entities = await query.Take(take).OrderBy(x => x.Id).ToArrayAsync(ct);
        var models = _mapper.Map<GenerationSampleModel[]>(entities);

        return new PaginationResponse<GenerationSampleModel>()
        {
            Values = models,
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
        var image = await _db.Images.FirstOrDefaultAsync(x => x.ShortToken == req.ImageShortToken, ct);
        if (image == null)
            ModelState.AddError(ModelStateErrors.ImageNotFound).ThrowIfNotValid();

        var entity = new GenerationSampleEntity()
        {
            EmbeddingId = req.EmbeddingId,
            HypernetId = req.HypernetId,
            ModelId = req.ModelId,
            VaeId = req.VaeId,
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
                .Include(x => x.Model!)
                .Include(x => x.Vae)
                .Include(x => x.Embedding)
                .Include(x => x.Hypernet)
            ;
    }
}