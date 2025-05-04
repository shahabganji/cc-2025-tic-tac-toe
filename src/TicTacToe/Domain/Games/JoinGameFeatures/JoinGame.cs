namespace TicTacToe.Domain.Games.JoinGameFeatures;

public record struct JoinGame(Guid GameId, Guid PlayerId);

public sealed class JoinGameHandler(IEventStore store) : CommandHandler<JoinGame, string>(store)
{
    private readonly IEventStore _store = store;
    
    public override async Task<string> Handle(JoinGame command)
    {
        var gameStream = GetStream<Game>(command.GameId);
        var game = await gameStream.Get();

        if (game is null)
        {
            throw new InvalidOperationException("No such game is found");
        }
        
        var playerType = game.Join(command.PlayerId);
        
        _store.AppendToStream(game.Id, game.Changes);
        
        await _store.SaveStreamAsync();

        return playerType;
    }
}
