using Microsoft.AspNetCore.SignalR.Client;
using TicTacToeOnline.ConsoleClient;

Console.Title = "TicTacToe Console Test Client";

var hubUrl = "http://localhost:5267/hubs/game";
var connection = new HubConnectionBuilder()
    .WithUrl(hubUrl)
    .WithAutomaticReconnect()
    .Build();

connection.On<CreateGameResponse>("OnCreateGame", (response) =>
{
    Console.WriteLine($"\n[SYSTEM] Successfully created new game with ID: {response.BoardDto.BoardId}");
});

connection.On<JoinGameResponse>("PlayerJoined", (response) =>
{
    if (response?.BoardDto != null)
    {
        Console.WriteLine($"\n[SYSTEM] Player joined game ID: {response.BoardDto.BoardId}");
    }
    else
    {
        Console.WriteLine("\n[ERROR] response.BoardDto is null");
    }
});

connection.On<string>("NotifyGroupOnPlayerJoin", (connectionId) =>
{
    Console.WriteLine($"\n[SYSTEM] Player with ID {connectionId} has joined the game");
});

connection.On<string, char[]>("GameUpdated", (statusMessage, boardState) =>
{
    Console.Clear();
    Console.WriteLine($"Status: {statusMessage}\n");
    RenderBoard(boardState);
    Console.Write("\nEnter cell index (0-8) to make a move: ");
});

connection.On<string>("ErrorOccured", (errorMessage) =>
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\n[ERROR] {errorMessage}");
    Console.ResetColor();
    Console.Write("Try again (0-8): ");
});

try
{
    Console.WriteLine("Connecting to the game server...");
    await connection.StartAsync();

    Console.WriteLine("\n[SYSTEM] Connected Succesfully!");

    Console.WriteLine("1. Create Game\n2. Join Game");

    var answer = Console.ReadLine();

    string boardId;
    if (answer == "1")
    {
        Console.WriteLine("\n[SYSTEM] Creating game...");
        var createGameResponse = await connection.InvokeAsync<CreateGameResponse>("CreateGame");

        if (createGameResponse!.BoardDto == null)
        {
            Console.WriteLine("\n[ERROR] Error. Board could not be fetched");
            return;
        }

        boardId = createGameResponse.BoardDto.BoardId;

        await connection.InvokeAsync<JoinGameResponse>("JoinGameRoom", boardId);
        Console.WriteLine($"\n[SYSTEM] Created & joined game room: {boardId}");
    }
    else
    {
        Console.WriteLine("Enter game id: ");
        boardId = Console.ReadLine()!;
        var joinGameResponse = await connection.InvokeAsync<JoinGameResponse>("JoinGameRoom", boardId);
        if (joinGameResponse == null || !joinGameResponse.Success)
        {
            Console.WriteLine($"\n[ERROR] Failed to join game.");
            return;
        }
        Console.WriteLine("\n[SYSTEM] Joined game successfully!");
    }

    await Task.Delay(-1);
}
catch (Exception ex)
{
    Console.WriteLine($"Connection failed: {ex.Message}");
}
finally
{
    await connection.StopAsync();
    Console.WriteLine("Disconnected!");
}

static void RenderBoard(char[] cells)
{
    for (int i = 0; i < 9; i++)
    {
        if ((i + 2) % 3 == 0)
        {
            Console.WriteLine();
        }

        Console.Write($"  {cells[i]}  ");
    }
}