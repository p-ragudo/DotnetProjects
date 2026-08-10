using TicTacToeOnline.Dto;
using TicTacToeOnline.Enums;

namespace TicTacToeOnline.Hubs;

public interface IGameClient
{
    // Sent to the client who just joined to confirm their assigned player mark ('X' or 'O')
    Task PlayerJoined(JoinGameResponse response);
    Task NotifyGroupOnPlayerJoin(string connectionId);

    // Broadcasted to everyone in a game room whenever the board or game status updates
    Task GameUpdated(GameClientResponse response, char[]? boardState);

    // Sent directly to a single connection if an action fails
    Task ErrorOccured(GameClientResponse response);
}