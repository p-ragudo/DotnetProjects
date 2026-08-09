using TicTacToeOnline.Data;
using TicTacToeOnline.Dto;
using TicTacToeOnline.Enums;
using TicTacToeOnline.GameEngine;

namespace TicTacToeOnline.Services;

public class BoardService
{
    private readonly IGameStore _activeGames;

    public BoardService(IGameStore activeGames)
    {
        _activeGames = activeGames;
    }

    public async Task<JoinGameResponse> JoinGameAsync(string boardId)
    {
        var (board, returnStatus) = _activeGames.GetBoardById(boardId);
        if (returnStatus == GameStoreReturnStatus.BoardDoesNotExist)
        {
            return JoinGameResponse.Failed(JoinGameErrorType.BoardNotFound);
        }
        if (board == null)
        {
            Console.Error.WriteLine("BoardNullException at TicTacToeOnline.BoardService.JoinGameAsync");
            return JoinGameResponse.Failed(JoinGameErrorType.BoardNullException);
        }

        return JoinGameResponse.Ok(board);
    }

    public async Task<MoveResult> MakeMoveAsync(string boardId, int move, char playerMark)
    {
        var (board, returnStatus) = _activeGames.GetBoardById(boardId);
        if (returnStatus == GameStoreReturnStatus.BoardDoesNotExist)
        {
            return MoveResult.Failed(MoveErrorType.BoardNotFound);
        }
        if (board == null)
        {
            Console.Error.WriteLine("BoardNullException at TicTacToeOnline.BoardService.MakeMoveAsync");
            return MoveResult.Failed(MoveErrorType.BoardNullException);
        }

        var boardOperationReturnStatus = board.TryMakeMove(move, playerMark);
        if (boardOperationReturnStatus == BoardOperationStatus.IndexOutOfRange)
        {
            return MoveResult.Failed(MoveErrorType.IndexOutOfRange);
        }
        if (boardOperationReturnStatus == BoardOperationStatus.CellNotEmpty)
        {
            return MoveResult.Failed(MoveErrorType.CellNotEmpty);
        }

        var (isWin, winningMark) = board.CheckForWin();

        return MoveResult.Ok(board, isWin, isWin ? winningMark : null);
    }
}