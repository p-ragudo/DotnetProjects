namespace TicTacToeOnline.GameEngine;

public class Board
{
    public readonly char[] _cells = new char[9];

    public char[] GetBoard() => (char[])_cells.Clone();
    public string BoardId { get; } = Guid.NewGuid().ToString();

    public (char, string) GetCell(int index)
    {
        if (index is < 0 or > 8)
        {
            return ('\0', "Index out of range");
        }

        return (_cells[index], "Cell fetch success");
    }

    public (bool, string) TryMakeMove(int index, char playerMark)
    {
        if (index is < 0 or > 8)
        {
            return (false, "Index out of range");
        }

        if (_cells[index] != 0)
        {
            return (false, "Cell not empty");
        }

        _cells[index] = playerMark;
        return (true, "Move success");
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

            start ++;
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