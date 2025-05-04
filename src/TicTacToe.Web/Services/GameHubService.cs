using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;

namespace TicTacToe.Web.Services;

internal sealed class GameHubService(IWebAssemblyHostEnvironment host)
{
    private HubConnection HubConnection { get; set; } = null!;

    private Func<Guid, string, Task>? _onGameCreated;
    public void OnGameCreated(Func<Guid, string, Task> handler) => _onGameCreated = handler;
    
    private Func<Guid, Guid, string, Task>? _onPlayerJoined;
    public void OnPlayerJoined(Func<Guid, Guid, string, Task> handler) => _onPlayerJoined = handler;
    
    
    private Func<Guid, Guid, int, Task>? _onCellFilled;
    public void OnCellFilled(Func<Guid, Guid, int, Task> handler) => _onCellFilled = handler;
    

    public async Task SetupConnection()
    {
        var hubsUri = new Uri($"{host.BaseAddress}hubs");

        HubConnection = new HubConnectionBuilder()
            .WithUrl(hubsUri)
            .Build();

        HubConnection.On("GameCreated", async (Guid gameId, string gameName) =>
        {
            if (_onGameCreated is not null)
            {
                await _onGameCreated.Invoke(gameId, gameName);
            }
        });

        HubConnection.On("PlayerJoined", async (Guid gameId, Guid playerId, string playerType) =>
        {
            if (_onPlayerJoined is not null)
            {
                await _onPlayerJoined.Invoke(gameId, playerId, playerType);
            }
        });
        
        HubConnection.On("CellFilled", async (Guid gameId, Guid playerId, int cell) =>
        {
            if (_onCellFilled is not null)
            {
                await _onCellFilled.Invoke(gameId, playerId, cell);
            }
        });

        await HubConnection.StartAsync();
    }

    public async Task JoinGame(Guid gameId, Guid playerId)
    {
        await HubConnection.InvokeCoreAsync("JoinGame",
        [
            new
            {
                GameId = gameId,
                PlayerId = playerId
            }
        ]);
    }
}
