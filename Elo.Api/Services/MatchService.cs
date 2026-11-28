using Elo.Api.Models;


namespace Elo.Api.Services;

public class MatchService
{
    private readonly ITeamService _teamService;
    private readonly IMatchRepository _matchRepository;
    private readonly EloRatingService _eloRatingService;

    public MatchService(
        ITeamService teamService,
        IMatchRepository matchRepository,
        EloRatingService eloRatingService)
    {
        _teamService = teamService;
        _matchRepository = matchRepository;
        _eloRatingService = eloRatingService;
    }

    public async Task<Match> CreateMatch(
        Guid externalId,
        int blueTeamId,
        int redTeamId,
        int blueScore,
        int redScore)
    {
        // Check for duplicate
        if (await _matchRepository.ExistsAsync(externalId))
        {
            throw new InvalidOperationException(
                "Match has already been registered.");
        }

        // Get teams
        var blueTeam =
            await _teamService.GetTeam(blueTeamId);

        var redTeam =
            await _teamService.GetTeam(redTeamId);

        // Create match
        var match = new Match
        {
            ExternalId = externalId,

            BlueTeamId = blueTeamId,
            BlueTeam = blueTeam,

            RedTeamId = redTeamId,
            RedTeam = redTeam,

            BlueScore = blueScore,
            RedScore = redScore,

            PlayedAt = DateTime.UtcNow,

            State = MatchState.Created
        };

        // Validate
        ValidateMatch(match);

        // Calculate ELO and process
        ProcessMatch(match);

        // Save
        await _matchRepository.AddAsync(match);

        return match;
    }

    public void ValidateMatch(Match match)
    {
        var playerIds = new[]
        {
            match.BlueTeam.AttackerId,
            match.BlueTeam.DefenderId,
            match.RedTeam.AttackerId,
            match.RedTeam.DefenderId
        };

        if (playerIds.Distinct().Count() != 4)
        {
            throw new ArgumentException(
                "A player can only participate once in a match.");
        }

        bool blueWon =
            match.BlueScore > match.RedScore;

        int winnerScore =
            blueWon
                ? match.BlueScore
                : match.RedScore;

        int loserScore =
            blueWon
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
                "Match must be validated before processing.");
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