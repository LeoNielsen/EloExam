using Elo.Api.Dtos;
using Elo.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Elo.Api.Controllers;

[ApiController]
[Route("api/teams")]
public class TeamController : ControllerBase
{
    private readonly TeamService _teamService;

    public TeamController(
        TeamService teamService)
    {
        _teamService = teamService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateTeamRequest request)
    {
        var team =
            await _teamService.CreateTeam(
                request.AttackerId,
                request.DefenderId);

        return Created(
            $"api/teams/{team.Id}",
            team);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(
        int id)
    {
        var team =
            await _teamService.GetTeam(id);

        return Ok(team);
    }
}