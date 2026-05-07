using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectLink.Application.Currency;
using ProjectLink.Contracts.Currency;

namespace ProjectLink.API.Controllers;

[ApiController]
[Route("api/currency")]
[Authorize]
public class CurrencyController : ControllerBase
{
    private readonly CurrencyService _currency;

    public CurrencyController(CurrencyService currency) => _currency = currency;

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Ok(await _currency.GetAsync(userId, ct));
    }

    [HttpPost("ad-reward")]
    public async Task<IActionResult> AdReward([FromBody] CurrencyAdRewardRequest req, CancellationToken ct)
    {
        var userId        = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var correlationId = HttpContext.Items["CorrelationId"] as string ?? HttpContext.TraceIdentifier;
        return Ok(await _currency.AdRewardAsync(userId, req.AdToken, correlationId, ct));
    }
}
