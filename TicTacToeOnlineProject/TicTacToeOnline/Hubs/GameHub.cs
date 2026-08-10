using Microsoft.AspNetCore.SignalR;
using TicTacToeOnline.Dto;
using TicTacToeOnline.Services;

namespace TicTacToeOnline.Hubs;

public class GameHub : Hub<IGameClient>
{
    private readonly BoardService _boardService;

    public GameHub(BoardService boardService)
    {
        _boardService = boardService;
    }

    public async Task<CreateGameResponse> CreateGame()
    {
        var response = await _boardService.CreateGameAsync();
        return response;
    }

    public async Task<JoinGameResponse> JoinGameRoom(string gameId)
    {
        var response = await _boardService.JoinGameAsync(gameId);

        if (response.Success)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await Clients.OthersInGroup(gameId).NotifyGroupOnPlayerJoin(Context.ConnectionId);
        }

        return response;
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