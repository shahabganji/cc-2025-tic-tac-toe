using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using TicTacToe.GameListAsyncProjection.Models;

namespace TicTacToe.GameListAsyncProjection;

public class GameListAsyncSnapshotProjector(ILogger<GameListAsyncSnapshotProjector> logger, Container container)
{
    [Function("AsyncSnapshot")]
    public async Task Run([CosmosDBTrigger(
            databaseName: "CodeCrafts2025",
            containerName: "XOEvents",
            Connection = "DatabaseConnection",
            LeaseContainerName = "leases",
            CreateLeaseContainerIfNotExists = true,
            StartFromBeginning = true, MaxItemsPerInvocation = 100)]
        IReadOnlyList<JsonObject> input)
    {
        var gameList = await InitializeGameList();
        var gameListChanged = false;

        foreach (var eventJson in input)
        {
            if (eventJson["id"]!.ToString().StartsWith("EventStream"))
                continue;

            switch (eventJson["type"]?.ToString())
            {
                case "GameCreated":
                    var createdGame = eventJson["Event"].Deserialize<GameCreated>();
                    gameList.When(createdGame!);
                    gameListChanged = true;
                    continue;
                case "GameFinished":
                    var finishedGame = eventJson["Event"].Deserialize<GameFinished>();
                    gameList.When(finishedGame!);
                    gameListChanged = true;
                    continue;
                default:
                    logger.LogInformation("Ignore Event: {Event}",
                        eventJson["type"]?.ToString() ?? "Unknown Event Type");
                    continue;
            }
        }

        if (gameListChanged)
        {
            await container.UpsertItemAsync(gameList, new PartitionKey(nameof(GameListAsyncSnapshot)));
        }
    }

    private async Task<GameListAsyncSnapshot> InitializeGameList()
    {
        const string id = nameof(GameListAsyncSnapshot);
        var partitionKey = new PartitionKey(id);

        try
        {
            var response = await container.ReadItemAsync<GameListAsyncSnapshot>(id, partitionKey);
            return response.Resource;
        }
        catch
        {
            return new GameListAsyncSnapshot();
        }
    }
}
