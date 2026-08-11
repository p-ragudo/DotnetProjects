using System.Collections.Concurrent;
using TicTacToeOnline.GameEngine;
using TicTacToeOnline.Enums;

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
}