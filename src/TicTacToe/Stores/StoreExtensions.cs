using TicTacToe.Domain;

namespace TicTacToe.Stores;

internal static class StoreExtensions
{
    internal static IServiceCollection AddEventStore(this IServiceCollection services)
    {
        services.AddScoped<IEventStore, InMemoryEventStore>();
        
        return services;
    }
}
