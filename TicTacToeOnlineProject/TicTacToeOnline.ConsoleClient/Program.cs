using Microsoft.AspNetCore.SignalR.Client;
using TicTacToeOnline.ConsoleClient;

Console.Title = "TicTacToe Console Test Client";

var hubUrl = "http://localhost:5267/hubs/game";
var connection = new HubConnectionBuilder()
    .WithUrl(hubUrl)
    .WithAutomaticReconnect()
    .Build();

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

connection.On<string>("NotifyGroupOnPlayerJoin", static (connectionId)
    => Console.WriteLine($"\n[SYSTEM] Player with ID {connectionId} has joined the game"));

connection.On<BoardDto>("GameUpdated", (dto) =>
{
    Console.Clear();
    RenderBoard(dto.Grid);
    Console.Write("\nEnter cell index (0-8) to make a move: ");
});

connection.On<BoardDto, string>("GameOver", (dto, winnerConnectionId) =>
{
    Console.Clear();
    RenderBoard(dto.Grid);
    Console.WriteLine($"\n[SYSTEM] Player {winnerConnectionId} has won the game!");
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
        if (joinGameResponse?.Success is not true)
        {
            Console.WriteLine($"\n[ERROR] Failed to join game.");
            return;
        }
        Console.WriteLine("\n[SYSTEM] Joined game successfully!");
    }

    string[] initCells = new string[9];
    RenderBoard(initCells);
    Console.WriteLine("\n[SYSTEM] Game initialized. Enter cell index (0-8) to make a move, or 'q' to quit.");

    while (true)
    {
        Console.Write("\nEnter cell index (0-8): ");
        var input = await Task.Run(() => Console.ReadLine());

        if (string.Equals(input?.Trim(), "q", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Exiting game...");
            break;
        }

        if (int.TryParse(input, out int cellIndex) && cellIndex >= 0 && cellIndex <= 8)
        {
            try
            {
                // Send move invocation to the hub
                await connection.InvokeAsync("MakeMove", boardId, cellIndex, 'X');
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[ERROR] Failed to send move: {ex.Message}");
                Console.ResetColor();
            }
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a number between 0 and 8.");
        }
    }
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

static void RenderBoard(string[] cells)
{
    if (cells == null || cells.Length < 9) return;

    Console.WriteLine("-------------");
    for (int i = 0; i < 9; i++)
    {
        string displayChar = cells[i] == "" ? " " : cells[i];
        Console.Write($"| {displayChar} ");

        if ((i + 1) % 3 == 0)
        {
            Console.WriteLine("|");
            Console.WriteLine("-------------");
        }
    }
}