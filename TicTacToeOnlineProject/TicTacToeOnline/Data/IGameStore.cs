using TicTacToeOnline.GameEngine;
using TicTacToeOnline.Enums;

namespace TicTacToeOnline.Data;

public interface IGameStore
{
    public Task<(Board?, GameStoreReturnStatus)> CreateBoard();
    public Task<(Board?, GameStoreReturnStatus)> GetBoardById(string boardId);
    public Task<(bool, GameStoreReturnStatus)> RemoveBoardById(string boardId);
}