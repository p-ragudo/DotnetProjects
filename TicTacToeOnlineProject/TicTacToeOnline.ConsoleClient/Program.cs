using Microsoft.AspNetCore.SignalR.Client;

Console.Title = "TicTacToe Console Test Client";

var hubUrl = "http://localhost:5267/hubs/game";
var connection = new HubConnectionBuilder()
    .WithUrl(hubUrl)
    .WithAutomaticReconnect()
    .Build();

connection.On<char>("PlayerJoined", (assignedMark) =>
{
    Console.WriteLine($"\n[SYSTEM] Successfully connected! You are playing as: '{assignedMark}'");
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

    Console.Write("Enter Game ID to join: ");
    string? gameId = Console.ReadLine();

    if (!string.IsNullOrWhiteSpace(gameId))
    {
        // Invoke server method to join room
        await connection.InvokeAsync("JoinGameRoom", gameId);
    }

    // 4. Input Loop: Read player moves from the terminal and push over WebSockets
    while (connection.State == HubConnectionState.Connected)
    {
        string? input = Console.ReadLine();
        if (int.TryParse(input, out int cellIndex))
        {
            // Invoke server method to make move
            await connection.InvokeAsync("MakeMove", gameId, cellIndex);
        }
        else if (input?.ToLower() == "quit")
        {
            break;
        }
        else
        {
            Console.WriteLine("Invalid input. Enter a number from 0 to 8 (or 'quit'):");
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