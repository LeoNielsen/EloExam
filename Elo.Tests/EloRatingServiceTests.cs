using Elo.Api.Services;
using FluentAssertions;

public class EloRatingServiceTests
{
    [Fact]
    public void Equal_players_gain_16_points()
    {
        var sut = new EloRatingService();

        var result = sut.Calculate(1200, 1200);

        result.winner.Should().Be(1216);
        result.loser.Should().Be(1184);
    }
}