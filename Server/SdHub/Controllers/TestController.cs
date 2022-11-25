using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SdHub.ApiTokenAuth;

namespace SdHub.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("[action]")]
    public string GetDefaultAuth()
    {
        return "OK";
    }

    [HttpGet("[action]")]
    [Authorize(AuthenticationSchemes = $"{ApiTokenDefaults.AuthenticationScheme}")]
    public string GetApiTokenAuth()
    {
        return "OK";
    }

    [HttpGet("[action]")]
    [Authorize(AuthenticationSchemes = $"{ApiTokenDefaults.AuthenticationScheme},{JwtBearerDefaults.AuthenticationScheme}")]
    public string GetApiTokenAndDefaultAuth()
    {
        return "OK";
    }

    [HttpGet("[action]")]
    [AllowAnonymous]
    public string GetAnonymousAuth()
    {
        return "OK";
    }
}