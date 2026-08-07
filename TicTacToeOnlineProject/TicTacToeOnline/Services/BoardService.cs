using TicTacToeOnline.Data;

namespace TicTacToeOnline.Services;

public class BoardService
{
    private readonly IGameStore _activeGames;

    public BoardService(IGameStore activeGames)
    {
        _activeGames = activeGames;
    }

    
}