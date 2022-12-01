using SdHub.Database.Entities.Images;

namespace SdHub.Database.Entities.Grids;

public class GridImageEntity
{
    public long GridId { get; set; }
    public GridEntity? Grid { get; set; }

    public long ImageId { get; set; }
    public ImageEntity? Image { get; set; }

    public int Order { get; set; }
}