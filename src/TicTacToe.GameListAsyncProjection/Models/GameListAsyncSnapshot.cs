namespace TicTacToe.GameListAsyncProjection.Models;

public record GameInfo(Guid Id, string Name);

public record GameCreated(Guid Id, string Name);

public record GameFinished(Guid GameId);

public sealed class GameListAsyncSnapshot
{
    public string id => nameof(GameListAsyncSnapshot);
    public string StreamId => nameof(GameListAsyncSnapshot);

    public List<GameInfo>? AvailableGames { get; set; }

    public void When(GameCreated gameCreated)
    {
        AvailableGames ??= [];

        AvailableGames.Add(new GameInfo(gameCreated.Id, gameCreated.Name));
    }

    public void When(GameFinished finishedGame)
    {
        AvailableGames ??= [];
        
        var game = AvailableGames.SingleOrDefault(x => x.Id == finishedGame.GameId);
        if (game is not null)
        {
            AvailableGames.Remove(game);
        }
    }
}
