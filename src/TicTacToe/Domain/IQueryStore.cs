using TicTacToe.Web.Contracts;

namespace TicTacToe.Domain;

public interface IQueryStore
{
    Task<IEnumerable<GameInfo>> GetAvailableGames();
}
