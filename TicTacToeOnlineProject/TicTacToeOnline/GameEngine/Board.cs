using TicTacToeOnline.Enums;
using TicTacToeOnline.Utils;

namespace TicTacToeOnline.GameEngine;

public class Board
{
    public readonly char[] _cells = new char[9];

    public char[] GetBoard() => (char[])_cells.Clone();
    public string Id { get; } = CodeGenerator.Generate();

    public (char, BoardOperationStatus) GetCell(int index)
    {
        if (index is < 0 or > 8)
        {
            return ('\0', BoardOperationStatus.IndexOutOfRange);
        }

        return (_cells[index], BoardOperationStatus.Success);
    }

    /// <summary>
    /// Write operation to an index in a board
    /// </summary>
    /// <param name="index">A number between 0-8 inclusive. Specifies which cell to perform the write operation.</param>
    /// <param name="playerMark">Specifies the character that gets written to a cell in a board</param>
    /// <returns>
    /// <para>BoardOperationReturnStatus: Success, IndexOutOfRange, CellNotEmpty</para>
    /// </returns>
    public BoardOperationStatus TryMakeMove(int index, char playerMark)
    {
        if (index is < 0 or > 8)
        {
            return BoardOperationStatus.IndexOutOfRange;
        }

        if (_cells[index] != 0)
        {
            return BoardOperationStatus.CellNotEmpty;
        }

        _cells[index] = playerMark;
        return BoardOperationStatus.Success;
    }

    /// <summary>
    /// Checks a sequence of 3 cells to check if there is a win
    /// </summary>
    /// <returns>
    /// <para>(bool, int)</para>
    /// <para>bool: 1 = win, 0 = no win</para>
    /// <para>int: 0 = no win, 1 X wins, 2 O wins</para>
    /// </returns>
    private (bool, char) CheckCellRangeWin(int start, int step)
    {
        int xScore = 0;
        int oScore = 0;

        for (int i = 0, indexer = start; i < 3; i++, indexer += step)
        {
            if (_cells[indexer] == 'X')
            {
                xScore++;
            }
            else if (_cells[indexer] == 'O')
            {
                oScore++;
            }
        }

        if (xScore == 3)
        {
            return (true, 'X');
        }
        else if (oScore == 3)
        {
            return (true, 'O');
        }

        return (false, '\0');
    }

    public (bool, char) CheckForWin()
    {
        bool win;
        char piece;

        // Check all horizontal
        for (int i = 0, start = 0, step = 1; i < 3; i++)
        {
            (win, piece) = CheckCellRangeWin(start, step);

            if (win)
            {
                return (true, piece);
            }

            start += 3;
        }

        // Check all vertical
        for (int i = 0, start = 0, step = 3; i < 3; i++)
        {
            (win, piece) = CheckCellRangeWin(start, step);

            if (win)
            {
                return (true, piece);
            }

            start++;
        }

        // Check all diagonal
        for (int i = 0, start = 0, step = 4; i < 2; i++)
        {
            (win, piece) = CheckCellRangeWin(start, step);

            if (win)
            {
                return (true, piece);
            }

            start = 2;
            step = 2;
        }

        return (false, '\0');
    }
}