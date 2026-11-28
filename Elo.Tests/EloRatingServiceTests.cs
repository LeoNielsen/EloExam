using Elo.Api.Models;
using Elo.Api.Services;
using FluentAssertions;

namespace Elo.Tests;

public class EloRatingServiceTests
{
    private readonly EloRatingService _sut = new();

    [Fact]
    public void Winner_should_gain_and_loser_should_lose_rating()
    {
        // Arrange
        var match = CreateMatch(
            blueAttackerRating: 1200,
            blueDefenderRating: 1200,
            redAttackerRating: 1200,
            redDefenderRating: 1200,
            blueScore: 10,
            redScore: 8);

        // Act
        var result = _sut.Calculate(match);

        // Assert
        result.BlueAttackerRating.Should().BeGreaterThan(1200);
        result.BlueDefenderRating.Should().BeGreaterThan(1200);

        result.RedAttackerRating.Should().BeLessThan(1200);
        result.RedDefenderRating.Should().BeLessThan(1200);
    }

    [Fact]
    public void Underdog_should_gain_more_than_favorite()
    {
        // Arrange
        var underdogMatch = CreateMatch(
            blueAttackerRating: 1000,
            blueDefenderRating: 1000,
            redAttackerRating: 1500,
            redDefenderRating: 1500,
            blueScore: 10,
            redScore: 5);

        var favoriteMatch = CreateMatch(
            blueAttackerRating: 1500,
            blueDefenderRating: 1500,
            redAttackerRating: 1000,
            redDefenderRating: 1000,
            blueScore: 10,
            redScore: 5);

        // Act
        var underdogResult = _sut.Calculate(underdogMatch);
        var favoriteResult = _sut.Calculate(favoriteMatch);

        var underdogGain = underdogResult.BlueAttackerRating - 1000;
        var favoriteGain = favoriteResult.BlueAttackerRating - 1500;

        // Assert
        underdogGain.Should().BeGreaterThan(favoriteGain);
    }

    [Fact]
    public void Larger_score_difference_should_give_larger_rating_change()
    {
        // Arrange
        var closeMatch = CreateMatch(
            blueAttackerRating: 1200,
            blueDefenderRating: 1200,
            redAttackerRating: 1200,
            redDefenderRating: 1200,
            blueScore: 10,
            redScore: 9);

        var dominantMatch = CreateMatch(
            blueAttackerRating: 1200,
            blueDefenderRating: 1200,
            redAttackerRating: 1200,
            redDefenderRating: 1200,
            blueScore: 10,
            redScore: 0);

        // Act
        var closeResult = _sut.Calculate(closeMatch);
        var dominantResult = _sut.Calculate(dominantMatch);

        var closeMatchGain = closeResult.BlueAttackerRating - 1200;

        var dominantMatchGain = dominantResult.BlueAttackerRating - 1200;

        // Assert
        dominantMatchGain.Should().BeGreaterThan(closeMatchGain);
    }

    [Fact]
    public void All_player_ratings_should_be_affected()
    {
        // Arrange
        var match = CreateMatch(
            blueAttackerRating: 1200,
            blueDefenderRating: 1200,
            redAttackerRating: 1200,
            redDefenderRating: 1200,
            blueScore: 10,
            redScore: 5);

        var blueAttackerRatingBefore = match.BlueTeam.Attacker.EloRating;
        var blueDefenderRatingBefore = match.BlueTeam.Defender.EloRating;
        var redAttackerRatingBefore = match.RedTeam.Attacker.EloRating;
        var redDefenderRatingBefore = match.RedTeam.Defender.EloRating;

        // Act
        var result = _sut.Calculate(match);

        // Assert
        result.BlueAttackerRating.Should().NotBe(blueAttackerRatingBefore);

        result.BlueDefenderRating.Should().NotBe(blueDefenderRatingBefore);

        result.RedAttackerRating.Should().NotBe(redAttackerRatingBefore);

        result.RedDefenderRating.Should().NotBe(redDefenderRatingBefore);
    }

    private static Match CreateMatch(int blueAttackerRating,
        int blueDefenderRating,
        int redAttackerRating,
        int redDefenderRating,
        int blueScore,
        int redScore)
    {
        return new Match
        {
            BlueTeam = CreateTeam(
                blueAttackerRating,
                blueDefenderRating),

            RedTeam = CreateTeam(
                redAttackerRating,
                redDefenderRating),

            BlueScore = blueScore,
            RedScore = redScore
        };
    }

    private static Team CreateTeam(
        int attackerRating,
        int defenderRating)
    {
        return new Team
        {
            Attacker = new Player
            {
                EloRating = attackerRating
            },

            Defender = new Player
            {
                EloRating = defenderRating
            }
        };
    }
}