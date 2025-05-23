using TicTacToe.Domain;
using TicTacToe.Domain.Shared;

namespace TicTacToe.Stores.InMemory;

public sealed class InMemoryEventStore : IEventStore
{
    private readonly List<StoredEvent> _previousEvents = [];
    private readonly List<StoredEvent> _newEvents = [];

    public long Version => _newEvents.Count + _previousEvents.Count;

    public Task<IReadOnlyCollection<IEvent>> LoadStreamEvents(Guid aggregateId, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyCollection<IEvent>>(
            _previousEvents.Where(e => e.Id == aggregateId).Select(e => e.Event).ToList());

    public void AppendToStream(Guid aggregateId, IReadOnlyCollection<IEvent> events)
    {
        _newEvents.AddRange(events.Select(e => new StoredEvent(aggregateId, Version, e)));
    }

    public Task SaveStreamAsync(CancellationToken ct = default) => Task.CompletedTask;
}
