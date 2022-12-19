using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Hangfire;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SdHub.Constants;
using SdHub.Database;
using SdHub.Database.Entities.Bins;
using SdHub.Extensions;
using SdHub.Hangfire.Jobs;
using SdHub.Models;
using SdHub.Models.Album;
using SdHub.Models.Bins;
using SdHub.Services.FileProc;

namespace SdHub.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ModelController : ControllerBase
{
    private readonly SdHubDbContext _db;
    private readonly IMapper _mapper;
    private readonly IFileProcessor _fileProcessor;

    public ModelController(SdHubDbContext db, IMapper mapper, IFileProcessor fileProcessor)
    {
        _db = db;
        _mapper = mapper;
        _fileProcessor = fileProcessor;
    }

    [HttpPost]
    [Route("[action]")]
    [AllowAnonymous]
    public async Task<PaginationResponse<ModelModel>> Search([FromBody] SearchModelRequest req,
        CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();

        var emptyQuery = _db.Models.AsQueryable();
        var query = emptyQuery;
        if (!string.IsNullOrWhiteSpace(req.SearchText) && req.Fields.Count > 0)
        {
            var searchText = req.SearchText;
            if (!searchText.StartsWith('%'))
                searchText = "%" + searchText;
            if (!searchText.EndsWith('%'))
                searchText += "%";

            var predicate = PredicateBuilder.New<ModelEntity>();
            foreach (var fieldType in req.Fields.Distinct())
            {
                if (fieldType == SearchModelInFieldType.Name)
                {
                    predicate = predicate.Or(x => EF.Functions.ILike(x.Name!, searchText));
                }
                else if (fieldType == SearchModelInFieldType.FullHash)
                {
                    predicate = predicate.Or(x =>
                        x.Versions!.Any(y => EF.Functions.ILike(y.CkptFile!.Hash!, searchText)));
                }
                else if (fieldType == SearchModelInFieldType.V1Hash)
                {
                    predicate = predicate.Or(x =>
                        x.Versions!.Any(y => EF.Functions.ILike(y.HashV1!, searchText)));
                }
            }

            query = query.Where(predicate);
        }

        if (req.SdVersions.Count > 0)
        {
            var vers = req.SdVersions.Distinct().ToArray();
            query = query.Where(x => vers.Contains(x.SdVersion));
        }

        query = query.Include(x => x.ModelTags!)
            .ThenInclude(x => x.Tag!);

        var total = await query.CountAsync(ct);
        var entities = await query.Skip(req.Skip).Take(req.Take).ToArrayAsync(ct);

        var models = _mapper.Map<ModelModel[]>(entities);

        return new PaginationResponse<ModelModel>()
        {
            Items = models,
            Total = total,
            Skip = req.Skip,
            Take = req.Take,
        };
    }

    [HttpGet]
    [Route("[action]")]
    [AllowAnonymous]
    public async Task<ModelModel> Get([FromQuery] GetModelRequest req, CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();
        var query = _db.Models
            .Include(x => x.ModelTags!)
            .ThenInclude(x => x.Tag!)
            .Include(x => x.Versions!)
            .ThenInclude(x => x.CkptFile!)
            .Where(x => x.Id == req.Id);

        var entity = await query.FirstOrDefaultAsync(ct);
        if (entity == null)
            ModelState.AddError(ModelStateErrors.ModelNotFound).ThrowIfNotValid();

        entity!.Versions!.ForEach(x => x.Model = null);
        return _mapper.Map<ModelModel>(entity);
    }

    [HttpPost]
    [Route("[action]")]
    [Authorize(Roles = UserRoleTypes.Admin)]
    public async Task<ModelModel> Create([FromBody] CreateModelRequest req, CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();

        var entity = new ModelEntity()
        {
            Name = req.Name,
            SdVersion = SdVersion.Unknown,
        };
        _db.Models.Add(entity);
        await _db.SaveChangesAsync(CancellationToken.None);
        return _mapper.Map<ModelModel>(entity);
    }

    [HttpPost]
    [Route("[action]")]
    [Authorize(Roles = UserRoleTypes.Admin)]
    public async Task<DeleteModelResponse> Delete([FromBody] DeleteModelRequest req, CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();

        var entity = await _db.Models
            .Where(x => x.Id == req.Id)
            .FirstOrDefaultAsync(ct);
        if (entity == null)
            ModelState.AddError(ModelStateErrors.ModelNotFound).ThrowIfNotValid();

        _db.Models.Remove(entity!);
        await _db.SaveChangesAsync(CancellationToken.None);
        return new DeleteModelResponse()
        {
        };
    }

    [HttpPost]
    [Route("[action]")]
    [Authorize(Roles = UserRoleTypes.Admin)]
    public async Task<ModelModel> Edit([FromBody] EditModelRequest req, CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();

        var entity = await _db.Models
            .Include(x => x.ModelTags!)
            .ThenInclude(x => x.Tag!)
            .Where(x => x.Id == req.Id)
            .FirstOrDefaultAsync(ct);

        if (entity == null)
            ModelState.AddError(ModelStateErrors.ModelNotFound).ThrowIfNotValid();

        if (req!.Name != null)
            entity!.Name = req.Name;
        if (req.About != null)
            entity!.About = req.About;
        if (req.SdVersion != null)
            entity!.SdVersion = req.SdVersion.Value;

        await _db.SaveChangesAsync(CancellationToken.None);
        return _mapper.Map<ModelModel>(entity);
    }

    [HttpPost]
    [Route("[action]")]
    [Authorize(Roles = UserRoleTypes.Admin)]
    public async Task<ModelVersionModel> AddVersion([FromBody] AddModelVersionRequest req,
        CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();

        if (!await _db.Models.AnyAsync(x => x.Id == req.ModelId, ct))
            ModelState.AddError(ModelStateErrors.ModelNotFound).ThrowIfNotValid();

        var entity = new ModelVersionEntity()
        {
            ModelId = req.ModelId,
        };

        _db.ModelVersions.Add(entity);
        await _db.SaveChangesAsync(CancellationToken.None);
        return _mapper.Map<ModelVersionModel>(entity);
    }

    [HttpPost]
    [Route("[action]")]
    [Authorize(Roles = UserRoleTypes.Admin)]
    public async Task<ModelVersionModel> EditVersion([FromBody] EditModelVersionRequest req,
        CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();

        if (!await _db.Models.AnyAsync(x => x.Id == req.ModelId, ct))
            ModelState.AddError(ModelStateErrors.ModelNotFound).ThrowIfNotValid();

        var entity = await _db.ModelVersions
            .Include(x => x.CkptFile)
            .FirstOrDefaultAsync(x => x.ModelId == req.ModelId && x.Id == req.VersionId, ct);
        if (entity == null)
            ModelState.AddError(ModelStateErrors.ModelVersionNotFound).ThrowIfNotValid();

        if (req!.Version != null)
            entity!.Version = req.Version;
        if (req.About != null)
            entity!.About = req.About;
        if (req!.KnownNames != null)
            entity!.KnownNames = req.KnownNames;
        var requireReCalc = false;
        if (req.CkptFile != null)
        {
            var decR = await _fileProcessor.DecomposeUrlAsync(req.CkptFile, ct);
            var file = await _db.Files.FirstOrDefaultAsync(
                x => x.StorageName == decR.Storage.Name && x.PathOnStorage == decR.PathOnStorage, ct);
            if (file == null)
                ModelState.AddError(ModelStateErrors.FileNotFound).ThrowIfNotValid();
            entity!.CkptFile = file;
            entity.HashV1 = null;
            requireReCalc = true;
        }

        await _db.SaveChangesAsync(CancellationToken.None);
        if (requireReCalc)
        {
            BackgroundJob.Enqueue<IBinUpdaterRunnerV1>(x =>
                x.UpdateModelVersionFilesAsync(req.ModelId, false, CancellationToken.None));
        }

        return _mapper.Map<ModelVersionModel>(entity);
    }

    [HttpPost]
    [Route("[action]")]
    [Authorize(Roles = UserRoleTypes.Admin)]
    public async Task DeleteVersion([FromBody] DeleteModelVersionRequest req,
        CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();

        if (!await _db.Models.AnyAsync(x => x.Id == req.ModelId, ct))
            ModelState.AddError(ModelStateErrors.ModelNotFound).ThrowIfNotValid();

        var entity = await _db.ModelVersions
            .FirstOrDefaultAsync(x => x.ModelId == req.ModelId && x.Id == req.VersionId, ct);
        if (entity == null)
            ModelState.AddError(ModelStateErrors.ModelVersionNotFound).ThrowIfNotValid();

        _db.ModelVersions.Remove(entity!);
        await _db.SaveChangesAsync(CancellationToken.None);
    }
}