namespace TicTacToe.Domain.Games.PlaySquareFeatures;

public record struct PlaySquare(Guid GameId, Guid PlayerId, int Cell);

public sealed class PlaySquareHandler(IEventStore eventStore) : CommandHandler<PlaySquare>(eventStore)
{
    private readonly IEventStore _store = eventStore;
    
    public override async Task Handle(PlaySquare command)
    {
        var gameStream = GetStream<Game>(command.GameId);
        var game = await gameStream.Get();

        if (game is null)
        {
            throw new InvalidOperationException("No such game is found");
        }
        
        game.Play(command.PlayerId, command.Cell);
        
        await _store.AppendToStream(game.Id, _store.Version, game.Changes);
    }
}
