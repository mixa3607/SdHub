using SdHub.Database.Entities.Grids;
using SdHub.Database.Entities.Images;

namespace SdHub.Models.Album;

public class AlbumImageModel
{
    public long? ImageId { get; set; }
    public ImageEntity? Image { get; set; }

    public long? GridId { get; set; }
    public GridEntity? Grid { get; set; }
}