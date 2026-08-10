namespace TicTacToeOnline.ConsoleClient;

public class BoardDto
{
    public string BoardId { get; init; } = string.Empty;
    public string[] Grid { get; init; } = new string[9];
}

public class CreateGameResponse
{
    public bool Success { get; set; }
    public string Status { get; set; } = string.Empty;
    public BoardDto? BoardDto { get; set;} = new();
}

public class JoinGameResponse
{
    public bool Success { get; set; }
    public string Status { get; set; } = string.Empty;
    public BoardDto? BoardDto { get; set; } = new();
}

public class MoveResult
{
    public bool Success { get; set; }
    public string Status { get; set; }
    public BoardDto? BoardDto { get; set; } = new();
    public bool IsGameOver { get; set; }
    public char? WinnerId { get; init; }
}