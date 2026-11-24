using Elo.Api.Models;

namespace Elo.Api.Services;

public class EloRatingService
{
    public EloCalculationResult Calculate(Match match)
    {
        bool blueWon = match.BlueScore > match.RedScore;
        
        Team blueTeam = match.BlueTeam;
        Team redTeam = match.RedTeam;

        double pointMultiplier = CalculatePointMultiplier(match.BlueScore, match.RedScore);
        
        double blueAttackerOdds = CalculatePlayerOdds(blueTeam.Attacker, redTeam);
        double blueDefenderOdds = CalculatePlayerOdds(blueTeam.Defender, redTeam);
        double redAttackerOdds = CalculatePlayerOdds(redTeam.Attacker, blueTeam);
        double redDefenderOdds = CalculatePlayerOdds(redTeam.Defender, blueTeam);

        double blueTeamOdds = (blueAttackerOdds + blueDefenderOdds) / 2;
        double redTeamOdds = (redAttackerOdds + redDefenderOdds) / 2;

        return new EloCalculationResult
        {
            BlueAttackerRating = CalculateNewRating(blueTeam.Attacker.EloRating, pointMultiplier, 
                (blueTeamOdds + blueAttackerOdds) / 2, blueWon),

            BlueDefenderRating = CalculateNewRating(blueTeam.Defender.EloRating, pointMultiplier,
                (blueTeamOdds + blueDefenderOdds) / 2, blueWon),

            RedAttackerRating = CalculateNewRating(redTeam.Attacker.EloRating, pointMultiplier,
                (redTeamOdds + redAttackerOdds) / 2, !blueWon),

            RedDefenderRating = CalculateNewRating(redTeam.Defender.EloRating, pointMultiplier,
                    (redTeamOdds + redDefenderOdds) / 2, !blueWon)
        };
    }

    private double CalculatePlayerOdds(Player player, Team opponentTeam)
    {
        double oddsAgainstAttacker = CalculateOdds(player.EloRating, opponentTeam.Attacker.EloRating);
        double oddsAgainstDefender = CalculateOdds(player.EloRating, opponentTeam.Defender.EloRating);

        return (oddsAgainstAttacker + oddsAgainstDefender) / 2;
    }

    private double CalculateOdds(int playerRating, int opponentRating)
    {
        return 1.0 / (1.0 + Math.Pow(10, (double)(opponentRating - playerRating) / 500));
    }

    private double CalculatePointMultiplier(int blueScore, int redScore)
    {
        int difference = Math.Abs(blueScore - redScore);

        if (difference <= 1)
        {
            return 1;
        }

        return 1 + Math.Pow(Math.Log10(difference), 3);
    }

    private int CalculateNewRating(int currentRating, double pointMultiplier, double odds, bool winner)
    {
        return (int)Math.Round(currentRating + (32 * pointMultiplier) * ((winner ? 1.0 : 0.0) - odds));
    }
}