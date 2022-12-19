using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Flurl.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SdHub.Database;
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
            .Include(x => x.CkptFile)
            .FirstOrDefaultAsync(x => x.Id == modelVersionId, ct);
        if (modelVersion == null)
            return;

        if (!force && modelVersion.HashV1 != null)
        {
            _logger.LogWarning($"{nameof(modelVersion.HashV1)} is set. Skip");
            return;
        }

        var mvM = _mapper.Map<ModelVersionModel>(modelVersion);
        if (mvM.CkptFile == null)
        {
            _logger.LogWarning("Ckpt file is null. Skip");
            return;
        }

        var bytes = await mvM.CkptFile.DirectUrl!
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
        modelVersion.HashV1 = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        await _db.SaveChangesAsync(CancellationToken.None);
    }
}