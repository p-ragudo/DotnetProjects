using TicTacToeOnline.Enums;
using TicTacToeOnline.GameEngine;

namespace TicTacToeOnline.Dto;

public record BoardDto
{
    public string BoardId { get; init; } = string.Empty;
    public string[] Grid { get; init; } = new string[9];
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

public record MoveResult
{
    public bool Success { get; init; }
    public MoveErrorReturnStatus Status { get; init; }
    public BoardDto? UpdatedBoardDto { get; init; }
    public bool IsGameOver { get; init; }
    public char? WinnerId { get; init; }

    public static MoveResult Failed(MoveErrorReturnStatus status)
        => new() { Success = false, Status = status };

    public static MoveResult Ok(BoardDto boardDto, bool isGameOver = false, char? winnerId = null)
        => new()
        {
            Success = true,
            Status = MoveErrorReturnStatus.MoveSuccess,
            UpdatedBoardDto = boardDto,
            IsGameOver = isGameOver,
            WinnerId = winnerId
        };
}