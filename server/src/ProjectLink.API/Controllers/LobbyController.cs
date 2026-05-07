using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectLink.Application.Lobby;

namespace ProjectLink.API.Controllers;

[ApiController]
[Route("api/lobby")]
[Authorize]
public class LobbyController : ControllerBase
{
    private readonly LobbyService _lobby;

    public LobbyController(LobbyService lobby) => _lobby = lobby;

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Ok(await _lobby.GetAsync(userId, ct));
    }
}
