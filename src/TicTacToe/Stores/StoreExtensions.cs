using Microsoft.Azure.Cosmos;
using TicTacToe.Domain;
using TicTacToe.Stores.CosmosDb;
using TicTacToe.Stores.InMemory;

namespace TicTacToe.Stores;

internal static class StoreExtensions
{
    internal static IServiceCollection AddMemoryEventStore(this IServiceCollection services)
    {
        services.AddScoped<IEventStore, InMemoryEventStore>();

        return services;
    }

    public static void AddCosmosEventStore(this IServiceCollection services, string connectionString)
    {
        var cosmosClient = new CosmosClient(connectionString, new CosmosClientOptions
        {
            Serializer = new CosmosDb.Serializer.CosmosSystemTextJsonSerializer(),
        });

        var database = cosmosClient.GetDatabase("CodeCrafts2025");
        var container = database.GetContainer("XOEvents");

        services.AddScoped<IEventStore>(_ => new CosmosEventStore(container));
        services.AddScoped<IQueryStore>(_ => new CosmosEventStore(container));
    }
}
