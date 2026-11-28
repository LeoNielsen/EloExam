using Elo.Api.Models;
using Elo.Api.Services;
using FluentAssertions;
using Moq;

using Match = Elo.Api.Models.Match;

namespace Elo.Tests;

public class MatchServiceTests
{
    private readonly Mock<ITeamService> _teamServiceMock;
    private readonly Mock<IMatchRepository> _matchRepositoryMock;
    private readonly MatchService _sut;

    public MatchServiceTests()
    {
        _teamServiceMock = new Mock<ITeamService>();
        _matchRepositoryMock = new Mock<IMatchRepository>();

        var eloRatingService = new EloRatingService();

        _sut = new MatchService(
            _teamServiceMock.Object,
            _matchRepositoryMock.Object,
            eloRatingService);
    }

    [Theory]
    [InlineData(10, 0)]
    [InlineData(10, 5)]
    [InlineData(10, 9)]
    [InlineData(0, 10)]
    public void Valid_scores_should_be_accepted(
        int blueScore,
        int redScore)
    {
        // Arrange
        var match = CreateMatch(
            blueScore,
            redScore);

        // Act
        _sut.ValidateMatch(match);

        // Assert
        match.State.Should().Be(MatchState.Validated);
    }

    [Theory]
    [InlineData(9, 0)]
    [InlineData(11, 5)]
    [InlineData(0, 9)]
    public void Winner_must_have_exactly_10_points(
        int blueScore,
        int redScore)
    {
        // Arrange
        var match = CreateMatch(
            blueScore,
            redScore);

        // Act
        var action = () =>
            _sut.ValidateMatch(match);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(10, -1)]
    [InlineData(-1, 10)]
    public void Loser_score_must_be_between_0_and_9(
        int blueScore,
        int redScore)
    {
        // Arrange
        var match = CreateMatch(
            blueScore,
            redScore);

        // Act
        var action = () =>
            _sut.ValidateMatch(match);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(1, 1, 2, 3)] // Samme spiller på Blue
    [InlineData(1, 2, 3, 3)] // Samme spiller på Red
    [InlineData(1, 2, 1, 3)] // Blue attacker = Red attacker
    [InlineData(1, 2, 3, 1)] // Blue attacker = Red defender
    [InlineData(1, 2, 2, 3)] // Blue defender = Red attacker
    [InlineData(1, 2, 3, 2)] // Blue defender = Red defender
    public void Player_can_only_participate_once_in_a_match(int blueAttackerId, int blueDefenderId,
        int redAttackerId, int redDefenderId)
    {
        // Arrange
        var match = CreateMatch(
            blueScore: 10,
            redScore: 5,
            blueAttackerId: blueAttackerId,
            blueDefenderId: blueDefenderId,
            redAttackerId: redAttackerId,
            redDefenderId: redDefenderId);

        // Act
        var action = () => _sut.ValidateMatch(match);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public async Task Valid_match_should_end_as_processed()
    {
        // Arrange
        SetupTeams();

        var externalId = Guid.NewGuid();

        // Act
        var match = await _sut.CreateMatch(
            externalId,
            blueTeamId: 1,
            redTeamId: 2,
            blueScore: 10,
            redScore: 5);

        // Assert
        match.State.Should()
            .Be(MatchState.Processed);
    }

    [Fact]
    public async Task Duplicate_match_should_be_rejected()
    {
        // Arrange
        var externalId = Guid.NewGuid();

        _matchRepositoryMock
            .Setup(repository =>
                repository.ExistsAsync(externalId))
            .ReturnsAsync(true);

        // Act
        var action = async () =>
            await _sut.CreateMatch(
                externalId,
                blueTeamId: 1,
                redTeamId: 2,
                blueScore: 10,
                redScore: 5);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidOperationException>();

        _matchRepositoryMock.Verify(
            repository =>
                repository.AddAsync(
                    It.IsAny<Match>()),
            Times.Never);
    }

    [Fact]
    public async Task Valid_match_should_be_saved_once()
    {
        // Arrange
        SetupTeams();

        var externalId = Guid.NewGuid();

        // Act
        await _sut.CreateMatch(
            externalId,
            blueTeamId: 1,
            redTeamId: 2,
            blueScore: 10,
            redScore: 5);

        // Assert
        _matchRepositoryMock.Verify(
            repository =>
                repository.AddAsync(
                    It.IsAny<Match>()),
            Times.Once);
    }

    private void SetupTeams()
    {
        var blueTeam = CreateTeam(
            id: 1,
            attackerId: 1,
            defenderId: 2);

        var redTeam = CreateTeam(
            id: 2,
            attackerId: 3,
            defenderId: 4);

        _teamServiceMock
            .Setup(service =>
                service.GetTeam(1))
            .ReturnsAsync(blueTeam);

        _teamServiceMock
            .Setup(service =>
                service.GetTeam(2))
            .ReturnsAsync(redTeam);

        _matchRepositoryMock
            .Setup(repository =>
                repository.ExistsAsync(
                    It.IsAny<Guid>()))
            .ReturnsAsync(false);
    }

    private static Match CreateMatch(
        int blueScore,
        int redScore,
        int blueAttackerId = 1,
        int blueDefenderId = 2,
        int redAttackerId = 3,
        int redDefenderId = 4)
    {
        return new Match
        {
            BlueTeamId = 1,

            BlueTeam = CreateTeam(
                id: 1,
                attackerId: blueAttackerId,
                defenderId: blueDefenderId),

            RedTeamId = 2,

            RedTeam = CreateTeam(
                id: 2,
                attackerId: redAttackerId,
                defenderId: redDefenderId),

            BlueScore = blueScore,
            RedScore = redScore,

            State = MatchState.Created
        };
    }

    private static Team CreateTeam(
        int id,
        int attackerId,
        int defenderId)
    {
        return new Team
        {
            Id = id,

            AttackerId = attackerId,

            Attacker = new Player
            {
                Id = attackerId,
                EloRating = 1200
            },

            DefenderId = defenderId,

            Defender = new Player
            {
                Id = defenderId,
                EloRating = 1200
            }
        };
    }
}