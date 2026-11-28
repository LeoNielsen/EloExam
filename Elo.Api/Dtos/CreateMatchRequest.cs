public class CreateMatchRequest
{
    public Guid ExternalId { get; set; }

    public int BlueTeamId { get; set; }

    public int RedTeamId { get; set; }

    public int BlueScore { get; set; }

    public int RedScore { get; set; }
}