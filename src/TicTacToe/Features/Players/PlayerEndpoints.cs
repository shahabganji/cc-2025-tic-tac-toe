using TicTacToe.Features.Players.RegisterFeatures;
using static TicTacToe.Features.Players.PlayerEndpoints.Handlers;

namespace TicTacToe.Features.Players;

internal static class PlayerEndpoints
{
    public static void RegisterPlayerEndpoints(this WebApplication webApplication)
    {
        webApplication.MapPost("/player/register", RegisterPlayer)
            .WithName("RegisterPlayer");
    }
    
    internal static class Handlers
    {
        internal static async Task<IResult> RegisterPlayer(RegisterPlayer command, RegisterPlayerHandler handler)
        {
            command.Id = Guid.CreateVersion7();
            await handler.Handle(command);
            return Results.Created("/player/{id}", command.Id);
        }
    }
}
