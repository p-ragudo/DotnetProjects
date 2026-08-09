using TicTacToeOnline.Enums;
using TicTacToeOnline.GameEngine;

namespace TicTacToeOnline.Dto;

public record JoinGameResponse
{
    public bool Success { get; init; }
    public JoinGameErrorType Error { get; init; }
    public Board? Board { get; init; }

    public static JoinGameResponse Failed(JoinGameErrorType error)
        => new() { Success = false, Error = error };

    public static JoinGameResponse Ok(Board board)
        => new() { Success = true, Board = board };
}

public record MoveResult
{
    public bool Success { get; init; }
    public MoveErrorType Error { get; init; }
    public Board? UpdatedBoard { get; init; }
    public bool IsGameOver { get; init; }
    public char? WinnerId { get; init; }

    public static MoveResult Failed(MoveErrorType error)
        => new() { Success = false, Error = error };

    public static MoveResult Ok(Board board, bool isGameOver = false, char? winnerId = null)
        => new() { Success = true, UpdatedBoard = board, IsGameOver = isGameOver, WinnerId = winnerId };
}