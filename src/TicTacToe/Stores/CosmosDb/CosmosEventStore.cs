using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos;
using TicTacToe.Domain;

namespace TicTacToe.Stores.CosmosDb;

internal sealed record CosmosEventStream(string StreamId)
{
    internal static string GetId(string streamId) => $"EventStream-{streamId}";

    public long Version { get; set; }
    
    [JsonPropertyName("id")] public string Id => GetId(StreamId);

    [JsonPropertyName("_etag")] public string Etag { get; set; }
};

internal sealed record CosmosStoredEvent(Guid StreamId, long Version, DateTimeOffset Timestamp, IEvent Event)
    : StoredEvent(StreamId, Version, Event)
{
    [JsonPropertyName("id")] public string Id => Timestamp.ToUnixTimeMilliseconds().ToString();
    [JsonPropertyName("_ts")] public string Ts => Timestamp.ToString();
    [JsonPropertyName("pk")] public string Pk => StreamId.ToString();
    [JsonPropertyName("type")] public string Type => Event.GetType().Name;
}

internal sealed class CosmosEventStore(Container container) : IEventStore
{
    private readonly Container _container = container;

    private CosmosEventStream? _activeStream;
    private readonly Dictionary<Guid, List<CosmosStoredEvent>> _events = new();

    public long Version => _activeStream?.Version ?? 0;


    public void AppendToStream(Guid aggregateId, long expectedVersion, IReadOnlyCollection<IEvent> events)
    {
        _activeStream ??= new CosmosEventStream(aggregateId.ToString());

        foreach (var @event in events)
        {
            _activeStream.Version++;
            if (_events.TryGetValue(aggregateId, out var storedEvents))
            {
                storedEvents.Add(new CosmosStoredEvent(aggregateId, _activeStream.Version, DateTimeOffset.Now, @event));
            }
            else
            {
                _events.Add(aggregateId,
                    [new CosmosStoredEvent(aggregateId, _activeStream.Version, DateTimeOffset.Now, @event)]);
            }
        }
    }

    public async Task<IReadOnlyCollection<IEvent>> LoadStreamEvents(Guid streamId, CancellationToken ct = default)
    {
        await InitializeActiveStream(streamId);

        
        var streamIterator =
            _container.GetItemQueryIterator<CosmosStoredEvent>(
                new QueryDefinition($"SELECT * FROM c WHERE c.StreamId = '{streamId}' AND c.id <> '{_activeStream.Id}'"));

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

        _activeStream.Version = events.Count > 0 ? events.Max(x => x.Version) : 0;
        return events.Select(x => x.Event).ToList();
    }

    [MemberNotNull(nameof(_activeStream))]
    private async Task InitializeActiveStream(Guid streamId)
    {
        var id = CosmosEventStream.GetId(streamId.ToString());
        var partitionKey = new PartitionKey(streamId.ToString());

        try
        {
            var response = await _container.ReadItemAsync<CosmosEventStream>(id, partitionKey);
            _activeStream = response.Resource;
        }
        catch
        {
            _activeStream = new CosmosEventStream(streamId.ToString());
        }
    }

    public async Task SaveStreamAsync()
    {
        if (_activeStream is null)
        {
            throw new InvalidOperationException("No active stream");
        }

        var streamId = _activeStream.StreamId;
        var transactionalBatch = _container.CreateTransactionalBatch(new PartitionKey(streamId));

        transactionalBatch.UpsertItem(_activeStream, new TransactionalBatchItemRequestOptions
        {
            IfMatchEtag = _activeStream.Etag,
            EnableContentResponseOnWrite = false
        });

        foreach (var (_, events) in _events)
        {
            foreach (var storedEvent in events)
            {
                transactionalBatch.UpsertItem(storedEvent);
            }

            var response = await transactionalBatch.ExecuteAsync(CancellationToken.None);

            if (!response.IsSuccessStatusCode && response[0].StatusCode == HttpStatusCode.PreconditionFailed)
            {
                throw new InvalidOperationException("Concurrency error");
            }
        }
    }
}
