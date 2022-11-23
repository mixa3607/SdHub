using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SdHub.Database;
using SdHub.Database.Entities;
using SimpleBase;

namespace SdHub.Services.Tokens;

public class TempCodesService : ITempCodesService
{
    private readonly SdHubDbContext _db;

    public TempCodesService(SdHubDbContext db)
    {
        _db = db;
    }

    public async Task<string> CreateAsync(string identifier, int attempts, TimeSpan lifetime, TempCodeType type,
        CancellationToken ct = default)
    {
        var entity = new TempCodeEntity()
        {
            ExpiredAt = DateTimeOffset.Now.Add(-lifetime),
            Code = GenerateShortToken(),
            CodeType = type,
            CurrAttempts = 0,
            CreatedAt = DateTimeOffset.Now,
            Identifier = identifier,
            MaxAttempts = attempts
        };
        _db.TempCodes.Add(entity);
        await _db.SaveChangesAsync(CancellationToken.None);

        return entity.Code;
    }

    public async Task<TempCodeActivateResult> ActivateAsync(string identifier, string code, bool encAttempts,
        CancellationToken ct = default)
    {
        var entity = await _db.TempCodes.FirstOrDefaultAsync(x => x.Identifier == identifier && x.Code == code && !x.Used, ct);
        if (entity == null)
            return TempCodeActivateResult.NotFound;
        if (entity.ExpiredAt > DateTimeOffset.Now)
            return TempCodeActivateResult.Lifetime;
        if (entity.MaxAttempts <= entity.CurrAttempts)
            return TempCodeActivateResult.MaxAttemptsReached;

        if (encAttempts) 
            entity.CurrAttempts++;

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