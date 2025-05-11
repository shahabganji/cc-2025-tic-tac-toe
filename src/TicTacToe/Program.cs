using static TicTacToe.ProgramExtension;

var builder = WebApplication.CreateBuilder(args);

var app = PrepareLiveDemo(builder);

app.Run();
