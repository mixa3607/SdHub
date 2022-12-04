using SdHub.Database.Entities.Grids;
using SdHub.Database.Entities.Images;

namespace SdHub.Database.Entities.Albums;

public class AlbumImageEntity
{
    public long Id { get; set; }

    public long AlbumId { get; set; }
    public AlbumEntity? Album { get; set; }

    public long? ImageId { get; set; }
    public ImageEntity? Image { get; set; }

    public long? GridId { get; set; }
    public GridEntity? Grid { get; set; }
}