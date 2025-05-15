namespace TicTacToe.Domain.Shared;

public sealed class EventStream<T>(Guid streamId, IEventStore store) where T : EventSourcedAggregateRoot, new()
{
    public long Version => store.Version;

    /// <summary>
    /// Retrieves an instance of the aggregate root of type <typeparamref name="T"/> by loading its
    /// event stream and applying them to the aggregate. Optionally, unknown events can be ignored.
    /// </summary>
    /// <param name="ignoreUnknownEvents">
    /// If set to true, any unknown events in the stream will be ignored during the reconstruction of the aggregate.
    /// If false, encountering an unknown event will throw an exception.
    /// </param>
    /// <returns>
    /// The reconstructed aggregate root of type <typeparamref name="T"/>, or null if no events were found for the stream.
    /// </returns>
    public async Task<T?> Get(bool ignoreUnknownEvents = false)
    {
        throw new NotImplementedException();
    }
}
