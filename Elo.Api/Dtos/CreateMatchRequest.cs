namespace Elo.Api.Dtos;

public class CreateMatchRequest
{
    public int BlueTeamId { get; set; }

    public int RedTeamId { get; set; }

    public int BlueScore { get; set; }

    public int RedScore { get; set; }
}