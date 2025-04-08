namespace TicTacToe.Domain.Games.Events;

public record struct GameCreated(Guid Id, string Name) : IEvent;
