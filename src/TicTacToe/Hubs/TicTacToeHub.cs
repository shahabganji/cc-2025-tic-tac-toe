using Microsoft.AspNetCore.SignalR;
using TicTacToe.Domain.Games.JoinGameFeatures;

namespace TicTacToe.Hubs;

public sealed class TicTacToeHub(JoinGameHandler handler) : Hub<ITicTacToeClient>
{
    public async Task JoinGame(JoinGame joinGame)
    {
        var playerType = await handler.Handle(joinGame);
        await Groups.AddToGroupAsync(Context.ConnectionId, joinGame.GameId.ToString());
        await Clients.Caller.PlayerJoined(joinGame.GameId, joinGame.PlayerId, playerType);
    }
}

public interface ITicTacToeClient
{
    Task GameCreated(Guid gameId, string gameName);
    Task CellFilled(Guid gameId, Guid playerId, long cell);
    Task PlayerJoined(Guid gameId, Guid playerId, string playerType);
}
