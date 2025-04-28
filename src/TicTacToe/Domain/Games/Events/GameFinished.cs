namespace TicTacToe.Domain.Games.Events;

public record struct GameFinished(Guid GameId, Guid? WinnerId, Guid? LoserId): IEvent;
