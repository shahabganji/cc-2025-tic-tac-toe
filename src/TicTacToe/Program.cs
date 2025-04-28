using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using TicTacToe.Domain;
using TicTacToe.Domain.Games.CreateGameFeatures;
using TicTacToe.Domain.Players.RegisterFeatures;
using TicTacToe.Hubs;
using TicTacToe.Stores;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCommandHandlers()
    .AddEventStore()
    ;

builder.Services.AddSignalR();

builder.Services.AddAntiforgery();
builder.Services.AddResponseCompression(options =>
{
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat([
        MediaTypeNames.Application.Octet,
        MediaTypeNames.Application.Wasm,
        MediaTypeNames.Application.Json,
    ]);
});

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseWebAssemblyDebugging();
}

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

app.MapPost("/game", async (CreateGame game, CreateGameHandler handler, IHubContext<TicTacToeHub, ITicTacToeClient> context) =>
    {
        var gameId = Guid.CreateVersion7();
        var gameName = string.IsNullOrWhiteSpace(game.SuggestedName) ? $"Game: {gameId}" : game.SuggestedName;

        var command = new CreateGame(gameId, gameName);
        await handler.Handle(command);
        
        await context.Clients.All.GameCreated(gameId, gameName);

        return Results.Created("/game/{id}", command.Id);
    })
    .WithName("CreateGame");

// if the requested route does not exist, then route it to the index.html file, blazor landing page
app.MapFallbackToFile("index.html");

app.Run();
