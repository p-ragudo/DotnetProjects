using TicTacToeOnline.Enums;
using TicTacToeOnline.GameEngine;

namespace TicTacToeOnline.Dto;

public record BoardDto
{
    public string BoardId { get; init; } = string.Empty;
    public string[] Grid { get; init; } = new string[9];
    public char CurrentTurn { get; init; }
}

public record CreateGameResponse
{
    public bool Success { get; init; }
    public GameStoreReturnStatus Status { get; init; }
    public BoardDto? BoardDto { get; init;}

    public static CreateGameResponse Failed(GameStoreReturnStatus status)
        => new() { Success = false, Status = status };

    public static CreateGameResponse Ok(BoardDto boardDto)
        => new()
        {
            Success = true,
            Status = GameStoreReturnStatus.CreateBoardSuccess,
            BoardDto = boardDto
        };
}

public record JoinGameResponse
{
    public bool Success { get; init; }
    public JoinGameReturnStatus Status { get; init; }
    public BoardDto? BoardDto { get; init; }

    public static JoinGameResponse Failed(JoinGameReturnStatus status)
        => new() { Success = false, Status = status };

    public static JoinGameResponse Ok(BoardDto boardDto)
        => new()
        {
            Success = true,
            Status = JoinGameReturnStatus.GameJoinSuccess,
            BoardDto = boardDto
        };
}

public record AssignMarkResponse
{
    public bool Success { get; init; }
    public AssignMarkStatus Status { get; init; }
    public char? Mark { get; init; }

    public static AssignMarkResponse Failed(AssignMarkStatus status)
        => new() { Success = false, Status = status};

    public static AssignMarkResponse Ok(char mark)
        => new()
        {
            Success = true,
            Status = AssignMarkStatus.Success,
            Mark = mark
        };
}

public record MoveResponse
{
    public bool Success { get; init; }
    public MoveReturnStatus Status { get; init; }
    public BoardDto? UpdatedBoardDto { get; init; }
    public bool IsGameOver { get; init; }
    public char? WinnerMark { get; init; }

    public static MoveResponse Failed(MoveReturnStatus status)
        => new() { Success = false, Status = status };

    public static MoveResponse Ok(BoardDto boardDto, bool isGameOver = false, char? winnerMark = null)
        => new()
        {
            Success = true,
            Status = MoveReturnStatus.MoveSuccess,
            UpdatedBoardDto = boardDto,
            IsGameOver = isGameOver,
            WinnerMark = winnerMark
        };
}

public record BoardResponse
{
    public bool Success { get; init; }
    public BoardReturnStatus Status { get; init; }
    public BoardDto? BoardDto { get; init; }

    public static BoardResponse Failed(BoardReturnStatus status)
        => new() { Success = false, Status = status };

    public static BoardResponse Ok(BoardReturnStatus status, BoardDto boardDto)
        => new() { Success = true, Status = status, BoardDto = boardDto };
}