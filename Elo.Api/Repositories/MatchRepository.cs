using Elo.Api.Infrastructure;
using Elo.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Elo.Api.Repositories;

public class MatchRepository : IMatchRepository
{
    private readonly EloDbContext _db;

    public MatchRepository(EloDbContext db)
    {
        _db = db;
    }

    public async Task<bool> ExistsAsync(Guid externalId)
    {
        return await _db.Matches
            .AnyAsync(x => x.ExternalId == externalId);
    }

    public async Task AddAsync(Match match)
    {
        _db.Matches.Add(match);

        await _db.SaveChangesAsync();
    }
}