using Shouldly;
using TicTacToe.Domain;
using TicTacToe.Domain.Shared;

namespace TicTacToe.Specifications.Helpers;

public abstract class CommandHandlerHelper<TCommand>
{
    protected readonly Guid AggregateId = Guid.CreateVersion7();
    protected readonly InMemoryEventStore EventStore = new();
    
    protected abstract CommandHandler<TCommand> Handler { get; }

    protected void Given(params IEvent[] events)
    {
        Given(AggregateId, events);
    }

    protected void Given(Guid aggregateId, params IEvent[] events)
    {
        EventStore.PreviousEvents.AddRange(events.Select((e,i) => new StoredEvent(aggregateId, i, e)));
    }

    protected Task When(TCommand command) => Handler.Handle(command);

    protected void Then(params IEvent[] expectedEvents)
    {
        Then(AggregateId, expectedEvents);
    }

    protected void Then(Guid aggregateId, params IEvent[] expectedEvents)
    {
        var actualEvents = EventStore.NewEvents
            .Where(e => e.Id == aggregateId)
            .OrderBy(e => e.Version)
            .Select(e => e.Event)
            .ToArray();

        actualEvents.Length.ShouldBe(expectedEvents.Length);

        for (var i = 0; i < actualEvents.Length; i++)
        {
            actualEvents[i].ShouldBeOfType(expectedEvents[i].GetType());
            try
            {
                actualEvents[i].ShouldBeEquivalentTo(expectedEvents[i]);
            }
            catch (InvalidOperationException e)
            {
                // Empty event with matching type is OK. This means that the event class
                // has no properties. If the types match in this situation, the correct
                // event has been appended. So we should ignore this exception.
                if (!e.Message.StartsWith("No members were found for comparison."))
                    throw;
            }
        }
    }
}

public abstract class CommandHandlerHelper<TCommand, TResponse>
{
    protected readonly Guid AggregateId = Guid.CreateVersion7();
    protected readonly InMemoryEventStore EventStore = new();

    protected abstract CommandHandler<TCommand, TResponse> Handler { get; }

    protected void Given(params IEvent[] events)
    {
        Given(AggregateId, events);
    }

    protected void Given(Guid aggregateId, params IEvent[] events)
    {
        EventStore.PreviousEvents.AddRange(events.Select((e,i) => new StoredEvent(aggregateId, i, e)));
    }

    protected Task<TResponse> When(TCommand command) => Handler.Handle(command);

    protected void Then(params IEvent[] expectedEvents)
    {
        Then(AggregateId, expectedEvents);
    }

    protected void Then(Guid aggregateId, params IEvent[] expectedEvents)
    {
        var actualEvents = EventStore.NewEvents
            .Where(e => e.Id == aggregateId)
            .OrderBy(e => e.Version)
            .Select(e => e.Event)
            .ToArray();

        actualEvents.Length.ShouldBe(expectedEvents.Length);

        for (var i = 0; i < actualEvents.Length; i++)
        {
            actualEvents[i].ShouldBeOfType(expectedEvents[i].GetType());
            try
            {
                actualEvents[i].ShouldBeEquivalentTo(expectedEvents[i]);
            }
            catch (InvalidOperationException e)
            {
                // Empty event with matching type is OK. This means that the event class
                // has no properties. If the types match in this situation, the correct
                // event has been appended. So we should ignore this exception.
                if (!e.Message.StartsWith("No members were found for comparison."))
                    throw;
            }
        }
    }
}
