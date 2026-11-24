using Elo.Api.Models;

namespace Elo.Api.Services;

public class EloRatingService
{
    private const int KFactor = 32;

    public EloCalculationResult Calculate(
        Team blueTeam,
        Team redTeam,
        int blueScore,
        int redScore)
    {
        bool blueWon =
            blueScore > redScore;

        double expectedBlue =
            1.0 /
            (1.0 +
             Math.Pow(
                 10,
                 (redTeam.TeamRating -
                  blueTeam.TeamRating) / 400));

        double expectedRed =
            1 - expectedBlue;

        int actualBlue =
            blueWon ? 1 : 0;

        int ratingChange =
            (int)Math.Round(
                KFactor *
                (actualBlue - expectedBlue));

        return new EloCalculationResult
        {
            BlueAttackerRating =
                blueTeam.Attacker.EloRating +
                ratingChange,

            BlueDefenderRating =
                blueTeam.Defender.EloRating +
                ratingChange,

            RedAttackerRating =
                redTeam.Attacker.EloRating -
                ratingChange,

            RedDefenderRating =
                redTeam.Defender.EloRating -
                ratingChange,

            RatingChange =
                ratingChange
        };
    }
}