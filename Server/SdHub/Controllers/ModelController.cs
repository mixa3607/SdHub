using System.Collections.Generic;
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
using SdHub.Database.Entities.Files;
using SdHub.Extensions;
using SdHub.Hangfire.Jobs;
using SdHub.Models;
using SdHub.Models.Album;
using SdHub.Models.Bins;
using SdHub.Services.FileProc;
using SdHub.Shared.AspErrorHandling.ModelState;

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
                        x!.Files!.Any(z => EF.Functions.ILike(z.File!.Hash!, searchText)));
                }
                else if (fieldType == SearchModelInFieldType.V1Hash)
                {
                    predicate = predicate.Or(x =>
                        x.Files!.Any(z => EF.Functions.ILike(z.ModelHashV1!, searchText)));
                }
                else if (fieldType == SearchModelInFieldType.V2Hash)
                {
                    predicate = predicate.Or(x =>
                        x.Files!.Any(z => EF.Functions.ILike(z.ModelHashV2!, searchText)));
                }
            }

            query = query.Where(predicate);
        }

        var total = await query.CountAsync(ct);
        var entities = await query.Skip(req.Skip).Take(req.Take).ToArrayAsync(ct);

        var models = _mapper.Map<ModelModel[]>(entities);

        return new PaginationResponse<ModelModel>()
        {
            Values = models,
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
        var query = _db.Models
            .Include(x => x.Files!).ThenInclude(x => x.File!)
            .Where(x => x.Id == req.Id);

        var entity = await query.FirstOrDefaultAsync(ct);
        if (entity == null)
            ModelState.AddError(ModelStateErrors.ModelNotFound).ThrowIfNotValid();
        
        return _mapper.Map<ModelModel>(entity);
    }

    [HttpPost]
    [Route("[action]")]
    [Authorize(Roles = UserRoleTypes.Admin)]
    public async Task<ModelModel> Create([FromBody] CreateModelRequest req, CancellationToken ct = default)
    {
        var entity = new ModelEntity()
        {
            Name = req.Name,
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
            .Where(x => x.Id == req.Id)
            .FirstOrDefaultAsync(ct);

        if (entity == null)
            ModelState.AddError(ModelStateErrors.ModelNotFound).ThrowIfNotValid();

        if (req!.Name != null)
            entity!.Name = req.Name;
        if (req.About != null)
            entity!.About = req.About;

        await _db.SaveChangesAsync(CancellationToken.None);
        return _mapper.Map<ModelModel>(entity);
    }
}