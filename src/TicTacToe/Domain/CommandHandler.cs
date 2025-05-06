namespace TicTacToe.Domain;

public abstract class CommandHandler<TCommand>(IEventStore eventStore)
{
    protected EventStream<TEntity> GetStream<TEntity>(Guid aggregateId) where TEntity : AggregateRoot, new()
    {
        return new EventStream<TEntity>(aggregateId, eventStore);
    }

    public abstract Task Handle(TCommand command);
}


public abstract class CommandHandler<TCommand, TResponse>(IEventStore eventStore)
{
    protected EventStream<TEntity> GetStream<TEntity>(Guid aggregateId) where TEntity : AggregateRoot, new()
    {
        return new EventStream<TEntity>(aggregateId, eventStore);
    }

    public abstract Task<TResponse> Handle(TCommand request);
}
