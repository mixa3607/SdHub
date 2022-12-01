namespace SdHub.Options;

public class AppInfoOptions
{
    public string? BaseUrl { get; set; }
    public string? GitRefName { get; set; }
    public string? GitCommitSha { get; set; }
    public string? FrontDevServer { get; set; }
    public bool DisableUsersRegistration { get; set; }
}