using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SdHub.Database;
using SdHub.Database.Entities.Users;
using SimpleBase;

namespace SdHub.Services.TempCodes;

public class TempCodesService : ITempCodesService
{
    private readonly SdHubDbContext _db;

    public TempCodesService(SdHubDbContext db)
    {
        _db = db;
    }

    public async Task<string> CreateAsync(string key, int attempts, TimeSpan lifetime, CancellationToken ct = default)
    {
        return await CreateAsync(key, attempts, DateTimeOffset.UtcNow.Add(-lifetime), ct);
    }

    public async Task<string> CreateAsync(string key, int attempts, DateTimeOffset lifetime, CancellationToken ct = default)
    {
        var entity = new TempCodeEntity()
        {
            ExpiredAt = lifetime.ToUniversalTime(),
            Code = GenerateShortToken(),
            CreatedAt = DateTimeOffset.UtcNow,
            MaxAttempts = attempts,
            Key = key,
        };
        _db.TempCodes.Add(entity);
        await _db.SaveChangesAsync(CancellationToken.None);

        return entity.Code;
    }

    public async Task<TempCodeActivateResult> ActivateAsync(string key, string code, bool increaseAttempts,
        CancellationToken ct = default)
    {
        var entity = await _db.TempCodes.FirstOrDefaultAsync(x => x.Key == key && x.Code == code, ct);
        if (entity == null)
            return TempCodeActivateResult.NotFound;
        if (entity.Used)
            return TempCodeActivateResult.Used;
        if (entity.ExpiredAt > DateTimeOffset.UtcNow)
            return TempCodeActivateResult.Lifetime;
        if (entity.MaxAttempts <= entity.UsedAttempts)
            return TempCodeActivateResult.MaxAttemptsReached;

        if (increaseAttempts)
            entity.UsedAttempts++;

        entity.Used = true;

        await _db.SaveChangesAsync(CancellationToken.None);
        return TempCodeActivateResult.Ok;
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