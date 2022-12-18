using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Flurl.Http;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SdHub.Constants;
using SdHub.Database;
using SdHub.Database.Entities.Files;
using SdHub.Extensions;
using SdHub.Models;
using SdHub.Models.Files;
using SdHub.Services.FileProc;

namespace SdHub.Controllers;

[AllowAnonymous]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class FilesController : ControllerBase
{
    private readonly SdHubDbContext _db;
    private readonly IMapper _mapper;
    private readonly IFileProcessor _fileProcessor;

    public FilesController(SdHubDbContext db, IMapper mapper, IFileProcessor fileProcessor)
    {
        _db = db;
        _mapper = mapper;
        _fileProcessor = fileProcessor;
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost]
    [Authorize(Roles = UserRoleTypes.Admin)]
    public async Task<PaginationResponse<FileModel>> Search([FromBody] SearchFileRequest req, CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();

        var emptyQuery = _db.Files.AsQueryable();
        var query = emptyQuery;

        if (!string.IsNullOrWhiteSpace(req.SearchText))
        {
            var searchText = req.SearchText;
            if (!searchText.StartsWith('%'))
                searchText = "%" + searchText;
            if (!searchText.EndsWith('%'))
                searchText += "%";

            var predicate = PredicateBuilder.New<FileEntity>();
            predicate = predicate.Or(x => EF.Functions.ILike(x.Name!, searchText));
            predicate = predicate.Or(x => EF.Functions.ILike(x.Hash!, searchText));
            predicate = predicate.Or(x => EF.Functions.ILike(x.PathOnStorage!, searchText));

            query = query.Where(predicate);
        }

        if (!string.IsNullOrWhiteSpace(req.Storage))
        {
            query = query.Where(x => EF.Functions.ILike(x.StorageName!, req.Storage));
        }

        query = query.OrderBy(x => x.CreatedAt);
        
        var total = await query.CountAsync(ct);
        var files = await query.Skip(req.Skip).Take(req.Take).ToArrayAsync(ct);
        var fileModels = _mapper.Map<FileModel[]>(files);


        return new PaginationResponse<FileModel>()
        {
            Items = fileModels,
            Total = total,
            Skip = req.Skip,
            Take = req.Take,
        };
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost]
    [Authorize(Roles = UserRoleTypes.Admin)]
    public async Task<FileModel> Import([FromBody] ImportFileRequest req, CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();

        var decR = await _fileProcessor.DecomposeUrlAsync(req.FileUrl!, ct);
        var exist = await decR.Storage.FileExistAsync(decR.PathOnStorage, ct);
        if (exist == null)
            ModelState.AddError(ModelStateErrors.FileNotFound).ThrowIfNotValid();

        var tmpFilePath = _fileProcessor.GetNewTempFilePath();
        var netStream = await req.FileUrl!.GetStreamAsync(ct);
        await using var fileStream = System.IO.File.OpenWrite(tmpFilePath);
        await netStream.CopyToAsync(fileStream, ct);

        var mime = await _fileProcessor.DetectMimeTypeAsync(tmpFilePath, ct);
        var hash = await _fileProcessor.CalculateHashAsync(tmpFilePath, ct);

        var file = new FileEntity()
        {
            Name = req.FileUrl!.Split("/").Last(),
            StorageName = decR.Storage.Name,
            PathOnStorage = decR.PathOnStorage,
            Size = exist!.Size,
            Extension = mime.Extension,
            MimeType = mime.MimeType,
            Hash = hash
        };

        _db.Files.Add(file);
        await _db.SaveChangesAsync(CancellationToken.None);
        file = await _db.Files.FirstAsync(x => x.Id == file.Id, ct);
        return _mapper.Map<FileModel>(file);
    }
}