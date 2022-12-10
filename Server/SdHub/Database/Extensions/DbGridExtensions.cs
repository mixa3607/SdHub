using System.Linq;
using SdHub.Database.Entities.Grids;

namespace SdHub.Database.Extensions;

public static class DbGridExtensions
{
    public static IQueryable<GridEntity> ApplyFilter(this IQueryable<GridEntity> q, string? shortCode = null,
        bool? deleted = false, bool? userDeleted = false, long? ownerId = null)
    {
        if (shortCode != null)
            q = q.Where(x => x.ShortToken == shortCode);

        if (deleted == false)
            q = q.Where(x => x.DeletedAt == null);
        else if (deleted == true)
            q = q.Where(x => x.DeletedAt != null);

        if (ownerId != null)
            q = q.Where(x => x.OwnerId == ownerId);

        if (userDeleted == false)
            q = q.Where(x => x.Owner!.DeletedAt == null);
        else if (userDeleted == true)
            q = q.Where(x => x.Owner!.DeletedAt != null);

        return q;
    }
}