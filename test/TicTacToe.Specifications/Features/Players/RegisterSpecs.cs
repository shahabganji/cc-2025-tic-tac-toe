using TicTacToe.Domain.Players.Events;
using TicTacToe.Domain.Shared;
using TicTacToe.Features.Players.RegisterFeatures;
using TicTacToe.Specifications.Helpers;

namespace TicTacToe.Specifications.Features.Players;

public sealed class RegisterSpecs : CommandHandlerHelper<RegisterPlayer>
{
    
    private Guid PlayerId => AggregateId;
    protected override CommandHandler<RegisterPlayer> Handler => new RegisterPlayerHandler(EventStore);
    
    [Fact]
    public async Task Registering_a_player_should_create_a_new_player()
    {
        Given();
        
        await When(new RegisterPlayer(PlayerId, "Shahab"));
        
        Then(new PlayerRegistered(PlayerId, "Shahab"));
    }
    
}
