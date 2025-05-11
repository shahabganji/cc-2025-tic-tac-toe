using TicTacToe.Features.Games.LoadGamesFeature;
using TicTacToe.Stores;

namespace TicTacToe.Domain.Shared;

public interface IQueryStore
{
    Task<ListOfAvailableGames> GetAvailableGames(CancellationToken ct = default);
    Task<IEnumerable<StoredEvent>> GetEventStream(Guid streamId, CancellationToken ct = default);
}
