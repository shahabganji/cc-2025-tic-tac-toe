namespace TicTacToe.Domain;

public sealed class EventStream<T>(Guid streamId, IEventStore store) where T : AggregateRoot, new()
{

    public long Version => store.Version;
    public async Task<T?> Get()
    {
        var events = await store.LoadStreamEvents(streamId);

        if (events.Count == 0)
            return null;

        var instance = new T();

        foreach (var e in events)
        {
            instance.Mutate(e);
        }
        
        return instance;
    }
}
