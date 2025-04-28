namespace TicTacToe.Domain.Games.Events;

public record struct CellFilled(Guid PlayerId, int Cell): IEvent;
public record struct GameFinished(Guid? WinnerId, Guid? LoserId): IEvent;
