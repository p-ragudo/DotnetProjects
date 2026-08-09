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
    ErrorCreatingBoard,
    BoardDoesNotExist,
    RemoveBoardSuccess,
    ErrorRemovingBoard
}

public enum JoinGameErrorType
{
    BoardNotFound,
    BoardNullException
}

public enum MoveErrorType
{
    BoardNotFound,
    BoardNullException,
    IndexOutOfRange,
    NotYourTurn,
    CellNotEmpty,
    GameFinished
}