namespace TicTacToe.Web.Contracts;

public sealed record EventsInfo
{
    public required Guid EventId { get; init; }
    public required string EventName { get; init; }
    public required string DisplayInfo { get; init; }
}
