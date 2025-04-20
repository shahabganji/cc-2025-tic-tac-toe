using TicTacToe.Domain;

namespace TicTacToe.Specifications.Helpers;

internal sealed record StoredEvent(Guid Id, long Version, IEvent Event);

public sealed class InMemoryEventStore : IEventStore
{
    internal List<StoredEvent> previousEvents = [];
    internal List<StoredEvent> newEvents = [];

    public Task<IReadOnlyCollection<IEvent>> LoadStreamEvents(Guid aggregateId, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyCollection<IEvent>>(
            previousEvents.Where(e => e.Id == aggregateId).Select(e => e.Event).ToList());

    public Task AppendToStream(Guid aggregateId, long expectedVersion, IReadOnlyCollection<IEvent> events)
    {
        newEvents.AddRange(events.Select(e => new StoredEvent(aggregateId, expectedVersion, e)));
        return Task.CompletedTask;
    }

    public Task SaveStreamAsync()
    {
        return Task.CompletedTask;
    }
}
