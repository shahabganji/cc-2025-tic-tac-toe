using TicTacToe.Domain;
using TicTacToe.Domain.Players;
using TicTacToe.Domain.Shared;

namespace TicTacToe.Features.Players.RegisterFeatures;

public record struct RegisterPlayer(Guid Id, string Name);

public sealed class RegisterPlayerHandler(IEventStore store) : CommandHandler<RegisterPlayer>(store)
{
    private readonly IEventStore _store = store;
    public override async Task Handle(RegisterPlayer command)
    {
        var gameStream = GetStream<Player>(command.Id);
        
        var player = Player.Create(command.Id, command.Name);
        
        _store.AppendToStream(player.Id, player.Changes);
        
        await _store.SaveStreamAsync();
    }
}
