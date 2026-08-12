using TicTacToeOnline.Dto;
using TicTacToeOnline.Enums;
using TicTacToeOnline.GameEngine;

namespace TicTacToeOnline.Hubs;

public interface IGameClient
{
    // Sent to the client who just joined to confirm their assigned player mark ('X' or 'O')
    Task PlayerJoined(JoinGameResponse response);
    Task NotifyGroupOnPlayerJoin(string connectionId);

    // Broadcasted to everyone in a game room whenever the board or game status updates
    Task GameUpdated(BoardDto boardDto);
    Task GameOver(BoardDto boardDto, string winnerConnectionId);
    Task SendCurrentTurn(char mark);

    // Sent directly to a single connection if an action fails
    Task ErrorOccured(GameClientResponse response);
}