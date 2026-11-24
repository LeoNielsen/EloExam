public class MatchService
{
    private readonly EloDbContext _db;
    private readonly EloRatingService _eloService;

    public MatchService(
        EloDbContext db,
        EloRatingService eloService)
    {
        _db = db;
        _eloService = eloService;
    }

    public async Task<Match> CreateMatch(
        Team blueTeam,
        Team redTeam,
        int blueScore,
        int redScore)
    {
        var match = new Match
        {
            BlueTeamId = blueTeam.Id,
            RedTeamId = redTeam.Id,
            BlueScore = blueScore,
            RedScore = redScore,
            State = MatchState.Created,
            PlayedAt = DateTime.UtcNow
        };

        match.State = MatchState.Validated;

        var eloResult =
            _eloService.Calculate(
                blueTeam,
                redTeam,
                blueScore,
                redScore);

        blueTeam.Attacker.EloRating =
            eloResult.BlueAttackerRating;

        blueTeam.Defender.EloRating =
            eloResult.BlueDefenderRating;

        redTeam.Attacker.EloRating =
            eloResult.RedAttackerRating;

        redTeam.Defender.EloRating =
            eloResult.RedDefenderRating;

        match.State =
            MatchState.Processed;

        _db.Matches.Add(match);

        await _db.SaveChangesAsync();

        return match;
    }
}
