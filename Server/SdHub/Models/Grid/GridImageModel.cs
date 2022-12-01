using SdHub.Models.Image;

namespace SdHub.Models.Grid;

public class GridImageModel
{
    public int Order { get; set; }
    public ImageModel? Image { get; set; }
}