using System.Net.Mime;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using TicTacToe.Domain;
using TicTacToe.Features.Games;
using TicTacToe.Features.Games.CreateGameFeatures;
using TicTacToe.Features.Games.FillCellFeatures;
using TicTacToe.Features.Games.GetGameEventsFeature;
using TicTacToe.Features.Games.LoadGamesFeature;
using TicTacToe.Features.Games.LoadGameStateFeature;
using TicTacToe.Features.Players;
using TicTacToe.Features.Players.RegisterFeatures;
using TicTacToe.Hubs;
using TicTacToe.Stores;
using GetGameEventsHandler = TicTacToe.Features.Games.GetGameEventsFeature.GetGameEventsHandler;

namespace TicTacToe;

internal static class ProgramExtension
{
    internal static WebApplication PrepareLiveDemo(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Services
            .AddGameFeatures()
            .AddPlayerFeatures()
            .AddCosmosEventStore(webApplicationBuilder.Configuration.GetConnectionString("Cosmos")!)
            ;

        webApplicationBuilder.Services.AddSignalR();

        webApplicationBuilder.Services.AddOutputCache();

        webApplicationBuilder.Services.AddAntiforgery();
        webApplicationBuilder.Services.AddResponseCompression(options =>
        {
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat([
                MediaTypeNames.Application.Octet,
                MediaTypeNames.Application.Wasm,
                MediaTypeNames.Application.Json,
            ]);
        });

        webApplicationBuilder.Services.AddProblemDetails();
        webApplicationBuilder.Services.AddExceptionHandler<TicTacToeGlobalExceptionHandler>();

        webApplicationBuilder.Services.AddOpenApi();

        var webApplication = webApplicationBuilder.Build();

// Configure the HTTP request pipeline.
        if (webApplication.Environment.IsDevelopment())
        {
            webApplication.MapOpenApi();
            webApplication.UseWebAssemblyDebugging();
        }

        webApplication.UseExceptionHandler();

        webApplication.UseOutputCache();


        webApplication.UseHttpsRedirection();

        webApplication.UseBlazorFrameworkFiles();
        webApplication.UseStaticFiles();

        webApplication.UseAntiforgery();

        webApplication.MapHub<TicTacToeHub>("/hubs");

        webApplication.MapPost("/player/register", async (RegisterPlayer command, RegisterPlayerHandler handler) =>
        {
            command.Id = Guid.CreateVersion7();
            await handler.Handle(command);
            return Results.Created("/player/{id}", command.Id);
        }).WithName("RegisterPlayer");

        webApplication.MapPost("/game",
                async (CreateGame game, CreateGameHandler handler,
                    IHubContext<TicTacToeHub, ITicTacToeClient> context) =>
                {
                    var gameId = Guid.CreateVersion7();
                    var gameName = string.IsNullOrWhiteSpace(game.SuggestedName)
                        ? $"Game: {gameId}"
                        : game.SuggestedName;

                    var command = new CreateGame(gameId, gameName);
                    await handler.Handle(command);

                    await context.Clients.All.GameCreated(gameId, gameName);

                    return Results.Created("/game/{id}", command.Id);
                })
            .WithName("CreateGame");


        webApplication.MapGet("/games/available",
                async (ShowListOfGamesHandler handler) =>
                {
                    var result = await handler.Query(new ShowListOfAvailableGames());

                    return Results.Ok(result);
                })
            .CacheOutput(policyBuilder => policyBuilder.Expire(TimeSpan.FromSeconds(5)))
            .WithName("ShowAvailableGames");

        webApplication.MapPost("/games/{gameId:guid}/play/cell",
                async (Guid gameId, FillCell cell, FillCellHandler handler,
                    IHubContext<TicTacToeHub, ITicTacToeClient> context) =>
                {
                    if (gameId != cell.GameId)
                    {
                        throw new InvalidOperationException(
                            "The game id in the path does not match the game id in the body");
                    }

                    await handler.Handle(cell);
                    await context.Clients.Groups(cell.GameId.ToString())
                        .CellFilled(cell.GameId, cell.PlayerId, cell.Cell);

                    return Results.Ok();
                })
            .WithName("FillCell");

        webApplication.MapGet("/games/{gameId:guid}/events",
                async (Guid gameId, GetGameEventsHandler handler) =>
                {
                    var result = await handler.Query(new GetGameEvents(gameId));

                    return Results.Ok(result);
                })
            .WithName("GetGameEvents");

        webApplication.MapGet("/games/{gameId:guid}/",
                async (Guid gameId, LoadGameStateHandler handler) =>
                {
                    var result = await handler.Handle(new LoadGameState(gameId));

                    return Results.Ok(result);
                })
            .WithName("LoadState");


// if the requested route does not exist, then route it to the index.html file, blazor landing page
        webApplication.MapFallbackToFile("index.html");
        return webApplication;
    }
}
