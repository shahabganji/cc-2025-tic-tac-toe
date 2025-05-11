using TicTacToe.Domain;
using TicTacToe.Domain.Games.Events;
using TicTacToe.Domain.Shared;
using TicTacToe.Web.Contracts;

namespace TicTacToe.Features.Games.GetGameEventsFeature;

public sealed record GetGameEvents(Guid GameId);

public sealed class GetGameEventsHandler(IQueryStore store)
{
    public async Task<IEnumerable<EventsInfo>> Query(GetGameEvents query)
    {
        var games = await store.GetEventStream(query.GameId);


        return games.Select(g => new EventsInfo
        {
            EventId = g.Id,
            EventName = g.Event.GetType().Name,
            DisplayInfo = g.Event switch
            {
                CellFilled cellFilled => $"{cellFilled.Cell}",
                GameCreated gameCreated => $"Game `{gameCreated.Name}` created",
                GameFinished gameFinished =>
                    $"Game finished, Winner: {gameFinished.WinnerId}, Loser: {gameFinished.LoserId}",
                PlayerJoined playerJoined => $"Player joined",
                _ => "Not Known Event"
            }
        }).Reverse();
    }
}
