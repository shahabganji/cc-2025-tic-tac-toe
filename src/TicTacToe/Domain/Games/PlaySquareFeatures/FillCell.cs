namespace TicTacToe.Domain.Games.PlaySquareFeatures;

public record struct FillCell(Guid GameId, Guid PlayerId, int Cell);

public sealed class FillCellHandler(IEventStore eventStore) : CommandHandler<FillCell>(eventStore)
{
    private readonly IEventStore _store = eventStore;
    
    public override async Task Handle(FillCell command)
    {
        var gameStream = GetStream<Game>(command.GameId);
        var game = await gameStream.Get();

        if (game is null)
        {
            throw new InvalidOperationException("No such game is found");
        }
        
        game.Play(command.PlayerId, command.Cell);
        
        _store.AppendToStream(game.Id, game.Changes);

        await _store.SaveStreamAsync();
    }
}
