using TicTacToeOnline.Dto;
using TicTacToeOnline.Enums;

namespace TicTacToeOnline.Hubs;

public interface IGameClient
{
    // Sent to the client who just joined to confirm their assigned player mark ('X' or 'O')
    Task PlayerJoined(JoinGameResponse response);
    Task NotifyGroupOnPlayerJoin();

    // Broadcasted to everyone in a game room whenever the board or game status updates
    Task GameUpdated(BoardDto boardDto);
    Task GameOver(BoardDto boardDto, char winnerMark);
    Task RematchRequest(RematchResponse response);
    // Sent directly to a single connection if an action fails
    Task NotifyGroupOnPlayerLeave();
    Task ErrorOccured(GameClientResponse response);
}