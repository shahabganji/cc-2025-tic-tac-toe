using TicTacToe.Domain.Games.Commands;

namespace TicTacToe.Domain;

internal static class DomainExtensions
{
    internal static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<CreateGameHandler>();
        
        return services;
    }
}
