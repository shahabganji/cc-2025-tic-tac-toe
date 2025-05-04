using TicTacToe.Domain.Games.CreateGameFeatures;
using TicTacToe.Domain.Games.FillCellFeatures;
using TicTacToe.Domain.Games.JoinGameFeatures;
using TicTacToe.Domain.Games.LoadGamesFeature;
using TicTacToe.Domain.Players.RegisterFeatures;

namespace TicTacToe.Domain;

internal static class DomainExtensions
{
    internal static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<FillCellHandler>();
        services.AddScoped<JoinGameHandler>();
        services.AddScoped<CreateGameHandler>();
        services.AddScoped<RegisterPlayerHandler>();
        
        return services;
    }
    
    internal static IServiceCollection AddQueryHandlers(this IServiceCollection services)
    {
        services.AddScoped<ShowListOfGamesHandler>();
        return services;
    }
}
