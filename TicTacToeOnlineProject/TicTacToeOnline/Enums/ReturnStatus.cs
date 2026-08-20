namespace TicTacToeOnline.Enums;

public enum BoardOperationStatus
{
    Success,
    IndexOutOfRange,
    CellNotEmpty,
    NotCurrentTurn
}

public enum AssignMarkStatus
{
    Success,
    InvalidMark,
    AlreadyAssigned,
    BoardNotFound,
    BoardNullException
}

public enum GameStoreReturnStatus
{
    CreateBoardSuccess,
    GetBoardSuccess,
    GameFull,
    ErrorCreatingBoard,
    BoardDoesNotExist,
    RemoveBoardSuccess,
    ErrorRemovingBoard,
    BoardNullException,
    RestartBoardSuccess,
    RematchRequestAccepted,
    RematchAccepted,
    RematchDenyAccepted,
    Waiting,
    RematchDenied,
}

public enum JoinGameReturnStatus
{
    GameJoinSuccess,
    GameFull,
    BoardNotFound,
    BoardNullException
}

public enum MoveReturnStatus
{
    MoveSuccess,
    BoardNotFound,
    BoardNullException,
    IndexOutOfRange,
    NotCurrentTurn,
    CellNotEmpty,
    GameFinished
}

public enum RematchReturnStatus
{
    BoardNotFound,
    BoardNullException,
    Waiting,
    RematchAccepted,
    RematchDenied
}

public enum BoardReturnStatus
{
    BoardFetchSuccess,
    BoardDoesNotExist,
    BoardNullException
}

public enum GameClientResponse
{
    PlayerJoinSuccess,
    BoardUpdated
}