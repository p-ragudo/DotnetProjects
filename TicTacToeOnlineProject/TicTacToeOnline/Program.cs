using System.Text.Json.Serialization;
using TicTacToeOnline.Data;
using TicTacToeOnline.Hubs;
using TicTacToeOnline.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IGameStore, InMemoryGameStore>();
builder.Services.AddTransient<BoardService>();
builder.Services.AddSignalR()
    .AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        );
    });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:3000", "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.MapHub<GameHub>("/hubs/game");

app.Run();