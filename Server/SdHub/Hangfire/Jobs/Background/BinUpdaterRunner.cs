using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Flurl.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SdHub.Database;
using SdHub.Database.Entities.Bins;
using SdHub.Models.Bins;

namespace SdHub.Hangfire.Jobs;

public class BinUpdaterRunner : IBinUpdaterRunnerV1
{
    private readonly ILogger<BinUpdaterRunner> _logger;
    private readonly SdHubDbContext _db;
    private readonly IMapper _mapper;

    public string Name => "BinUpdater";

    public BinUpdaterRunner(ILogger<BinUpdaterRunner> logger, SdHubDbContext db, IMapper mapper)
    {
        _logger = logger;
        _db = db;
        _mapper = mapper;
    }


    public async Task UpdateModelVersionFilesAsync(long modelVersionId, bool force, CancellationToken ct = default)
    {
        var modelVersion = await _db.ModelVersions
            .Include(x => x.Files!)
            .ThenInclude(x => x.File!)
            .FirstOrDefaultAsync(x => x.Id == modelVersionId, ct);
        if (modelVersion == null)
            return;

        foreach (var file in modelVersion.Files!)
        {
            if (!force && file.ModelHashV1 != null)
            {
                _logger.LogWarning($"{nameof(file.ModelHashV1)} is set. Skip");
                continue;
            }

            await UpdateHashV1Async(file, ct);
        }

        await _db.SaveChangesAsync(CancellationToken.None);
    }

    public async Task UpdateHashV1Async(ModelVersionFileEntity file, CancellationToken ct = default)
    {
        var model = _mapper.Map<ModelVersionFileModel>(file);
        if (model.File == null)
        {
            _logger.LogWarning("File is null. Skip");
            return;
        }

        var bytes = await model.File.DirectUrl!
            .ConfigureRequest(x => { x.AllowedHttpStatusRange = "206"; })
            .WithHeader("Range", $"bytes={0x100000}-{0x100000 + 0x10000 - 1}")
            .GetBytesAsync(ct);
        if (bytes.Length != 0x10000)
        {
            _logger.LogError("Receive {act} but expected {exp}", bytes.Length, 0x10000);
            return;
        }

        using var sha = SHA256.Create();
        var hashBytes = sha.ComputeHash(bytes);
        file.ModelHashV1 = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
}