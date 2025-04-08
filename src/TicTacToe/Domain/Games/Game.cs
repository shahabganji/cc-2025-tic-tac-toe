using TicTacToe.Domain.Games.Events;

namespace TicTacToe.Domain.Games;

public sealed class Game : AggregateRoot
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;

    public static Game Create(Guid commandId, string suggestedName)
    {
        var game = new Game();
        var gameCreated = new GameCreated(commandId, suggestedName);
        game.Apply(gameCreated);
        
        return game;
    }

    public void When(GameCreated gameCreated)
    {
        Id = gameCreated.Id;
        Name = gameCreated.Name;
    }
}
