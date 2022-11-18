using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SdHub.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[AllowAnonymous]
public class TestController : ControllerBase
{
    [HttpGet]
    public string Get()
    {
        return "OK";
    }
}