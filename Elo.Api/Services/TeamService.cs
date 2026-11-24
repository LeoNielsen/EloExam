using Elo.Api.Infrastructure;
using Elo.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Elo.Api.Services;

public class TeamService
{
    private readonly EloDbContext _db;
    private readonly PlayerService _playerService;

    public TeamService(EloDbContext db, PlayerService playerService)
    {
        _db = db;
        _playerService = playerService;
    }

    public async Task<Team> CreateTeam(
        int attackerId,
        int defenderId)
    {
        if (attackerId == defenderId)
        {
            throw new ArgumentException(
                "Attacker and defender must be different players.");
        }

        var attacker = await _playerService.GetPlayer(attackerId);

        var defender = await _playerService.GetPlayer(defenderId);
        
        if (attacker == null)
        {
            throw new ArgumentException(
                "Attacker not found.");
        }

        if (defender == null)
        {
            throw new ArgumentException(
                "Defender not found.");
        }

        var team = new Team
        {
            AttackerId = attackerId,
            DefenderId = defenderId
        };

        _db.Teams.Add(team);

        await _db.SaveChangesAsync();

        return team;
    }

    public async Task<Team> GetTeam(int teamId)
    {
        return await _db.Teams
            .Include(x => x.Attacker)
            .Include(x => x.Defender)
            .FirstAsync(x => x.Id == teamId);
    }
}