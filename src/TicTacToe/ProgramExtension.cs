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
    internal static WebApplication CreateTicTacToe(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Services
            .AddGameFeatures()
            .AddPlayerFeatures()
            .AddCosmosEventStore(webApplicationBuilder.Configuration.GetConnectionString("Cosmos")!)
            ;

        var webApplication = ConfigureWebApplicationServices(webApplicationBuilder);

        webApplication.MapHub<TicTacToeHub>("/hubs");

        webApplication.RegisterPlayerEndpoints();

        webApplication.RegisterGameEndpoints();

        // if the requested route does not exist, then route it to the index.html file, blazor landing page
        webApplication.MapFallbackToFile("index.html");
        return webApplication;
    }

    private static WebApplication ConfigureWebApplicationServices(WebApplicationBuilder webApplicationBuilder)
    {
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
        return webApplication;
    }
}
