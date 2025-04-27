using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos;
using TicTacToe.Domain;

namespace TicTacToe.Stores;

internal sealed class CosmosEventStore(Container container) : IEventStore
{
    private sealed record CosmosStoredEvent(Guid StreamId, long Version, DateTimeOffset Timestamp, IEvent EventData)
        : StoredEvent(StreamId, Version, EventData)
    {
        [JsonPropertyName("id")] public string Id => Timestamp.ToString("0");
        [JsonPropertyName("_ts")] public string Ts => Timestamp.ToString();
        [JsonPropertyName("pk")] public string Pk => StreamId.ToString();
        [JsonPropertyName("type")] public string Type => EventData.GetType().Name;
    }

    private readonly Container _container = container;
    private readonly Dictionary<Guid, List<CosmosStoredEvent>> _streams = new();

    public void AppendToStream(Guid aggregateId, long expectedVersion, IReadOnlyCollection<IEvent> events)
    {
        foreach (var @event in events)
        {
            if (_streams.TryGetValue(aggregateId, out var storedEvents))
            {
                storedEvents.Add(new CosmosStoredEvent(aggregateId, expectedVersion, DateTimeOffset.Now, @event));
            }
            else
            {
                _streams.Add(aggregateId,
                    [new CosmosStoredEvent(aggregateId, expectedVersion, DateTimeOffset.Now, @event)]);
            }
        }
    }

    public async Task<IReadOnlyCollection<IEvent>> LoadStreamEvents(Guid streamId, CancellationToken ct = default)
    {
        var streamIterator =
            _container.GetItemQueryIterator<StoredEvent>(
                new QueryDefinition($"SELECT * FROM c WHERE c.StreamId = '{streamId}' AND c.id <> '{streamId}'"));

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

        return events.Select(x => x.Event).ToList();
    }

    public async Task SaveStreamAsync()
    {
        foreach (var (key, events) in _streams)
        {
            var streamId = key.ToString();
            var transactionalBatch = _container.CreateTransactionalBatch(new PartitionKey(streamId));

            foreach (var storedEvent in events)
            {
                transactionalBatch.UpsertItem(storedEvent);
                // await _container.UpsertItemAsync<Event>(@event, new PartitionKey(@event.StreamId.ToString()));   
            }

            _ = await transactionalBatch.ExecuteAsync(CancellationToken.None);
        }
    }

    // /// <summary>
    // /// Gets the snapshot using Point Read mechanism from ACD SDK
    // /// </summary>
    // /// <param name="streamId">The unique identifier of the stream</param>
    // /// <returns></returns>
    // public async Task<Maybe<TA>> GetSnapshot<TA>(Guid streamId) where TA : IAmAggregateRoot, new()
    // {
    //     try
    //     {
    //         var snapshot = await _container.ReadItemAsync<TA>(
    //             id: streamId.ToString(), new PartitionKey(streamId.ToString()), new ItemRequestOptions
    //             {
    //                 EnableContentResponseOnWrite = false,
    //             });
    //
    //         return snapshot.Resource;
    //     }
    //     catch
    //     {
    //         throw new InvalidOperationException("No stream found");
    //     }
    // }
}
