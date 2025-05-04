using TicTacToe.Domain;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Core;
using TicTacToe.Domain.Games.LoadGamesFeature;
using TicTacToe.Web.Contracts;


namespace TicTacToe.Stores.CosmosDb;

internal sealed partial class CosmosEventStore : IQueryStore
{
    public async Task<ListOfAvailableGames> GetAvailableGames()
    {
        const string id = "GameListAsyncSnapshot";
        var partitionKey = new PartitionKey(id);

        try
        {
            var response = await container.ReadItemAsync<ListOfAvailableGames>(id, partitionKey);
            return response.Resource;
        }
        catch
        {
            return new ListOfAvailableGames();
        }
    }
}
