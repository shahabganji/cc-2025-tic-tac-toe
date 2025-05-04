using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using TicTacToe;
using TicTacToe.Domain;
using TicTacToe.Domain.Games.CreateGameFeatures;
using TicTacToe.Domain.Games.FillCellFeatures;
using TicTacToe.Domain.Games.LoadGamesFeature;
using TicTacToe.Domain.Players.RegisterFeatures;
using TicTacToe.Hubs;
using TicTacToe.Stores;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCommandHandlers()
    .AddQueryHandlers()
    .AddCosmosEventStore(builder.Configuration.GetConnectionString("Cosmos")!)
    ;

builder.Services.AddSignalR();

builder.Services.AddOutputCache();

builder.Services.AddAntiforgery();
builder.Services.AddResponseCompression(options =>
{
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat([
        MediaTypeNames.Application.Octet,
        MediaTypeNames.Application.Wasm,
        MediaTypeNames.Application.Json,
    ]);
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<TicTacToeGlobalExceptionHandler>();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseWebAssemblyDebugging();
}

app.UseExceptionHandler();

app.UseOutputCache();


app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseAntiforgery();

app.MapHub<TicTacToeHub>("/hubs");

app.MapPost("/player/register", async (RegisterPlayer command, RegisterPlayerHandler handler) =>
{
    command.Id = Guid.CreateVersion7();
    await handler.Handle(command);
    return Results.Created("/player/{id}", command.Id);
}).WithName("RegisterPlayer");

app.MapPost("/game",
        async (CreateGame game, CreateGameHandler handler, IHubContext<TicTacToeHub, ITicTacToeClient> context) =>
        {
            var gameId = Guid.CreateVersion7();
            var gameName = string.IsNullOrWhiteSpace(game.SuggestedName) ? $"Game: {gameId}" : game.SuggestedName;

            var command = new CreateGame(gameId, gameName);
            await handler.Handle(command);

            await context.Clients.All.GameCreated(gameId, gameName);

            return Results.Created("/game/{id}", command.Id);
        })
    .WithName("CreateGame");


app.MapGet("/games/available",
        async (ShowListOfGamesHandler handler) =>
        {
            var result = await handler.Query(new ShowListOfAvailableGames());

            return Results.Ok(result);
        })
    .CacheOutput(policyBuilder => policyBuilder.Expire(TimeSpan.FromSeconds(5)))
    .WithName("ShowAvailableGames");


app.MapPost("/game/{gameId:guid}/play/cell",
        async (Guid gameId, FillCell cell, FillCellHandler handler,
            IHubContext<TicTacToeHub, ITicTacToeClient> context) =>
        {
            if (gameId != cell.GameId)
            {
                throw new InvalidOperationException("The game id in the path does not match the game id in the body");
            }

            await handler.Handle(cell);
            await context.Clients.Groups(cell.GameId.ToString()).CellFilled(cell.GameId, cell.PlayerId, cell.Cell);

            return Results.Ok();
        })
    .WithName("FillCell");


// if the requested route does not exist, then route it to the index.html file, blazor landing page
app.MapFallbackToFile("index.html");

app.Run();
