using TicTacToe.Features.Players.RegisterFeatures;

namespace TicTacToe.Features.Players;

internal static class PlayersExtensions
{
    internal static IServiceCollection AddPlayerFeatures(this IServiceCollection services)
    {
        services.AddScoped<RegisterPlayerHandler>();
        
        return services;
    }

}
