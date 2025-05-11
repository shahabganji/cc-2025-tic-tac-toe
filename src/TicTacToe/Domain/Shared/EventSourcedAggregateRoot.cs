namespace TicTacToe.Domain.Shared;

public abstract class EventSourcedAggregateRoot
{
    private readonly List<IEvent> _changes = [];
    public IReadOnlyCollection<IEvent> Changes => _changes.AsReadOnly();

    protected void Apply(IEvent e)
    {
        _changes.Add(e);
        Mutate(e);
    }

    public void Mutate(IEvent e)
    {
        ((dynamic)this).When((dynamic)e);
    }
}
