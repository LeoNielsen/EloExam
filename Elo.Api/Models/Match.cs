namespace Elo.Api.Models;

public class Match
{
    public int Id { get; set; }

    public Guid ExternalId { get; set; }

    public int BlueTeamId { get; set; }
    public Team BlueTeam { get; set; } = null!;

    public int RedTeamId { get; set; }
    public Team RedTeam { get; set; } = null!;

    public int BlueScore { get; set; }
    public int RedScore { get; set; }

    public DateTime PlayedAt { get; set; }

    public MatchState State { get; set; }
}