using TicTacToe.Domain;

namespace TicTacToe.Stores.InMemory;

public sealed class InMemoryEventStore : IEventStore
{
    private readonly List<StoredEvent> _previousEvents = [];
    private readonly List<StoredEvent> _newEvents = [];

    public long Version => _newEvents.Count + _previousEvents.Count;

    public Task<IReadOnlyCollection<IEvent>> LoadStreamEvents(Guid aggregateId, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyCollection<IEvent>>(
            _previousEvents.Where(e => e.Id == aggregateId).Select(e => e.Event).ToList());

    public void AppendToStream(Guid aggregateId, long expectedVersion, IReadOnlyCollection<IEvent> events)
    {
        _newEvents.AddRange(events.Select(e => new StoredEvent(aggregateId, expectedVersion, e)));
    }

    public Task SaveStreamAsync()
    {
        return Task.CompletedTask;
    }
}
