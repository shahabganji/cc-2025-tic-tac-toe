using TicTacToe.Domain;

namespace TicTacToe.Stores;

internal record StoredEvent(Guid Id, long Version, IEvent Event);
