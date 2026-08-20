using Microsoft.AspNetCore.SignalR;
using TicTacToeOnline.Dto;
using TicTacToeOnline.Enums;
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
        var response = await _boardService.JoinGameAsync(gameId, Context.ConnectionId);

        if (response.Success)
        {
            Context.Items["GameId"] = gameId;
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await Clients.OthersInGroup(gameId).NotifyGroupOnPlayerJoin();
        }

        return response;
    }

    public async Task<AssignMarkResponse> GetMark(string gameId)
    {
        var response = await _boardService.GetMarkAsync(gameId, Context.ConnectionId);
        return response;
    }

    public async Task<MoveResponse> MakeMove(string gameId, int cellIndex, char playerMark)
    {
        var response = await _boardService.MakeMoveAsync(gameId, cellIndex, playerMark);

        if (!response.Success)
        {
            return response;
        }
        if (response.IsGameOver)
        {
            await Clients.Group(gameId).GameOver(response.UpdatedBoardDto!, (char) response.WinnerMark!);
            return response;
        }

        await Clients.Group(gameId).GameUpdated(response.UpdatedBoardDto!);
        return response;
    }

    public async Task Rematch(string gameId, bool rematch)
    {
        var response = await _boardService.RematchAsync(gameId, rematch);

        if (response.Status == RematchReturnStatus.RematchAccepted)
        {
            // Broadcast reset board to EVERYONE in the room
            await Clients.Group(gameId).RematchRequest(response);
        }
        else
        {
            // Send "Waiting" or "RematchDenied" only to the opponent
            await Clients.OthersInGroup(gameId).RematchRequest(response);
        }
    }

    public async Task LeaveGame(string gameId)
    {
        Console.WriteLine($"Client {Context.ConnectionId} left game {gameId}");
        await _boardService.TryDeleteBoardById(gameId, Context.ConnectionId);
        await Clients.Group(gameId).NotifyGroupOnPlayerLeave();
    }

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"Client connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"Client disconnected: {Context.ConnectionId}");

        if (Context.Items.TryGetValue("GameId", out var gameIdObj) && gameIdObj is string gameId)
        {
            await _boardService.TryDeleteBoardById(gameId, Context.ConnectionId);
            await Clients.Group(gameId).NotifyGroupOnPlayerLeave(); // Optionally notify the other player
        }

        await base.OnDisconnectedAsync(exception);
    }
}