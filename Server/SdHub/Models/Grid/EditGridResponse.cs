namespace SdHub.Models.Grid;

public class EditGridResponse
{
    public GridModel? Grid { get; set; }
    public bool Success { get; set; }
    public string? Reason { get; set; }
}