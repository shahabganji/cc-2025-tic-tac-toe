namespace TicTacToe.Domain.Shared;

public abstract class EventSourcedAggregateRoot
{
    private readonly List<IEvent> _changes = [];
    public IReadOnlyCollection<IEvent> Changes => _changes.AsReadOnly();

    protected void Apply(IEvent @event)
    {
        _changes.Add(@event);
        When(@event);
    }

    public void When(IEvent e)
    {
        ((dynamic)this).When((dynamic)e);
    }
}
