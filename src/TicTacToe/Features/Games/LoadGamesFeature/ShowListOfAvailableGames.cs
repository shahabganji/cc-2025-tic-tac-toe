using TicTacToe.Domain;
using TicTacToe.Domain.Shared;
using TicTacToe.Web.Contracts;

namespace TicTacToe.Features.Games.LoadGamesFeature;

public record ShowListOfAvailableGames();

public record ListOfAvailableGames
{
    public List<GameInfo> AvailableGames { get; set; } = [];
}

public sealed class ShowListOfGamesHandler(IQueryStore store)
{
    public async Task<IEnumerable<GameInfo>> Query(ShowListOfAvailableGames query)
    {
        var games = await store.GetAvailableGames();
        return games.AvailableGames;
    }
}
