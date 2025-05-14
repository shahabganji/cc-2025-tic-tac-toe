using TicTacToe.Features.Players.RegisterFeatures;

namespace TicTacToe.Features.Players;

internal static class PlayerServiceCollectionExtensions
{
    internal static IServiceCollection AddPlayerFeatures(this IServiceCollection services)
    {
        services.AddScoped<RegisterPlayerHandler>();
        
        return services;
    }

}
