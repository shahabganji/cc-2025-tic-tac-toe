using TicTacToe.Domain.Shared;

namespace TicTacToe.Domain.Games.Events;

public record struct GameCreated(Guid Id, string Name) : IEvent;
public record struct PlayerJoined(Guid PlayerId) : IEvent;
public record struct CellFilled(Guid PlayerId, int Cell): IEvent;
public record struct GameFinished(Guid GameId, Guid? WinnerId, Guid? LoserId): IEvent;
