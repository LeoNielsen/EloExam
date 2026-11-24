using Elo.Api.Infrastructure;
using Elo.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Elo.Api.Services;

public class PlayerService
{
    private readonly EloDbContext _db;

    public PlayerService(EloDbContext db)
    {
        _db = db;
    }

    public async Task<Player> CreatePlayer(
        string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Name is required.");
        }

        var player = new Player
        {
            Name = name.Trim(),
            EloRating = 1200
        };

        _db.Players.Add(player);

        await _db.SaveChangesAsync();

        return player;
    }

    public async Task<Player> GetPlayer(
        int playerId)
    {
        return await _db.Players
            .FirstAsync(x => x.Id == playerId);
    }

    public async Task<List<Player>> GetPlayers()
    {
        return await _db.Players
            .OrderByDescending(x => x.EloRating)
            .ToListAsync();
    }
}