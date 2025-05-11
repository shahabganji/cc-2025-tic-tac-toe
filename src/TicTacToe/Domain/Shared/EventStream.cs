namespace TicTacToe.Domain.Shared;

public sealed class EventStream<T>(Guid streamId, IEventStore store) where T : EventSourcedAggregateRoot, new()
{
    public long Version => store.Version;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ignoreUnknownEvents">This helps
    /// to create aggregates that are based on the same stream
    /// but ignoring some of the events that are not relevant to them</param>
    /// <returns></returns>
    public async Task<T?> Get(bool ignoreUnknownEvents = false)
    {
        var events = await store.LoadStreamEvents(streamId);

        if (events.Count == 0)
            return null;

        var instance = new T();

        foreach (var e in events)
        {
            try
            {
                instance.When(e);
            }
            catch
            {
                if (!ignoreUnknownEvents)
                    throw;
            }
        }

        return instance;
    }
}
