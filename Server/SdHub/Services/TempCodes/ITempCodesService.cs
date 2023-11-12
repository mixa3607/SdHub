using System;
using System.Threading;
using System.Threading.Tasks;

namespace SdHub.Services.TempCodes;

public interface ITempCodesService
{
    Task<string> CreateAsync(string key, int attempts, TimeSpan lifetime,
        CancellationToken ct = default);

    Task<string> CreateAsync(string key, int attempts, DateTimeOffset lifetime,
        CancellationToken ct = default);

    Task<TempCodeActivateResult> ActivateAsync(string key, string code, bool increaseAttempts,
        CancellationToken ct = default);
}