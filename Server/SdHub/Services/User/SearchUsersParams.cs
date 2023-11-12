namespace SdHub.Services.User;

public class SearchUsersParams
{
    public int Skip { get; set; }
    public int Count { get; set; } = 20;

    public string? Query { get; set; }
}