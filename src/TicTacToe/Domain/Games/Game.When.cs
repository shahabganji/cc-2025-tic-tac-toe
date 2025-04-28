using TicTacToe.Domain.Games.Events;

namespace TicTacToe.Domain.Games;

public sealed partial class Game
{
    public void When(GameCreated gameCreated)
    {
        Id = gameCreated.Id;
        Name = gameCreated.Name;

        for (var i = 0; i < _boardCells.Length; i++)
        {
            _boardCells[i] = EmptyCell;
        }
    }

    public void When(PlayerJoined joinedPlayer)
    {
        if (XPlayer is not null && OPlayer is not null)
        {
            throw new InvalidOperationException("Game is full");
        }

        if (XPlayer is null)
        {
            XPlayer = joinedPlayer.PlayerId;
            CurrentPlayer = XPlayer;
            return;
        }

        OPlayer = joinedPlayer.PlayerId;
    }

    public void When(CellFilled playedSquare)
    {
        _boardCells[playedSquare.Cell] = playedSquare.PlayerId == XPlayer ? XCell : OCell;

        CurrentPlayer = playedSquare.PlayerId == XPlayer ? OPlayer : XPlayer;
    }

    public void When(GameFinished finishedGame)
    {
        Winner = finishedGame.WinnerId;
        Loser = finishedGame.LoserId;
    }
}
