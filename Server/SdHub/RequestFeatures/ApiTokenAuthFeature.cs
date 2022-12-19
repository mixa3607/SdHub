using SdHub.Models;

namespace SdHub.RequestFeatures;

public class ApiTokenAuthFeature
{
    public UserModel User { get; }

    public ApiTokenAuthFeature(UserModel user)
    {
        User = user;
    }
}