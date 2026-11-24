using Elo.Api.Models;
using Elo.Api.Services;
using FluentAssertions;

namespace Elo.Tests;

public class EloRatingServiceTests
{
    [Fact]
    public void Winning_team_should_gain_rating()
    {
        var sut = new EloRatingService();

        var match = new Match
        {
            BlueTeam = CreateTeam(1200, 1200),
            RedTeam = CreateTeam(1200, 1200),
            BlueScore = 10,
            RedScore = 8
        };

        var result = sut.Calculate(match);
        
        result.BlueAttackerRating
            .Should()
            .BeGreaterThan(1200);

        result.BlueDefenderRating
            .Should()
            .BeGreaterThan(1200);

        result.RedAttackerRating
            .Should()
            .BeLessThan(1200);

        result.RedDefenderRating
            .Should()
            .BeLessThan(1200);
    }

    [Fact]
    public void Underdog_should_gain_more_rating()
    {
        var sut = new EloRatingService();

        var match = new Match
        {
            BlueTeam = CreateTeam(1000, 1000),
            RedTeam = CreateTeam(1500, 1500),
            BlueScore = 10,
            RedScore = 5
        };

        var result = sut.Calculate(match);

        result.BlueAttackerRating
            .Should()
            .BeGreaterThan(1020);

        result.BlueDefenderRating
            .Should()
            .BeGreaterThan(1020);
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