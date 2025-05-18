using TicTacToe.Domain.Games.Events;
using TicTacToe.Domain.Shared;

namespace TicTacToe.Domain.Games;

public sealed partial class Game : EventSourcedAggregateRoot
{
    private bool _isGameFinished;
    private Guid? _xPlayer;
    private Guid? _oPlayer;
    private Guid? _currentPlayer;
    
    private readonly Cell[] _boardCells = new Cell[9];

    private readonly int[][] _wins =
    [
        [0, 1, 2], [3, 4, 5], [6, 7, 8],
        [0, 3, 6], [1, 4, 7], [2, 5, 8],
        [0, 4, 8], [2, 4, 6]
    ];
    
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;

    public static Game Create(Guid commandId, string suggestedName)
    {
        var game = new Game();
        var gameCreated = new GameCreated(commandId, suggestedName);
        game.Apply(gameCreated);

        return game;
    }

    public string Join(Guid playerId)
    {
        if (_xPlayer == playerId)
        {
            return "X";
        }

        if (_oPlayer == playerId)
        {
            return "O";
        }

        Apply(new PlayerJoined(playerId));

        return _xPlayer == playerId ? "X" : "O";
    }

    public void Play(Guid playerId, int cell)
    {
        throw new NotImplementedException();
    }

    private void DetectFinishedGame()
    {
        var gameFinished = _boardCells.All(boardCell => !boardCell.IsEmpty);
        if (gameFinished)
        {
            Apply(new GameFinished(Id, null, null));
        }
    }

    private bool WinnerDetected(Guid playerId)
    {
        var hasWinner = false;
        foreach (var combo in _wins)
        {
            var (a, b, c) = (combo[0], combo[1], combo[2]);
            if (_boardCells[a] == Cell.Empty || _boardCells[a] != _boardCells[b] || _boardCells[b] != _boardCells[c])
                continue;

            hasWinner = true;
        }

        if (!hasWinner)
        {
            return false;
        }

        Apply(new GameFinished(Id, playerId, playerId == _xPlayer ? _oPlayer.Value : _xPlayer.Value));
        return true;
    }

    public string[] ToArray() => _boardCells.Select(cell => cell.ToString()).ToArray();
}
