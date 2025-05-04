using TicTacToe.Domain.Games.LoadGamesFeature;

namespace TicTacToe.Domain;

public interface IQueryStore
{
    Task<ListOfAvailableGames> GetAvailableGames();
}
