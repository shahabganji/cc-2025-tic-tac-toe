using TicTacToe.Domain.Games.CreateGameFeatures;
using TicTacToe.Domain.Games.JoinGameFeatures;
using TicTacToe.Domain.Players.RegisterFeatures;

namespace TicTacToe.Domain;

internal static class DomainExtensions
{
    internal static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<JoinGameHandler>();
        services.AddScoped<CreateGameHandler>();
        services.AddScoped<RegisterPlayerHandler>();
        
        return services;
    }
}
