using Microsoft.AspNetCore.SignalR;
namespace TicTacToeOnline.Hubs;

public class GameHub : Hub<IGameClient>
{
    public async Task JoinGameRoom(string gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);

        await Clients.Caller.PlayerJoined('X');

        char[] emptyBoard = new char[9];
        await Clients.Group(gameId).GameUpdated($"Player connected with ID: {Context.ConnectionId}", emptyBoard);
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