using System.Threading;
using System.Threading.Tasks;

namespace SdHub.Hangfire.Jobs;

public class BinUpdaterRunner : IBinUpdaterRunnerV1
{
    public string Name => "BinUpdater";

    public Task UpdateModelVersionFilesAsync(long modelVersionId, bool force, CancellationToken ct = default)
    {
        throw new System.NotImplementedException();
    }
}