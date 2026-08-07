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
    Console.WriteLine($"Connected. Your connection ID is {connection.ConnectionId}");
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