using System.Linq;
using SdHub.Database.Entities.Images;

namespace SdHub.Database.Extensions;

public static class DbImageExtensions
{
    public static IQueryable<ImageEntity> ApplyFilter(this IQueryable<ImageEntity> q, string? shortCode = null,
        long? ownerId = null, bool? deleted = null, bool? userDeleted = false, bool? gridDeleted = null,
        bool? inGrid = null)
    {
        if (shortCode != null)
            q = q.Where(x => x.ShortToken == shortCode);

        if (deleted == false)
            q = q.Where(x => x.DeletedAt == null);
        else if (deleted == true)
            q = q.Where(x => x.DeletedAt != null);

        if (gridDeleted == false)
            q = q.Where(x => x.GridImage!.Grid!.DeletedAt == null);
        else if (gridDeleted == true)
            q = q.Where(x => x.GridImage!.Grid!.DeletedAt != null);

        if (inGrid == false)
            q = q.Where(x => x.GridImage == null);
        else if (inGrid == true)
            q = q.Where(x => x.GridImage != null);

        if (ownerId != null)
            q = q.Where(x => x.OwnerId == ownerId);

        if (userDeleted == false)
            q = q.Where(x => x.Owner!.DeletedAt == null);
        else if (userDeleted == true)
            q = q.Where(x => x.Owner!.DeletedAt != null);

        return q;
    }
}