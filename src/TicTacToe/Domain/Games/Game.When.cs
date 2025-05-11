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
        if (_xPlayer is not null && _oPlayer is not null)
        {
            throw new InvalidOperationException("Game is full");
        }

        if (_xPlayer is null)
        {
            _xPlayer = joinedPlayer.PlayerId;
            _currentPlayer = _xPlayer;
            return;
        }

        _oPlayer = joinedPlayer.PlayerId;
    }

    public void When(CellFilled playedSquare)
    {
        _boardCells[playedSquare.Cell] = playedSquare.PlayerId == _xPlayer ? XCell : OCell;

        _currentPlayer = playedSquare.PlayerId == _xPlayer ? _oPlayer : _xPlayer;
    }

    public void When(GameFinished finishedGame)
    {
        _isGameFinished = true;
    }
}
