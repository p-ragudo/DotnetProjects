using Microsoft.AspNetCore.SignalR.Client;
using TicTacToeOnline.ConsoleClient;

Console.Title = "TicTacToe Console Test Client";

var hubUrl = "http://localhost:5267/hubs/game";
var connection = new HubConnectionBuilder()
    .WithUrl(hubUrl)
    .WithAutomaticReconnect()
    .Build();

char mark = ' ';
bool gameOver = false;
char currentTurn = ' ';

// Semaphore initialized to 0 locks execution until Release() is called
using SemaphoreSlim turnSemaphore = new(0, 1);

connection.On<JoinGameResponse>("PlayerJoined", (response) =>
{
    if (response?.BoardDto != null)
    {
        Console.WriteLine($"\n[SYSTEM] Player joined game ID: {response.BoardDto.BoardId}");
        currentTurn = response.BoardDto.CurrentTurn;
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

    currentTurn = dto.CurrentTurn;

    // Unblock if it's now this player's turn
    if (currentTurn == mark && turnSemaphore.CurrentCount == 0)
    {
        turnSemaphore.Release();
    }
});

// Unblock the turn loop when it becomes this player's turn
connection.On<char>("CurrentTurn", (turnMark) =>
{
    currentTurn = turnMark;
    if (currentTurn == mark && turnSemaphore.CurrentCount == 0)
    {
        turnSemaphore.Release();
    }
});

connection.On<BoardDto, string>("GameOver", (dto, winnerConnectionId) =>
{
    Console.Clear();
    RenderBoard(dto.Grid);
    Console.WriteLine($"\n[SYSTEM] Player {winnerConnectionId} has won the game!");
    gameOver = true;

    // Unblock the loop so it can cleanly exit on game over
    if (turnSemaphore.CurrentCount == 0)
    {
        turnSemaphore.Release();
    }
});

connection.On<string>("ErrorOccured", (errorMessage) =>
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\n[ERROR] {errorMessage}");
    Console.ResetColor();

    // Allow the player to try entering input again on invalid turn action
    if (turnSemaphore.CurrentCount == 0)
    {
        turnSemaphore.Release();
    }
});

try
{
    Console.WriteLine("Connecting to the game server...");
    await connection.StartAsync();

    Console.WriteLine("\n[SYSTEM] Connected Succesfully!");
    Console.WriteLine("1. Create Game\n2. Join Game");

    var answer = Console.ReadLine();

    string boardId;
    JoinGameResponse? joinGameResponse;

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

        joinGameResponse = await connection.InvokeAsync<JoinGameResponse>("JoinGameRoom", boardId);
        Console.WriteLine($"\n[SYSTEM] Created & joined game room: {boardId}");
    }
    else
    {
        Console.WriteLine("Enter game id: ");
        boardId = Console.ReadLine()!;

        joinGameResponse = await connection.InvokeAsync<JoinGameResponse>("JoinGameRoom", boardId);
        if (joinGameResponse?.Success is not true)
        {
            Console.WriteLine($"\n[ERROR] Failed to join game.");
            return;
        }
        Console.WriteLine("\n[SYSTEM] Joined game successfully!");
    }

    // Assign currentTurn directly from the RPC response object
    if (joinGameResponse?.BoardDto != null)
    {
        currentTurn = joinGameResponse.BoardDto.CurrentTurn;
    }

    var assignMarkResponse = await connection.InvokeAsync<AssignMarkResponse>("GetMark", boardId);
    mark = assignMarkResponse.Mark;
    Console.WriteLine($"\n[SYSTEM] You are {mark}");

    // Release semaphore immediately if this player goes first
    if (currentTurn == mark && turnSemaphore.CurrentCount == 0)
    {
        turnSemaphore.Release();
    }

    string[] initCells = joinGameResponse?.BoardDto?.Grid ?? new string[9];
    RenderBoard(initCells);
    Console.WriteLine("\n[SYSTEM] Game initialized.");

    while (!gameOver)
    {
        if (currentTurn != mark)
        {
            Console.WriteLine($"\n[SYSTEM] Current Turn: {currentTurn}. Waiting for opponent...");
            
            // Asynchronously pause here until SignalR fires CurrentTurn matching 'mark'
            await turnSemaphore.WaitAsync();
        }

        if (gameOver) break;

        Console.Write($"\n[YOUR TURN ({mark})] Enter cell index (0-8) or 'q' to quit: ");
        var input = await Task.Run(() => Console.ReadLine()!);

        if (string.Equals(input?.Trim(), "q", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Exiting game...");
            break;
        }

        if (int.TryParse(input, out int cellIndex) && cellIndex is >= 0 and <= 8)
        {
            try
            {
                await connection.InvokeAsync("MakeMove", boardId, cellIndex, mark);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[ERROR] Failed to send move: {ex.Message}");
                Console.ResetColor();

                // Re-enable input if the invocation failed
                if (turnSemaphore.CurrentCount == 0)
                {
                    turnSemaphore.Release();
                }
            }
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a number between 0 and 8.");
            
            // Re-enable input prompt for retry
            if (turnSemaphore.CurrentCount == 0)
            {
                turnSemaphore.Release();
            }
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
        string displayChar = string.IsNullOrEmpty(cells[i]) ? " " : cells[i];
        Console.Write($"| {displayChar} ");

        if ((i + 1) % 3 == 0)
        {
            Console.WriteLine("|");
            Console.WriteLine("-------------");
        }
    }
}