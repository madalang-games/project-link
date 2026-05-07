using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectLink.Application.Progress;
using ProjectLink.Contracts.Progress;

namespace ProjectLink.API.Controllers;

[ApiController]
[Route("api/progress")]
[Authorize]
public class ProgressController : ControllerBase
{
    private readonly GetProgressQueryHandler _getHandler;

    public ProgressController(GetProgressQueryHandler getHandler)
    {
        _getHandler = getHandler;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Ok(await _getHandler.HandleAsync(new GetProgressQuery(userId, ct)));
    }

    [HttpPost("batch")]
    public async Task<IActionResult> GetBatch([FromBody] BatchProgressRequest req, CancellationToken ct)
    {
        var userId   = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var response = await _getHandler.HandleAsync(new GetProgressQuery(userId, ct));
        var filtered = response.Stages.Where(s => req.StageIds.Contains(s.StageId)).ToList();
        return Ok(new ProgressResponse { Stages = filtered });
    }
}
