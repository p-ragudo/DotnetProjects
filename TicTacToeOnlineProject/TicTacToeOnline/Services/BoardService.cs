using TicTacToeOnline.Data;
using TicTacToeOnline.Dto;
using TicTacToeOnline.Enums;
using TicTacToeOnline.Extensions;

namespace TicTacToeOnline.Services;

public class BoardService
{
    private readonly IGameStore _gameStore;

    public BoardService(IGameStore activeGames)
    {
        _gameStore = activeGames;
    }

    public async Task<CreateGameResponse> CreateGameAsync()
    {
        var (board, returnStatus) = await _gameStore.CreateBoard();
        if (returnStatus == GameStoreReturnStatus.ErrorCreatingBoard)
        {
            return CreateGameResponse.Failed(GameStoreReturnStatus.ErrorCreatingBoard);
        }
        if (board == null)
        {
            return CreateGameResponse.Failed(GameStoreReturnStatus.BoardNullException);
        }

        return CreateGameResponse.Ok(board.ToDto());
    }

    public async Task<JoinGameResponse> JoinGameAsync(string boardId)
    {
        var (board, returnStatus) = await _gameStore.GetBoardById(boardId);
        if (returnStatus == GameStoreReturnStatus.BoardDoesNotExist)
        {
            return JoinGameResponse.Failed(JoinGameReturnStatus.BoardNotFound);
        }
        if (board == null)
        {
            Console.Error.WriteLine("BoardNullException at TicTacToeOnline.BoardService.JoinGameAsync");
            return JoinGameResponse.Failed(JoinGameReturnStatus.BoardNullException);
        }

        return JoinGameResponse.Ok(board.ToDto());
    }

    public async Task<MoveResult> MakeMoveAsync(string boardId, int move, char playerMark)
    {
        var (board, returnStatus) = await _gameStore.GetBoardById(boardId);
        if (returnStatus == GameStoreReturnStatus.BoardDoesNotExist)
        {
            return MoveResult.Failed(MoveErrorReturnStatus.BoardNotFound);
        }
        if (board == null)
        {
            Console.Error.WriteLine("BoardNullException at TicTacToeOnline.BoardService.MakeMoveAsync");
            return MoveResult.Failed(MoveErrorReturnStatus.BoardNullException);
        }

        var boardOperationReturnStatus = board.TryMakeMove(move, playerMark);
        if (boardOperationReturnStatus == BoardOperationStatus.IndexOutOfRange)
        {
            return MoveResult.Failed(MoveErrorReturnStatus.IndexOutOfRange);
        }
        if (boardOperationReturnStatus == BoardOperationStatus.CellNotEmpty)
        {
            return MoveResult.Failed(MoveErrorReturnStatus.CellNotEmpty);
        }

        var (isWin, winningMark) = board.CheckForWin();

        return MoveResult.Ok(board.ToDto(), isWin, isWin ? winningMark : null);
    }
}