namespace TicTacToe.Domain;

public interface IEventStore
{
    long Version { get; }

    Task<IReadOnlyCollection<IEvent>> LoadStreamEvents(Guid aggregateId, CancellationToken ct = default);
    void AppendToStream(Guid aggregateId, IReadOnlyCollection<IEvent> events);
    Task SaveStreamAsync();
}
