namespace TicTacToe.Domain.Games.Events;

public record struct CellFilled(Guid PlayerId, int Cell): IEvent;
