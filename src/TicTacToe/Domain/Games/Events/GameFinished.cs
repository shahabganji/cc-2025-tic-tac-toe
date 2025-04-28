namespace TicTacToe.Domain.Games.Events;

public record struct GameFinished(Guid? WinnerId, Guid? LoserId): IEvent;