namespace Elo.Api.Models;

public class Team
{
    public int Id { get; set; }

    public int AttackerId { get; set; }

    public Player Attacker { get; set; } = null!;

    public int DefenderId { get; set; }

    public Player Defender { get; set; } = null!;
}
