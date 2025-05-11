namespace TicTacToe.Domain.Games;

public readonly record struct Cell
{
    private readonly char? _value;

    private Cell(char? value) => _value = value;

    public static Cell X => new('X');
    public static Cell O => new('O');
    public static Cell Empty => new(null);
    
    public override string ToString() => _value.ToString() ?? string.Empty;
}