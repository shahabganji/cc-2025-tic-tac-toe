using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TicTacToe.GameListAsyncProjection.Internals;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddSingleton(_ =>
        {
            var connectionString = Environment.GetEnvironmentVariable("DatabaseConnection");
            var cosmosClient = new CosmosClient(connectionString, new CosmosClientOptions
            {
                Serializer = new CosmosSystemTextJsonSerializer(),
            });
            
            var database = cosmosClient.GetDatabase("CodeCrafts2025");
            var container = database.GetContainer("XOEvents");

            return container;
        });
    })
    .Build();

host.Run();

