using Microsoft.AspNetCore.SignalR;

namespace TicTacToe.Hubs;

public sealed class TicTacToeHub : Hub<ITicTacToeClient>
{
    public async Task InformClientsOfGameCreated(Guid gameId, string gameName)
    {
        await Clients.Others.GameCreated(gameId, gameName);
    }
}

public interface ITicTacToeClient
{
    Task GameCreated(Guid gameId, string gameName);
}
