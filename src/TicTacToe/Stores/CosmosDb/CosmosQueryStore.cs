using TicTacToe.Domain;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Core;
using TicTacToe.Domain.Games.LoadGamesFeature;
using TicTacToe.Web.Contracts;


namespace TicTacToe.Stores.CosmosDb;

internal sealed partial class CosmosEventStore : IQueryStore
{
    public async Task<ListOfAvailableGames> GetAvailableGames(CancellationToken ct = default)
    {
        const string id = "GameListAsyncSnapshot";
        var partitionKey = new PartitionKey(id);

        try
        {
            var response = await container.ReadItemAsync<ListOfAvailableGames>(id, partitionKey, cancellationToken: ct);
            return response.Resource;
        }
        catch
        {
            return new ListOfAvailableGames();
        }
    }

    public async Task<IEnumerable<StoredEvent>> GetEventStream(Guid streamId, CancellationToken ct = default)
    {
        var query = $"SELECT * FROM c WHERE c.StreamId = '{streamId}' AND c.id <> 'EventStream-{streamId}'";
        var streamIterator = _container.GetItemQueryIterator<CosmosStoredEvent>(new QueryDefinition(query));

        if (!streamIterator.HasMoreResults)
            return [];

        var events = new List<StoredEvent>();

        while (streamIterator.HasMoreResults)
        {
            var readNext = await streamIterator.ReadNextAsync(ct);

            if (readNext.Count == 0)
                continue;

            events.AddRange(readNext.Resource);
        }

        return events;
    }
}
