using TicTacToeOnline.Enums;
using TicTacToeOnline.GameEngine;

namespace TicTacToeOnline.Data;

public interface IGameStore
{
    public Task<(Board?, GameStoreReturnStatus)> CreateBoard();
    public Task<(Board?, GameStoreReturnStatus)> GetBoardById(string boardId);
    public Task<(bool, GameStoreReturnStatus)> RemoveBoardById(string boardId);
    public Task<GameStoreReturnStatus> Rematch(string boardId, bool rematch);
}