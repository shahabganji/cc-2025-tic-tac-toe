using TicTacToe.Domain;
using TicTacToe.Domain.Games.Events;
using TicTacToe.Domain.Games.JoinGameFeatures;
using TicTacToe.Specifications.Helpers;

namespace TicTacToe.Specifications.Domain.Games;

public sealed class JoinGameSpecifications : CommandHandlerHelper<JoinGame>
{
    private Guid GameId => AggregateId;
    
    private readonly Guid _xPlayer = Guid.NewGuid();
    private readonly Guid _oPlayer = Guid.NewGuid();

    protected override CommandHandler<JoinGame> Handler => new JoinGameHandler(EventStore);

    [Fact]
    public async Task Joining_an_empty_game_should_add_the_player_to_the_game()
    {
        Given(new GameCreated(GameId, "My Game"));

        await When(new JoinGame(GameId, _xPlayer));

        Then(new PlayerJoined(_xPlayer));
    }

    [Fact]
    public async Task Joining_a_game_with_one_player_should_add_the_player_to_the_game()
    {
        Given(
            new GameCreated(GameId, "My Game"),
            new PlayerJoined(_xPlayer));

        await When(new JoinGame(GameId, _oPlayer));

        Then(new PlayerJoined(_oPlayer));
    }

    [Fact]
    public async Task Joining_a_full_game_should_fail()
    {
        Given(
            new GameCreated(GameId, "My Game"),
            new PlayerJoined(_xPlayer),
            new PlayerJoined(_oPlayer));

        await Assert.ThrowsAsync<InvalidOperationException>(() => When(new JoinGame(GameId, _oPlayer)));
    }
}
