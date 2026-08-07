using TicTacToeOnline.GameEngine;
using TicTacToeOnline.Enums;

namespace TicTacToeOnline.Data;

public interface IGameStore
{
    public (bool, GameStoreReturnStatus) CreateBoard();
    public (Board?, GameStoreReturnStatus) GetBoardById(string boardId);
    public (bool, GameStoreReturnStatus) RemoveBoardById(string boardId);
}