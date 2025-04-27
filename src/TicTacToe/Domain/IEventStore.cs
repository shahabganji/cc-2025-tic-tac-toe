namespace TicTacToe.Domain;

public interface IEvent;

public interface IEventStore
{
    public long Version => 1;
    
    Task<IReadOnlyCollection<IEvent>> LoadStreamEvents(Guid aggregateId, CancellationToken ct = default);
    void AppendToStream(Guid aggregateId, long expectedVersion, IReadOnlyCollection<IEvent> events);
    Task SaveStreamAsync();
}
