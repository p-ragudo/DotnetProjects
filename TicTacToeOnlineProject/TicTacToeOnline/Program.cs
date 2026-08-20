using System.Text.Json.Serialization;
using TicTacToeOnline.Data;
using TicTacToeOnline.Hubs;
using TicTacToeOnline.Services;

var builder = WebApplication.CreateBuilder(args);

var envOrigins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS");

var allowedOrigins = !string.IsNullOrWhiteSpace(envOrigins)
    ? envOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    : [
        "http://localhost:3000",
        "http://localhost:5173",
        "http://192.168.18.152:5173"
      ];

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
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.MapHub<GameHub>("/hubs/game");
app.MapGet("/health", () => Results.Ok(
    new { status = "healthy", timestamp = DateTime.UtcNow }
));

app.Run();