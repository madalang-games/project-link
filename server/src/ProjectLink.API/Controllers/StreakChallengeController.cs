using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectLink.Application.StreakChallenge;
using ProjectLink.Contracts.StreakChallenge;

namespace ProjectLink.API.Controllers;

[ApiController]
[Route("api/streak-challenge")]
[Authorize]
public class StreakChallengeController : ControllerBase
{
    private readonly StreakChallengeService _service;

    public StreakChallengeController(StreakChallengeService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Ok(await _service.GetStateAsync(userId, ct));
    }

    [HttpPost("activate")]
    public async Task<IActionResult> Activate(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Ok(await _service.ActivateAsync(userId, ct));
    }

    [HttpPost("level/{level:int}/start")]
    public async Task<IActionResult> StartLevel(int level, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Ok(await _service.StartLevelAsync(userId, level, ct));
    }

    [HttpPost("level/{level:int}/claim-reward")]
    public async Task<IActionResult> ClaimReward(int level, [FromBody] StreakChallengeClaimRewardRequest req, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Ok(await _service.ClaimRewardAsync(userId, level, req.CorrelationId, ct));
    }

    [HttpPost("level/{level:int}/claim-reward-with-ad")]
    public async Task<IActionResult> ClaimRewardWithAd(int level, [FromBody] StreakChallengeClaimRewardWithAdRequest req, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Ok(await _service.ClaimRewardWithAdAsync(userId, level, req.AdToken, req.AdPlacementId, req.CorrelationId, ct));
    }
}
