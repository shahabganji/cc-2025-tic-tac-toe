using TicTacToe.Domain.Games.Events;
using TicTacToe.Domain.Games.GetGameEventsFeature;
using TicTacToe.Web.Contracts;

namespace TicTacToe.Domain.Games.LoadGameStateFeature;

public sealed record LoadGameState(Guid GameId);

public sealed class LoadGameStateHandler(IEventStore store) : CommandHandler<LoadGameState, string[]>(store)
{
    private readonly IEventStore _store = store;

    public override async Task<string[]> Handle(LoadGameState request)
    {
        var stream = GetStream<Game>(request.GameId);
        var game = await stream.Get();

        return game.ToArray();
    }
}
