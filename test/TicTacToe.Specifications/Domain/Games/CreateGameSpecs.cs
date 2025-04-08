using TicTacToe.Domain;
using TicTacToe.Domain.Games.Commands;
using TicTacToe.Domain.Games.Events;
using TicTacToe.Specifications.Helpers;

namespace TicTacToe.Specifications.Domain.Games;

public sealed class GameSpecifications : CommandHandlerHelper<CreateGame>
{
    private Guid GameId => AggregateId;
    protected override CommandHandler<CreateGame> Handler => new CreateGameHandler(EventStore);

    
    [Fact]
    public void Creating_a_new_game_should_create_a_new_game()
    {
        Given();

        When(new CreateGame(GameId, "My Game"));

        Then(new GameCreated(GameId, "My Game"));
    }

    
    [Fact]
    public void Attempt_to_create_a_game_twice_should_be_idempotent()
    {
        Given(new GameCreated(GameId, "My Game"));

        When(new CreateGame(GameId, "My Game"));

        Then();
    }
}
