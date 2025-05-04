using TicTacToe.Domain.Games.Events;

namespace TicTacToe.Domain.Games;

public sealed partial class Game : AggregateRoot
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public Guid? Winner { get; private set; }
    public Guid? Loser { get; private set; }

    public bool IsGameFinished { get; private set; }

    private Guid? XPlayer { get; set; }
    private Guid? OPlayer { get; set; }

    private Guid? CurrentPlayer { get; set; }

    private static readonly char? EmptyCell = null;
    private const char XCell = 'X';
    private const char OCell = 'O';

    private readonly char?[] _boardCells = new char?[9];

    private readonly int[][] _wins =
    [
        [0, 1, 2], [3, 4, 5], [6, 7, 8],
        [0, 3, 6], [1, 4, 7], [2, 5, 8],
        [0, 4, 8], [2, 4, 6]
    ];

    public static Game Create(Guid commandId, string suggestedName)
    {
        var game = new Game();
        var gameCreated = new GameCreated(commandId, suggestedName);
        game.Apply(gameCreated);

        return game;
    }
    
    public string Join(Guid playerId)
    {
        if (XPlayer == playerId)
        {
            return "X";
        }

        if (OPlayer == playerId)
        {
            return "O";
        }
        
        Apply(new PlayerJoined(playerId));
        
        return XPlayer == playerId ? "X" : "O";
    }
    
    public void Play(Guid playerId, int cell)
    {
        GuardAgainstInvalidPrerequisites(playerId, cell);

        if (CurrentPlayer != playerId)
        {
            throw new InvalidOperationException("It is not your turn");
        }

        var cellValue = playerId == XPlayer ? XCell : OCell;
        _boardCells[cell] = cellValue;

        Apply(new CellFilled(playerId, cell));

        if (WinnerDetected(playerId))
            return;

        DetectFinishedGame();
    }

    private void GuardAgainstInvalidPrerequisites(Guid playerId, int cell)
    {
        if (IsGameFinished)
        {
            throw new InvalidOperationException("Game is already finished");
        }
        
        if (XPlayer is null || OPlayer is null)
        {
            throw new InvalidOperationException("Game is not started");
        }

        if (playerId != XPlayer && playerId != OPlayer)
        {
            throw new InvalidOperationException("Player is not part of the game");
        }
        
        if (_boardCells[cell] != EmptyCell)
        {
            throw new InvalidOperationException("Cell is already filled");
        }
    }

    private void DetectFinishedGame()
    {
        var gameFinished = _boardCells.All(boardCell => boardCell != EmptyCell);
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
            if (_boardCells[a] == EmptyCell || _boardCells[a] != _boardCells[b] || _boardCells[b] != _boardCells[c])
                continue;

            hasWinner = true;
        }

        if (!hasWinner)
        {
            return false;
        }

        Apply(new GameFinished(Id, playerId, playerId == XPlayer ? OPlayer.Value : XPlayer.Value));
        return true;
    }
}
