using System.Collections.Concurrent;
using TicTacToeOnline.Enums;
using TicTacToeOnline.GameEngine;

namespace TicTacToeOnline.Data;

public class InMemoryGameStore : IGameStore
{
    private readonly ConcurrentDictionary<string, Board> _games = new();

    public async Task<(Board?, GameStoreReturnStatus)> CreateBoard()
    {
        var board = new Board();

        if (_games.TryAdd(board.Id, board))
        {
            return (board, GameStoreReturnStatus.CreateBoardSuccess);
        }
        else
        {
            return (null, GameStoreReturnStatus.ErrorCreatingBoard);
        }
    }

    public async Task<(Board?, GameStoreReturnStatus)> GetBoardById(string boardId)
    {
        if (_games.TryGetValue(boardId, out Board? value))
        {
            return (value, GameStoreReturnStatus.GetBoardSuccess);
        }
        else
        {
            return (null, GameStoreReturnStatus.BoardDoesNotExist);
        }
    }

    public async Task<(bool, GameStoreReturnStatus)> RemoveBoardById(string boardId)
    {
        if (_games.TryRemove(boardId, out _))
        {
            return (true, GameStoreReturnStatus.RemoveBoardSuccess);
        }
        else
        {
            return (false, GameStoreReturnStatus.ErrorRemovingBoard);
        }
    }

    public async Task<GameStoreReturnStatus> Rematch(string boardId, bool rematch)
    {
        if (_games.TryGetValue(boardId, out Board? board))
        {
            var result = board.Rematch(rematch);

            if (result == 2)
            {
                return GameStoreReturnStatus.RematchDenied;
            }
            if (result == 1)
            {
                return GameStoreReturnStatus.Waiting;
            }

            return GameStoreReturnStatus.RematchAccepted;

        }
        else
        {
            return GameStoreReturnStatus.BoardDoesNotExist;
        }
    }
}