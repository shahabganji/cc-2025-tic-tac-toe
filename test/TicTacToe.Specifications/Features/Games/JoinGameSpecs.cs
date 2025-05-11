using TicTacToe.Domain.Games.Events;
using TicTacToe.Domain.Shared;
using TicTacToe.Features.Games.JoinGameFeatures;
using TicTacToe.Specifications.Helpers;

namespace TicTacToe.Specifications.Features.Games;

public sealed class JoinGameSpecifications : CommandHandlerHelper<JoinGame, string>
{
    private Guid GameId => AggregateId;

    private readonly Guid _xPlayer = Guid.NewGuid();
    private readonly Guid _oPlayer = Guid.NewGuid();
    private readonly Guid _thirdPlayer = Guid.NewGuid();

    protected override CommandHandler<JoinGame, string> Handler => new JoinGameHandler(EventStore);

    [Fact]
    public async Task Joining_an_empty_game_should_add_the_player_to_the_game()
    {
        Given(new GameCreated(GameId, "My Game"));

        var playerType = await When(new JoinGame(GameId, _xPlayer));

        Then(new PlayerJoined(_xPlayer));
        
        Assert.Equal("X", playerType);
    }

    [Fact]
    public async Task xPlayer_joining_the_game_should_be_idempotent()
    {
        Given(new GameCreated(GameId, "My Game"),
            new PlayerJoined(_xPlayer)
        );

        await When(new JoinGame(GameId, _xPlayer));
        
        Then();
    }
    
    [Fact]
    public async Task oPlayer_joining_the_game_should_be_idempotent()
    {
        Given(new GameCreated(GameId, "My Game"),
            new PlayerJoined(_xPlayer),
            new PlayerJoined(_oPlayer)
        );

        await When(new JoinGame(GameId, _oPlayer));
        
        Then();
    }


    [Fact]
    public async Task Joining_a_game_with_one_player_should_add_the_player_to_the_game()
    {
        Given(
            new GameCreated(GameId, "My Game"),
            new PlayerJoined(_xPlayer));

        var playerType =  await When(new JoinGame(GameId, _oPlayer));

        Then(new PlayerJoined(_oPlayer));
        
        Assert.Equal("O", playerType);

    }

    [Fact]
    public async Task Joining_a_full_game_should_fail()
    {
        Given(
            new GameCreated(GameId, "My Game"),
            new PlayerJoined(_xPlayer),
            new PlayerJoined(_oPlayer));

        await Assert.ThrowsAsync<InvalidOperationException>(() => When(new JoinGame(GameId, _thirdPlayer)));
    }
}
