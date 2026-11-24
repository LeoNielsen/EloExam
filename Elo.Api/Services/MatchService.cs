using Elo.Api.Infrastructure;
using Elo.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Elo.Api.Services;

public class MatchService
{
    private readonly EloDbContext _db;
    private readonly EloRatingService _eloRatingService;
    private readonly TeamService _teamService;
    
    public MatchService(EloDbContext db, TeamService teamService, EloRatingService eloRatingService)
    {
        _db = db;
        _teamService = teamService;
        _eloRatingService = eloRatingService;
    }

    public async Task<Match> CreateMatch(int blueTeamId, int redTeamId, int blueScore, int redScore)
    {
        var blueTeam = await _teamService.GetTeam(blueTeamId);
        var redTeam = await _teamService.GetTeam(redTeamId);

        var match = new Match
        {
            BlueTeamId = blueTeamId,
            RedTeamId = redTeamId,
            BlueTeam = blueTeam,
            RedTeam = redTeam,
            BlueScore = blueScore,
            RedScore = redScore,
            PlayedAt = DateTime.UtcNow,
            State = MatchState.Created
        };

        ValidateMatch(match);

        ProcessMatch(match);

        _db.Matches.Add(match);

        await _db.SaveChangesAsync();

        return match;
    }

    private async Task<Team> GetTeam(int id)
    {
        return await _db.Teams
            .Include(x => x.Attacker)
            .Include(x => x.Defender)
            .FirstAsync(x => x.Id == id);
    }

    private void ValidateMatch(Match match)
    {
        if (match.BlueTeamId == match.RedTeamId)
        {
            throw new ArgumentException(
                "A team cannot play itself.");
        }

        if (match.BlueScore == match.RedScore)
        {
            throw new ArgumentException(
                "Draws are not allowed.");
        }

        bool blueWon = match.BlueScore > match.RedScore;

        int winnerScore = blueWon
            ? match.BlueScore
            : match.RedScore;

        int loserScore = blueWon
            ? match.RedScore
            : match.BlueScore;

        if (winnerScore != 10)
        {
            throw new ArgumentException(
                "Winner must score exactly 10 goals.");
        }

        if (loserScore < 0 ||
            loserScore > 9)
        {
            throw new ArgumentException(
                "Loser score must be between 0 and 9.");
        }

        match.State = MatchState.Validated;
    }

    private void ProcessMatch(Match match)
    {
        if (match.State != MatchState.Validated)
        {
            throw new InvalidOperationException(
                "Match must be validated first.");
        }

        var result =
            _eloRatingService.Calculate(match);

        match.BlueTeam.Attacker.EloRating =
            result.BlueAttackerRating;

        match.BlueTeam.Defender.EloRating =
            result.BlueDefenderRating;

        match.RedTeam.Attacker.EloRating =
            result.RedAttackerRating;

        match.RedTeam.Defender.EloRating =
            result.RedDefenderRating;

        match.State = MatchState.Processed;
    }
}