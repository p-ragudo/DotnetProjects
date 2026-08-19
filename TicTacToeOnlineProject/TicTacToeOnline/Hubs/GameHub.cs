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
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await Clients.OthersInGroup(gameId).NotifyGroupOnPlayerJoin();
        }

        return response;
    }

    public async Task<AssignMarkResponse> GetMark(string gameId)
    {
        var response = await _boardService.GetMarkAsync(gameId);
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
            await Clients.Group(gameId).GameOver(response.UpdatedBoardDto!, (char)response.WinnerMark);
            return response;
        }

        await Clients.Group(gameId).GameUpdated(response.UpdatedBoardDto!);
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