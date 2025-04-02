namespace TicTacToe.Domain;

public interface IEvent;

public interface IEventStore
{
    Task<IReadOnlyCollection<IEvent>> LoadStreamEvents(Guid aggregateId, CancellationToken ct = default);
    Task AppendToStream(Guid aggregateId, long expectedVersion, IReadOnlyCollection<IEvent> events);
    Task SaveStreamAsync();
}
