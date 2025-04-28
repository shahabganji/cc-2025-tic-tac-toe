using TicTacToe.Domain;
using TicTacToe.Domain.Games.Events;
using TicTacToe.Domain.Games.PlaySquareFeatures;
using TicTacToe.Specifications.Helpers;

namespace TicTacToe.Specifications.Domain.Games;

public sealed class FillCellSpecs : CommandHandlerHelper<FillCell>
{
    private Guid GameId => AggregateId;
    private GameCreated NewGame => new GameCreated(GameId, "My Game");

    private readonly Guid _xPlayer = Guid.NewGuid();
    private readonly Guid _oPlayer = Guid.NewGuid();

    protected override CommandHandler<FillCell> Handler => new FillCellHandler(EventStore);

    [Fact]
    public async Task Cannot_play_a_game_without_players()
    {
        Given(NewGame);

        await Assert.ThrowsAsync<InvalidOperationException>(() => When(new FillCell(GameId, _xPlayer, 0)));
    }

    [Fact]
    public async Task Game_Can_be_played_by_joined_players()
    {
        Given(NewGame,
        new PlayerJoined(_xPlayer),
        new PlayerJoined(_oPlayer));

        var invalidPlayerId = Guid.NewGuid();
        await Assert.ThrowsAsync<InvalidOperationException>(() => When(new FillCell(GameId, invalidPlayerId, 0)));
    }


    [Fact]
    public async Task xPlayer_Starts_the_game()
    {
        Given(NewGame,
            new PlayerJoined(_xPlayer),
            new PlayerJoined(_oPlayer));

        await When(new FillCell(GameId, _xPlayer, 1));

        Then(new CellFilled(_xPlayer, 1));
    }

    [Fact]
    public async Task Play_an_empty_cell()
    {
        Given(NewGame,
            new PlayerJoined(_xPlayer),
            new PlayerJoined(_oPlayer));

        await When(new FillCell(GameId, _xPlayer, 0));

        Then(new CellFilled(_xPlayer, 0));
    }

    [Fact]
    public async Task Play_already_filled_cell_will_fail()
    {
        Given(NewGame,
            new PlayerJoined(_xPlayer),
            new PlayerJoined(_oPlayer),
            new CellFilled(_xPlayer, 0));

        await Assert.ThrowsAsync<InvalidOperationException>(() => When(new FillCell(GameId, _oPlayer, 0)));
    }


    [Fact]
    public async Task Play_out_of_your_turn_will_fail()
    {
        Given(NewGame, new CellFilled(_xPlayer, 0));

        await Assert.ThrowsAsync<InvalidOperationException>(() => When(new FillCell(GameId, _xPlayer, 1)));
    }

    [Fact]
    public async Task OPlayer_cannot_start_the_game()
    {
        Given(NewGame);

        await Assert.ThrowsAsync<InvalidOperationException>(() => When(new FillCell(GameId, _oPlayer, 1)));
    }


    [Fact]
    public async Task Detect_out_of_turn_play()
    {
        Given(NewGame,
            new PlayerJoined(_xPlayer),
            new PlayerJoined(_oPlayer),
            new CellFilled(_xPlayer, 0),
            new CellFilled(_oPlayer, 1),
            new CellFilled(_xPlayer, 4),
            new CellFilled(_oPlayer, 7)
        );

        await Assert.ThrowsAsync<InvalidOperationException>(() => When(new FillCell(GameId, _oPlayer, 8)));
    }

    [Fact]
    public async Task Detect_a_win_on_first_rows()
    {
        Given(NewGame,
            new PlayerJoined(_xPlayer),
            new PlayerJoined(_oPlayer),
            new CellFilled(_xPlayer, 0),
            new CellFilled(_oPlayer, 4),
            new CellFilled(_xPlayer, 1),
            new CellFilled(_oPlayer, 5)
        );

        await When(new FillCell(GameId, _xPlayer, 2));

        Then(new CellFilled(_xPlayer, 2), new GameFinished(GameId, _xPlayer, _oPlayer));
    }

    [Fact]
    public async Task Detect_a_win_on_second_rows()
    {
        Given(NewGame,
            new PlayerJoined(_xPlayer),
            new PlayerJoined(_oPlayer),
            new CellFilled(_xPlayer, 3),
            new CellFilled(_oPlayer, 6),
            new CellFilled(_xPlayer, 4),
            new CellFilled(_oPlayer, 7)
        );

        await When(new FillCell(GameId, _xPlayer, 5));

        Then(new CellFilled(_xPlayer, 5), new GameFinished(GameId, _xPlayer, _oPlayer));
    }

    [Fact]
    public async Task Detect_a_win_on_third_rows()
    {
        Given(NewGame,
            new PlayerJoined(_xPlayer),
            new PlayerJoined(_oPlayer),
            new CellFilled(_xPlayer, 6),
            new CellFilled(_oPlayer, 0),
            new CellFilled(_xPlayer, 7),
            new CellFilled(_oPlayer, 4)
        );

        await When(new FillCell(GameId, _xPlayer, 8));

        Then(new CellFilled(_xPlayer, 8), new GameFinished(GameId, _xPlayer, _oPlayer));
    }


    [Fact]
    public async Task Detect_a_win_on_first_column()
    {
        Given(NewGame,
            new PlayerJoined(_xPlayer),
            new PlayerJoined(_oPlayer),
            new CellFilled(_xPlayer, 2),
            new CellFilled(_oPlayer, 0),
            new CellFilled(_xPlayer, 5),
            new CellFilled(_oPlayer, 3),
            new CellFilled(_xPlayer, 4)
        );

        await When(new FillCell(GameId, _oPlayer, 6));

        Then(new CellFilled(_oPlayer, 6), new GameFinished(GameId, _oPlayer, _xPlayer));
    }

    [Fact]
    public async Task Detect_a_win_on_second_column()
    {
        Given(NewGame,
            new PlayerJoined(_xPlayer),
            new PlayerJoined(_oPlayer),
            new CellFilled(_xPlayer, 8),
            new CellFilled(_oPlayer, 7),
            new CellFilled(_xPlayer, 5),
            new CellFilled(_oPlayer, 4),
            new CellFilled(_xPlayer, 0)
        );

        await When(new FillCell(GameId, _oPlayer, 1));

        Then(new CellFilled(_oPlayer, 1), new GameFinished(GameId, _oPlayer, _xPlayer));
    }

    [Fact]
    public async Task Detect_a_win_on_third_column()
    {
        Given(NewGame,
            new PlayerJoined(_xPlayer),
            new PlayerJoined(_oPlayer),
            new CellFilled(_xPlayer, 0),
            new CellFilled(_oPlayer, 2),
            new CellFilled(_xPlayer, 3),
            new CellFilled(_oPlayer, 5),
            new CellFilled(_xPlayer, 4)
        );

        await When(new FillCell(GameId, _oPlayer, 8));

        Then(new CellFilled(_oPlayer, 8), new GameFinished(GameId, _oPlayer, _xPlayer));
    }

    [Fact]
    public async Task Detect_a_diagonal_left_to_right_win()
    {
        Given(NewGame,
            new PlayerJoined(_xPlayer),
            new PlayerJoined(_oPlayer),
            new CellFilled(_xPlayer, 0),
            new CellFilled(_oPlayer, 1),
            new CellFilled(_xPlayer, 4),
            new CellFilled(_oPlayer, 7)
        );

        await When(new FillCell(GameId, _xPlayer, 8));

        Then(new CellFilled(_xPlayer, 8), new GameFinished(GameId, _xPlayer, _oPlayer));
    }


    [Fact]
    public async Task Detect_a_diagonal_right_to_left_win()
    {
        Given(NewGame,
            new PlayerJoined(_xPlayer),
            new PlayerJoined(_oPlayer),
            new CellFilled(_xPlayer, 0),
            new CellFilled(_oPlayer, 2),
            new CellFilled(_xPlayer, 5),
            new CellFilled(_oPlayer, 6),
            new CellFilled(_xPlayer, 3)
        );

        await When(new FillCell(GameId, _oPlayer, 4));

        Then(new CellFilled(_oPlayer, 4), new GameFinished(GameId, _oPlayer, _xPlayer));
    }


    [Fact]
    public async Task Detect_a_draw()
    {
        Given(NewGame,
            new PlayerJoined(_xPlayer),
            new PlayerJoined(_oPlayer),
            new CellFilled(_xPlayer, 1),
            new CellFilled(_oPlayer, 0),
            new CellFilled(_xPlayer, 2),
            new CellFilled(_oPlayer, 4),
            new CellFilled(_xPlayer, 3),
            new CellFilled(_oPlayer, 5),
            new CellFilled(_xPlayer, 7),
            new CellFilled(_oPlayer, 6)
        );

        await When(new FillCell(GameId, _xPlayer, 8));
        Then(new CellFilled(_xPlayer, 8), new GameFinished(GameId, null, null));
    }
}
