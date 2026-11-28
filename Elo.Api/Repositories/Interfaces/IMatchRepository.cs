using Elo.Api.Models;

public interface IMatchRepository
{
    Task<bool> ExistsAsync(Guid externalId);

    Task AddAsync(Match match);
}