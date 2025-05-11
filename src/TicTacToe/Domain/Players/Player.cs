using TicTacToe.Domain.Players.Events;
using TicTacToe.Domain.Shared;

namespace TicTacToe.Domain.Players;

public sealed class Player : EventSourcedAggregateRoot
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;

    public static Player Create(Guid commandId, string suggestedName)
    {
        var player = new Player();
        var registered = new PlayerRegistered(commandId, suggestedName);
        player.Apply(registered);

        return player;
    }

    public void When(PlayerRegistered registration)
    {
        Id = registration.Id;
        Name = registration.Name;
    }
}
