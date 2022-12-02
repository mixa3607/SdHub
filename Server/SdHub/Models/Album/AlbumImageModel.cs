using SdHub.Models.Grid;
using SdHub.Models.Image;

namespace SdHub.Models.Album;

public class AlbumImageModel
{
    public long? ImageId { get; set; }
    public ImageModel? Image { get; set; }

    public long? GridId { get; set; }
    public GridModel? Grid { get; set; }
}