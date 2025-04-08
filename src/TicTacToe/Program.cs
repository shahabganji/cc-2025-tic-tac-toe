using TicTacToe.Domain.Games.Commands;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/game/{suggestedName}", async (string suggestedName, CreateGameHandler handler) =>
    {
        var command = new CreateGame(Guid.CreateVersion7(), suggestedName);
        await handler.Handle(command);
        
        return Results.Created("/game/{id}", command.Id);
    })
    .WithName("CreateGame");

app.Run();

