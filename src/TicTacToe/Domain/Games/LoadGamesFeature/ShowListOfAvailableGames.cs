using TicTacToe.Web.Contracts;

namespace TicTacToe.Domain.Games.LoadGamesFeature;

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
