using TicTacToe.Domain;
using TicTacToe.Domain.Shared;

namespace TicTacToe.Specifications.Helpers;

internal sealed record StoredEvent(Guid Id, long Version, IEvent Event);

public sealed class InMemoryEventStore : IEventStore
{
    internal readonly List<StoredEvent> PreviousEvents = [];
    internal readonly List<StoredEvent> NewEvents = [];

    public long Version => NewEvents.Count + PreviousEvents.Count;

    public Task<IReadOnlyCollection<IEvent>> LoadStreamEvents(Guid aggregateId, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyCollection<IEvent>>(
            PreviousEvents.Where(e => e.Id == aggregateId).Select(e => e.Event).ToList());

    public void AppendToStream(Guid aggregateId, IReadOnlyCollection<IEvent> events)
    {
        NewEvents.AddRange(events.Select(e => new StoredEvent(aggregateId, 1, e)));
    }

    public Task SaveStreamAsync(CancellationToken ct = default) => Task.CompletedTask;
}
