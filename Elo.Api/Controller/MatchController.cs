using Elo.Api.Dtos;
using Elo.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Elo.Api.Controllers;

[ApiController]
[Route("api/matches")]
public class MatchController : ControllerBase
{
    private readonly MatchService _matchService;

    public MatchController(
        MatchService matchService)
    {
        _matchService = matchService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateMatchRequest request)
    {
        var match =
            await _matchService.CreateMatch(
                request.BlueTeamId,
                request.RedTeamId,
                request.BlueScore,
                request.RedScore);

        return Created(
            $"api/matches/{match.Id}",
            match);
    }
}