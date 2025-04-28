using TicTacToe.Web.Contracts;

namespace TicTacToe.Domain.Games.LoadGamesFeature;

public record ShowListOfGames();


public sealed class ShowListOfGamesHandler(IQueryStore store)
{
    public async Task<IEnumerable<GameInfo>> Query(ShowListOfGames query)
    {
        var games = await store.GetAvailableGames();
        return games;
    }
}
