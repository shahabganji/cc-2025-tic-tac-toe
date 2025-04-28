namespace TicTacToe.Domain;

public interface IEventStore
{
    long Version { get; }

    void AppendToStream(Guid aggregateId, IReadOnlyCollection<IEvent> events);
    
    Task<IReadOnlyCollection<IEvent>> LoadStreamEvents(Guid aggregateId, CancellationToken ct = default);
    
    Task SaveStreamAsync(CancellationToken ct = default);
}
