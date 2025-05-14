using TicTacToe.Features.Games.CreateGameFeatures;
using TicTacToe.Features.Games.FillCellFeatures;
using TicTacToe.Features.Games.GetGameEventsFeature;
using TicTacToe.Features.Games.JoinGameFeatures;
using TicTacToe.Features.Games.LoadGamesFeature;
using TicTacToe.Features.Games.LoadGameStateFeature;

namespace TicTacToe.Features.Games;

internal static class GameServiceCollectionExtensions
{
    internal static IServiceCollection AddGameFeatures(this IServiceCollection services)
    {
        services.AddScoped<FillCellHandler>();
        services.AddScoped<JoinGameHandler>();
        services.AddScoped<CreateGameHandler>();
        
        services.AddScoped<GetGameEventsHandler>();
        services.AddScoped<LoadGameStateHandler>();
        services.AddScoped<ShowListOfGamesHandler>();
        
        return services;
    }
}
