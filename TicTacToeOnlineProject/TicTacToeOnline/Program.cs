using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
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

// 1. Cap SignalR buffers and connection lifetimes to avoid memory exhaustion
builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = 32 * 1024;
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    options.KeepAliveInterval = TimeSpan.FromSeconds(10);
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
})
.AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.Converters.Add(
        new JsonStringEnumConverter()
    );
});

// 2. HTTP Rate Limiting to prevent spam/abuse on endpoints
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));
});

// 3. Prevent slow-client header/body buffering attacks on Kestrel
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10 MB limit
    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);
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
app.UseRateLimiter();

app.MapHub<GameHub>("/hubs/game");
app.MapGet("/health", () =>
    Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow })
).DisableRateLimiting();

app.Run();