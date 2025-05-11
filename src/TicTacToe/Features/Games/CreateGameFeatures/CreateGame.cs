using TicTacToe.Domain;
using TicTacToe.Domain.Games;
using TicTacToe.Domain.Shared;

namespace TicTacToe.Features.Games.CreateGameFeatures;

public sealed record CreateGame(Guid Id, string SuggestedName);

public sealed class CreateGameHandler(IEventStore store) : CommandHandler<CreateGame>(store)
{
    private readonly IEventStore _store = store;

    public override async Task Handle(CreateGame command)
    {
        var gameStream = GetStream<Game>(command.Id);
        var game = await gameStream.Get();

        if (game is null)
        {
            game = Game.Create(command.Id, command.SuggestedName);
            _store.AppendToStream(game.Id,game.Changes);
            await _store.SaveStreamAsync();
        }
    }
}
