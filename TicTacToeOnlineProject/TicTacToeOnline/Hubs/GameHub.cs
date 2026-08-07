using Microsoft.AspNetCore.SignalR;

namespace TicTacToeOnline.Hubs;

public class GameHub : Hub<IGameClient>
{
    public async Task JoinGameRoom(string gameId)
    {

    }

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"Client connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
        await base.OnDisconnectedAsync(exception);
    }
}