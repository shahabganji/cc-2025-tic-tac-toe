using TicTacToe.Domain;
using TicTacToe.Domain.Shared;

namespace TicTacToe.Stores;

public record StoredEvent(Guid Id, long Version, IEvent Event);
