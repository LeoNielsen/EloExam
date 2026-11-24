using Elo.Api.Dtos;
using Elo.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Elo.Api.Controllers;

[ApiController]
[Route("api/players")]
public class PlayerController : ControllerBase
{
    private readonly PlayerService _playerService;

    public PlayerController(
        PlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreatePlayerRequest request)
    {
        var player =
            await _playerService.CreatePlayer(
                request.Name);

        return CreatedAtAction(
            nameof(GetById),
            new { id = player.Id },
            player);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(
            await _playerService.GetPlayers());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        int id)
    {
        return Ok(
            await _playerService.GetPlayer(id));
    }
}