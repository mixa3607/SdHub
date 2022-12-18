using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SdHub.Services.Tokens;

namespace SdHub.Controllers;

[AllowAnonymous]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class JwksController : ControllerBase
{
    private readonly IJwtGeneratorService _jwtGenerator;

    public JwksController(IJwtGeneratorService jwtGenerator)
    {
        _jwtGenerator = jwtGenerator;
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet]
    public ActionResult<JsonWebKeySet> Get(CancellationToken ct = default)
    {
        var keySet = new JsonWebKeySet();
        keySet.Keys.Add(_jwtGenerator.GenerateJwk());
        return Ok(keySet);
    }
}