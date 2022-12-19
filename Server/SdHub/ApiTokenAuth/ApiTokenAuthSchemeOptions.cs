using Microsoft.AspNetCore.Authentication;

namespace SdHub.ApiTokenAuth;

public class ApiTokenAuthSchemeOptions : AuthenticationSchemeOptions
{
    /// <summary>
    /// Gets or sets the challenge to put in the "WWW-Authenticate" header.
    /// </summary>
    public string Challenge { get; set; } = ApiTokenDefaults.AuthenticationScheme;
}