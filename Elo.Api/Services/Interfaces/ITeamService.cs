using Elo.Api.Models;

public interface ITeamService
{
    Task<Team> GetTeam(int teamId);
}
