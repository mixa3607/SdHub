using System.Linq;
using SdHub.Database.Entities.Albums;

namespace SdHub.Database.Extensions;

public static class DbAlbumExtensions
{
    public static IQueryable<AlbumEntity> ApplyFilter(this IQueryable<AlbumEntity> q, string? shortCode = null,
        long? ownerId = null, bool? deleted = false, bool? userDeleted = false)
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