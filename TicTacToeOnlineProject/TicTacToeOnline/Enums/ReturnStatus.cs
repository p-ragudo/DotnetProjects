namespace TicTacToeOnline.Enums;

public enum BoardOperationStatus
{
    Success,
    IndexOutOfRange,
    CellNotEmpty
}

public enum GameStoreReturnStatus
{
    CreateBoardSuccess,
    GetBoardSuccess,
    ErrorCreatingBoard,
    BoardDoesNotExist,
    RemoveBoardSuccess,
    ErrorRemovingBoard,
    BoardNullException
}

public enum JoinGameReturnStatus
{
    GameJoinSuccess,
    BoardNotFound,
    BoardNullException
}

public enum MoveErrorReturnStatus
{
    MoveSuccess,
    BoardNotFound,
    BoardNullException,
    IndexOutOfRange,
    NotYourTurn,
    CellNotEmpty,
    GameFinished
}

public enum GameClientResponse
{
    PlayerJoinSuccess,
    BoardUpdated
}