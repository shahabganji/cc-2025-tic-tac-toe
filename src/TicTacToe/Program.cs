using static TicTacToe.ProgramExtension;

var builder = WebApplication.CreateBuilder(args);

var app = CreateTicTacToe(builder);

app.Run();
