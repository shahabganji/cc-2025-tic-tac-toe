namespace TicTacToe.Domain;

public sealed class EventStream<T>(Guid streamId, IEventStore store) where T : AggregateRoot, new()
{
    public long Version { get; private set; } 
    
    public async Task<T?> Get()
    {
        var events = await store.LoadStreamEvents(streamId);

        if (events.Count == 0)
            return null;

        var instance = new T();

        foreach (var e in events)
        {
            instance.Mutate(e);
            Version++;
        }
        
        return instance;
    } 
    
}
