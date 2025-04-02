using TicTacToe.Domain;

namespace TicTacToe.Games.Events;

public record struct GameCreated(Guid Id, string Name) : IEvent;
