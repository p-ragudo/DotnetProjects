using TicTacToeOnline.Enums;
using TicTacToeOnline.Utils;

namespace TicTacToeOnline.GameEngine;

public class Board
{
    public readonly char[] _cells = new char[9];

    public char[] GetBoard() => (char[])_cells.Clone();
    public string Id { get; } = CodeGenerator.Generate();
    public char CurrentTurn { get; private set; } = 'X';

    public Dictionary<string, char> PlayerMarks { get; } = [];
    public int RematchAgreeCount = 0;
    public int RematchDisagreeCount = 0;
    public int PlayersPresent => PlayerMarks.Count;

    public (char, BoardOperationStatus) GetCell(int index)
    {
        if (index is < 0 or > 8)
        {
            return ('\0', BoardOperationStatus.IndexOutOfRange);
        }

        return (_cells[index], BoardOperationStatus.Success);
    }

    public void SwitchTurn()
    {
        if (CurrentTurn == 'X')
        {
            CurrentTurn = 'O';
        }
        else
        {
            CurrentTurn = 'X';
        }
    }

    /// <summary>
    /// Assigns an available mark or retrieves the mark already owned by this connection.
    /// </summary>
    public (char Mark, AssignMarkStatus Status) GetOrAssignMark(string connectionId)
    {
        // If this player already has a mark, return it
        if (PlayerMarks.TryGetValue(connectionId, out var existingMark))
        {
            return (existingMark, AssignMarkStatus.Success);
        }

        if (PlayerMarks.Count >= 2)
        {
            return ('\0', AssignMarkStatus.AlreadyAssigned);
        }

        // Determine which mark is still free
        bool hasX = PlayerMarks.ContainsValue('X');
        bool hasO = PlayerMarks.ContainsValue('O');

        char assignedMark;
        if (!hasX && !hasO)
        {
            assignedMark = Random.Shared.Next(2) == 0 ? 'X' : 'O';
        }
        else
        {
            assignedMark = hasX ? 'O' : 'X';
        }

        PlayerMarks[connectionId] = assignedMark;
        return (assignedMark, AssignMarkStatus.Success);
    }

    public void RemovePlayer(string connectionId)
    {
        PlayerMarks.Remove(connectionId);
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
        if (playerMark != CurrentTurn)
        {
            return BoardOperationStatus.NotCurrentTurn;
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

        // Check for draw (all cells filled, no winner)
        bool isBoardFull = Array.TrueForAll(_cells, cell => cell != '\0' && cell != 0);
        if (isBoardFull)
        {
            return (true, 'D'); // 'D' represents Draw
        }

        return (false, '\0');
    }

    public void RestartBoard()
    {
        Array.Clear(_cells, 0, _cells.Length);
        RematchAgreeCount = 0;
        RematchDisagreeCount = 0;
        CurrentTurn = 'X';

        // Randomize marks between the 2 connected players
        if (PlayerMarks.Count == 2)
        {
            var keys = PlayerMarks.Keys.ToList();
            char firstMark = Random.Shared.Next(2) == 0 ? 'X' : 'O';
            char secondMark = firstMark == 'X' ? 'O' : 'X';

            PlayerMarks[keys[0]] = firstMark;
            PlayerMarks[keys[1]] = secondMark;
        }
    }

    /// <summary>
    /// Restarts boards if both players agree
    /// </summary>
    /// <returns>
    /// <para>0 if both players agree</para>
    /// <para>1 if a player agrees and the other has not made a decision</para>
    /// <para>2 if a player disagrees</para>
    /// </returns>
    public int Rematch(bool rematch)
    {
        if (rematch)
        {
            RematchAgreeCount++;

            if (RematchAgreeCount >= 2) return 0;
            return 1;
        }
        else
        {
            RematchDisagreeCount++;
            return 2;
        }
    }
}