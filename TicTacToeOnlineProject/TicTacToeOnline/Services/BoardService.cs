using TicTacToeOnline.Data;
using TicTacToeOnline.Dto;
using TicTacToeOnline.Enums;
using TicTacToeOnline.Extensions;
using TicTacToeOnline.GameEngine;

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

        if (board == null)
        {
            return CreateGameResponse.Failed(GameStoreReturnStatus.BoardNullException);
        }
        if (returnStatus == GameStoreReturnStatus.ErrorCreatingBoard)
        {
            return CreateGameResponse.Failed(GameStoreReturnStatus.ErrorCreatingBoard);
        }

        Console.WriteLine($"Created game {board.Id}");
        return CreateGameResponse.Ok(board.ToDto());
    }

    public async Task<BoardResponse> GetBoardByIdAsync(string boardId)
    {
        var (board, returnStatus) = await _gameStore.GetBoardById(boardId);

        if (board == null)
        {
            Console.Error.WriteLine("BoardNullException at TicTacToeOnline.BoardService.GetBoardByIdAsync");
            return BoardResponse.Failed(BoardReturnStatus.BoardNullException);
        }
        if (returnStatus == GameStoreReturnStatus.BoardDoesNotExist)
        {
            return BoardResponse.Failed(BoardReturnStatus.BoardDoesNotExist);
        }

        return BoardResponse.Ok(BoardReturnStatus.BoardFetchSuccess, board.ToDto());
    }

    public async Task<JoinGameResponse> JoinGameAsync(string boardId, string connectionId)
    {
        var (board, returnStatus) = await _gameStore.GetBoardById(boardId);

        if (returnStatus == GameStoreReturnStatus.BoardDoesNotExist || board == null)
        {
            return JoinGameResponse.Failed(JoinGameReturnStatus.BoardNotFound);
        }
        
        if (board.PlayerMarks.ContainsKey(connectionId))
        {
            return JoinGameResponse.Ok(board.ToDto());
        }

        if (board.PlayersPresent >= 2)
        {
            return JoinGameResponse.Failed(JoinGameReturnStatus.GameFull);
        }

        // Assign mark upon join
        board.GetOrAssignMark(connectionId);
        Console.WriteLine($"Client {connectionId} joined game {board.Id}");

        return JoinGameResponse.Ok(board.ToDto());
    }

    public async Task<AssignMarkResponse> GetMarkAsync(string boardId, string connectionId)
    {
        var (board, returnStatus) = await _gameStore.GetBoardById(boardId);

        if (board == null)
        {
            Console.Error.WriteLine("BoardNullException at TicTacToeOnline.BoardService.JoinGameAsync");
            return AssignMarkResponse.Failed(AssignMarkStatus.BoardNullException);
        }
        if (returnStatus == GameStoreReturnStatus.BoardDoesNotExist) return AssignMarkResponse.Failed(AssignMarkStatus.BoardNotFound);

        var (mark, status) = board.GetOrAssignMark(connectionId);
        if (status != AssignMarkStatus.Success)
        {
            return AssignMarkResponse.Failed(status);
        }

        return AssignMarkResponse.Ok(mark);
    }

    public async Task<char?> GetCurrentTurn(string boardId)
    {
        var (board, returnStatus) = await _gameStore.GetBoardById(boardId);

        if (board == null)
        {
            Console.Error.WriteLine("BoardNullException at TicTacToeOnline.BoardService.GetCurrentTurn");
            return null;
        }
        if (returnStatus == GameStoreReturnStatus.BoardDoesNotExist)
        {
            return null;
        }

        return board.CurrentTurn;
    }

    public async Task<MoveResponse> MakeMoveAsync(string boardId, int move, char playerMark)
    {
        var (board, returnStatus) = await _gameStore.GetBoardById(boardId);

        if (board == null)
        {
            Console.Error.WriteLine("BoardNullException at TicTacToeOnline.BoardService.MakeMoveAsync");
            return MoveResponse.Failed(MoveReturnStatus.BoardNullException);
        }
        if (returnStatus == GameStoreReturnStatus.BoardDoesNotExist)
        {
            return MoveResponse.Failed(MoveReturnStatus.BoardNotFound);
        }

        var boardOperationReturnStatus = board.TryMakeMove(move, playerMark);
        if (boardOperationReturnStatus == BoardOperationStatus.IndexOutOfRange)
        {
            return MoveResponse.Failed(MoveReturnStatus.IndexOutOfRange);
        }
        if (boardOperationReturnStatus == BoardOperationStatus.NotCurrentTurn)
        {
            return MoveResponse.Failed(MoveReturnStatus.NotCurrentTurn);
        }
        if (boardOperationReturnStatus == BoardOperationStatus.CellNotEmpty)
        {
            return MoveResponse.Failed(MoveReturnStatus.CellNotEmpty);
        }

        board.SwitchTurn();
        var (isWin, winningMark) = board.CheckForWin();

        return MoveResponse.Ok(board.ToDto(), isWin, isWin ? winningMark : null);
    }

    public async Task<RematchResponse> RematchAsync(string boardId, bool rematch)
    {
        var (board, returnStatus) = await _gameStore.GetBoardById(boardId);

        if (board == null)
        {
            Console.Error.WriteLine("BoardNullException at TicTacToeOnline.BoardService.RematchAsync");
            return RematchResponse.Failed(RematchReturnStatus.BoardNullException);
        }
        if (returnStatus == GameStoreReturnStatus.BoardDoesNotExist)
        {
            return RematchResponse.Failed(RematchReturnStatus.BoardNotFound);
        }

        var response = board.Rematch(rematch);

        if (response == 2)
        {
            return RematchResponse.Ok(null, RematchReturnStatus.RematchDenied);
        }
        if (response == 1)
        {
            return RematchResponse.Ok(null, RematchReturnStatus.Waiting);
        }

        board.RestartBoard();
        return RematchResponse.Ok(board.ToDto(), RematchReturnStatus.RematchAccepted);
    }

    public async Task TryDeleteBoardById(string boardId, string connectionId)
    {
        var (board, returnStatus) = await _gameStore.GetBoardById(boardId);

        if (board == null || returnStatus == GameStoreReturnStatus.BoardDoesNotExist)
        {
            Console.Error.WriteLine($"Board {boardId} not found or null at TryDeleteBoardById");
            return; // Early return prevents null dereference crash
        }

        // Free up the mark and slot for this connection
        board.RemovePlayer(connectionId);
        Console.WriteLine($"Client {connectionId} left board {boardId}. Remaining players: {board.PlayersPresent}");

        if (board.PlayersPresent <= 0)
        {
            await _gameStore.RemoveBoardById(boardId);
            Console.WriteLine($"Board {boardId} deleted.");
        }
    }
}