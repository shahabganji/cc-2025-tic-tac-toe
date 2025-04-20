namespace TicTacToe.Domain.Games.Events;

public record struct PlayerJoined(Guid PlayerId) : IEvent;
