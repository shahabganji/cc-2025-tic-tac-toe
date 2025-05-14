using Microsoft.AspNetCore.SignalR;
using TicTacToe.Features.Games.CreateGameFeatures;
using TicTacToe.Features.Games.FillCellFeatures;
using TicTacToe.Features.Games.GetGameEventsFeature;
using TicTacToe.Features.Games.LoadGamesFeature;
using TicTacToe.Features.Games.LoadGameStateFeature;
using TicTacToe.Hubs;
using static TicTacToe.Features.Games.GameEndpoints.Handlers;

namespace TicTacToe.Features.Games;

internal static class GameEndpoints
{
    public static void RegisterGameEndpoints(this WebApplication webApplication)
    {
        var gamesGroup = webApplication.MapGroup("/games");

        gamesGroup.MapPost("/", CreateGameEndpoint).WithName("CreateGame");

        gamesGroup.MapGet("/available", ShowAvailableGamesEndpoint).WithName("ShowAvailableGames")
            .CacheOutput(policyBuilder => policyBuilder.Expire(TimeSpan.FromSeconds(5)));
        gamesGroup.MapPost("/{gameId:guid}/play/cell", PlayCellEndpoint).WithName("FillCell");

        gamesGroup.MapGet("/{gameId:guid}/events", GetGameEventsEndpoint).WithName("GetGameEvents");

        gamesGroup.MapGet("/{gameId:guid}/", LoadStateEndpoint).WithName("LoadState");
    }


    internal static class Handlers
    {
        internal static async Task<IResult> CreateGameEndpoint(CreateGame game, CreateGameHandler handler,
            IHubContext<TicTacToeHub, ITicTacToeClient> context)
        {
            var gameId = Guid.CreateVersion7();
            var gameName = string.IsNullOrWhiteSpace(game.SuggestedName)
                ? $"Game: {gameId}"
                : game.SuggestedName;

            var command = new CreateGame(gameId, gameName);
            await handler.Handle(command);

            await context.Clients.All.GameCreated(gameId, gameName);

            return Results.Created("/game/{id}", command.Id);
        }

        internal static async Task<IResult> ShowAvailableGamesEndpoint(ShowListOfGamesHandler handler)
        {
            var result = await handler.Query(new ShowListOfAvailableGames());

            return Results.Ok(result);
        }

        internal static async Task<IResult> PlayCellEndpoint(Guid gameId, FillCell cell, FillCellHandler handler,
            IHubContext<TicTacToeHub, ITicTacToeClient> context)
        {
            if (gameId != cell.GameId)
            {
                throw new InvalidOperationException("The game id in the path does not match the game id in the body");
            }

            await handler.Handle(cell);
            await context.Clients.Groups(cell.GameId.ToString())
                .CellFilled(cell.GameId, cell.PlayerId, cell.Cell);

            return Results.Ok();
        }

        internal static async Task<IResult> GetGameEventsEndpoint(Guid gameId, GetGameEventsHandler handler)
        {
            var result = await handler.Query(new GetGameEvents(gameId));

            return Results.Ok(result);
        }

        internal static async Task<IResult> LoadStateEndpoint(Guid gameId, LoadGameStateHandler handler)
        {
            var result = await handler.Handle(new LoadGameState(gameId));

            return Results.Ok(result);
        }
    }
}
