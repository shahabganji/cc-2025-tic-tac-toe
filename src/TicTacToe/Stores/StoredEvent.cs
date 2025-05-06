using TicTacToe.Domain;

namespace TicTacToe.Stores;

public record StoredEvent(Guid Id, long Version, IEvent Event);
