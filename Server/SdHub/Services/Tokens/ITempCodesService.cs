using System;
using System.Threading;
using System.Threading.Tasks;

namespace SdHub.Services.Tokens;

public interface ITempCodesService
{
    Task<string> CreateAsync(string identifier, int attempts, TimeSpan lifetime, TempCodeType type, 
        CancellationToken ct = default);

    Task<TempCodeActivateResult> ActivateAsync(string identifier, string code, bool encAttempts,
        CancellationToken ct = default);
}