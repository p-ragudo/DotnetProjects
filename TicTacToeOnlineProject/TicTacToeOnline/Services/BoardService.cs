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
        if (returnStatus == GameStoreReturnStatus.ErrorCreatingBoard)
        {
            return CreateGameResponse.Failed(GameStoreReturnStatus.ErrorCreatingBoard);
        }
        if (board == null)
        {
            return CreateGameResponse.Failed(GameStoreReturnStatus.BoardNullException);
        }

        Console.WriteLine($"Created game {board.Id}");
        return CreateGameResponse.Ok(board.ToDto());
    }

    public async Task<BoardResponse> GetBoardByIdAsync(string boardId)
    {
        var (board, returnStatus) = await _gameStore.GetBoardById(boardId);
        if (returnStatus == GameStoreReturnStatus.BoardDoesNotExist)
        {
            return BoardResponse.Failed(BoardReturnStatus.BoardDoesNotExist);
        }
        if (board == null)
        {
            Console.Error.WriteLine("BoardNullException at TicTacToeOnline.BoardService.GetBoardByIdAsync");
            return BoardResponse.Failed(BoardReturnStatus.BoardNullException);
        }

        return BoardResponse.Ok(BoardReturnStatus.BoardFetchSuccess, board.ToDto());
    }

    public async Task<JoinGameResponse> JoinGameAsync(string boardId, string connectionId)
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
        if (board.PlayerIds.Contains(connectionId))
        {
            return JoinGameResponse.Ok(board.ToDto());
        }
        if (board.PlayerIds.Count >= 2)
        {
            return JoinGameResponse.Failed(JoinGameReturnStatus.GameFull);
        }

        board.PlayerIds.Add(connectionId);
        Console.WriteLine($"Client {connectionId} has joined game {board.Id}");
        return JoinGameResponse.Ok(board.ToDto());
    }

    public async Task<AssignMarkResponse> GetMarkAsync(string boardId)
    {
        var (board, returnStatus) = await _gameStore.GetBoardById(boardId);
        if (returnStatus == GameStoreReturnStatus.BoardDoesNotExist)
        {
            return AssignMarkResponse.Failed(AssignMarkStatus.BoardNotFound);
        }
        if (board == null)
        {
            Console.Error.WriteLine("BoardNullException at TicTacToeOnline.BoardService.JoinGameAsync");
            return AssignMarkResponse.Failed(AssignMarkStatus.BoardNullException);
        }

        var mark = board.GetMark();
        return AssignMarkResponse.Ok(mark);
    }

    public async Task<char?> GetCurrentTurn(string boardId)
    {
        var (board, returnStatus) = await _gameStore.GetBoardById(boardId);
        if (returnStatus == GameStoreReturnStatus.BoardDoesNotExist)
        {
            return null;
        }
        if (board == null)
        {
            Console.Error.WriteLine("BoardNullException at TicTacToeOnline.BoardService.GetCurrentTurn");
            return null;
        }

        return board.CurrentTurn;
    }

    public async Task<MoveResponse> MakeMoveAsync(string boardId, int move, char playerMark)
    {
        var (board, returnStatus) = await _gameStore.GetBoardById(boardId);
        if (returnStatus == GameStoreReturnStatus.BoardDoesNotExist)
        {
            return MoveResponse.Failed(MoveReturnStatus.BoardNotFound);
        }
        if (board == null)
        {
            Console.Error.WriteLine("BoardNullException at TicTacToeOnline.BoardService.MakeMoveAsync");
            return MoveResponse.Failed(MoveReturnStatus.BoardNullException);
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
}