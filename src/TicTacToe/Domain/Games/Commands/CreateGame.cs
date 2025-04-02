namespace TicTacToe.Domain.Games.Commands;

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
            await _store.AppendToStream(game.Id, 0, game.Changes);
        }
    }
}
