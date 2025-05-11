using TicTacToe.Domain.Shared;

namespace TicTacToe.Domain.Players.Events;

public record struct PlayerRegistered(Guid Id, string Name) : IEvent;
