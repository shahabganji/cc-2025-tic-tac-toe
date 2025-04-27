namespace TicTacToe.Domain.Games.JoinGameFeatures;

public record struct JoinGame(Guid GameId, Guid PlayerId);

public sealed class JoinGameHandler(IEventStore store) : CommandHandler<JoinGame>(store)
{
    private readonly IEventStore _store = store;
    
    public override async Task Handle(JoinGame command)
    {
        var gameStream = GetStream<Game>(command.GameId);
        var game = await gameStream.Get();

        if (game is null)
        {
            throw new InvalidOperationException("No such game is found");
        }
        
        game.Join(command.PlayerId);
        
        _store.AppendToStream(game.Id, _store.Version, game.Changes);
        
        await _store.SaveStreamAsync();
    }
}
