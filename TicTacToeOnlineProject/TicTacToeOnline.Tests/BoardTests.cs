using TicTacToeOnline.Enums;
using TicTacToeOnline.GameEngine;

namespace TicTacToeOnline.Tests;

public class BoardTests
{
    private readonly Board _board = new();

    [Fact]
    public void GetCell_ReturnsNoCharAndIndexOutOfRangeForIndexNine()
    {
        const int index = 9;

        var (playerMark, boardOperationStatus) = _board.GetCell(index);

        Assert.Equal('\0', playerMark);
        Assert.Equal(BoardOperationStatus.IndexOutOfRange, boardOperationStatus);
    }

    [Fact]
    public void GetCell_ReturnsNoCharAndIndexOutOfRangeForIndexNegativeOne()
    {
        const int index = -1;

        var (playerMark, boardOperationStatus) = _board.GetCell(index);

        Assert.Equal('\0', playerMark);
        Assert.Equal(BoardOperationStatus.IndexOutOfRange, boardOperationStatus);
    }

    [Fact]
    public void GetCell_ReturnsNoCharAndSuccess()
    {
        const int index = 0;

        var (playerMark, boardOperationStatus) = _board.GetCell(index);

        Assert.Equal('\0', playerMark);
        Assert.Equal(BoardOperationStatus.Success, boardOperationStatus);
    }

    [Fact]
    public void GetCell_ReturnsXAndSuccess()
    {
        const int index = 0;
        _board.TryMakeMove(index, 'X');

        var (playerMark, boardOperationStatus) = _board.GetCell(index);

        Assert.Equal('X', playerMark);
        Assert.Equal(BoardOperationStatus.Success, boardOperationStatus);
    }

    [Fact]
    public void TryMakeMove_ReturnsSuccess()
    {
        const int index = 0;
        var boardOperationStatus = _board.TryMakeMove(index, 'X');

        Assert.Equal(BoardOperationStatus.Success, boardOperationStatus);
    }

    [Fact]
    public void TryMakeMove_ReturnsIndexOutOfRangeForIndexNine()
    {
        const int index = 9;

        var boardOperationStatus = _board.TryMakeMove(index, 'X');

        Assert.Equal(BoardOperationStatus.IndexOutOfRange, boardOperationStatus);
    }

    [Fact]
    public void TryMakeMove_ReturnsIndexOutOfRangeForIndexNegativeOne()
    {
        const int index = -1;

        var boardOperationStatus = _board.TryMakeMove(index, 'X');

        Assert.Equal(BoardOperationStatus.IndexOutOfRange, boardOperationStatus);
    }

    [Fact]
    public void TryMakeMove_ReturnsCellNotEmptyWithInputX()
    {
        const int index = 0;
        _board.TryMakeMove(index, 'X');

        var boardOperationStatus = _board.TryMakeMove(index, 'X');

        Assert.Equal(BoardOperationStatus.CellNotEmpty, boardOperationStatus);
    }

    [Fact]
    public void TryMakeMove_ReturnsCellNotEmptyWithInputO()
    {
        const int index = 0;
        _board.TryMakeMove(index, 'X');

        var boardOperationStatus = _board.TryMakeMove(index, 'O');

        Assert.Equal(BoardOperationStatus.CellNotEmpty, boardOperationStatus);
    }

    [Fact]
    public void CheckForWin_TopRowXWin()
    {
        _board.TryMakeMove(0, 'X');
        _board.TryMakeMove(1, 'X');
        _board.TryMakeMove(2, 'X');

        var (win, playerMark) = _board.CheckForWin();

        Assert.True(win);
        Assert.Equal('X', playerMark);
    }

    [Fact]
    public void CheckForWin_TopRowXWinWithSomeOInput()
    {
        _board.TryMakeMove(3, 'O');
        _board.TryMakeMove(4, 'O');
        _board.TryMakeMove(9, 'O');

        _board.TryMakeMove(0, 'X');
        _board.TryMakeMove(1, 'X');
        _board.TryMakeMove(2, 'X');

        var (win, playerMark) = _board.CheckForWin();

        Assert.True(win);
        Assert.Equal('X', playerMark);
    }

    [Fact]
    public void CheckForWin_MiddleRowOWin()
    {
        _board.TryMakeMove(3, 'O');
        _board.TryMakeMove(4, 'O');
        _board.TryMakeMove(5, 'O');

        var (win, playerMark) = _board.CheckForWin();

        Assert.True(win);
        Assert.Equal('O', playerMark);
    }

    [Fact]
    public void CheckForWin_MiddleRowOWinWithSomeXInput()
    {
        _board.TryMakeMove(0, 'X');
        _board.TryMakeMove(1, 'X');
        _board.TryMakeMove(6, 'X');
        _board.TryMakeMove(8, 'X');

        _board.TryMakeMove(3, 'O');
        _board.TryMakeMove(4, 'O');
        _board.TryMakeMove(5, 'O');

        var (win, playerMark) = _board.CheckForWin();

        Assert.True(win);
        Assert.Equal('O', playerMark);
    }

    [Fact]
    public void CheckForWin_BottomRowXWin()
    {
        _board.TryMakeMove(6, 'X');
        _board.TryMakeMove(7, 'X');
        _board.TryMakeMove(8, 'X');

        var (win, playerMark) = _board.CheckForWin();

        Assert.True(win);
        Assert.Equal('X', playerMark);
    }

    [Fact]
    public void CheckForWin_BottomRowXWinWithSomeOInput()
    {
        _board.TryMakeMove(0, 'O');
        _board.TryMakeMove(3, 'O');
        _board.TryMakeMove(5, 'O');

        _board.TryMakeMove(6, 'X');
        _board.TryMakeMove(7, 'X');
        _board.TryMakeMove(8, 'X');

        var (win, playerMark) = _board.CheckForWin();

        Assert.True(win);
        Assert.Equal('X', playerMark);
    }

    [Fact]
    public void CheckForWin_FirstColumnXWins()
    {
        _board.TryMakeMove(0, 'X');
        _board.TryMakeMove(3, 'X');
        _board.TryMakeMove(6, 'X');

        var (win, playerMark) = _board.CheckForWin();

        Assert.True(win);
        Assert.Equal('X', playerMark);
    }

    [Fact]
    public void CheckForWin_FirstColumnXWinsWithSomeOInput()
    {
        _board.TryMakeMove(1, 'O');
        _board.TryMakeMove(2, 'O');
        _board.TryMakeMove(4, 'O');
        _board.TryMakeMove(8, 'O');

        _board.TryMakeMove(0, 'X');
        _board.TryMakeMove(3, 'X');
        _board.TryMakeMove(6, 'X');

        var (win, playerMark) = _board.CheckForWin();

        Assert.True(win);
        Assert.Equal('X', playerMark);
    }

    [Fact]
    public void CheckForWin_SecondColumnOWins()
    {
        _board.TryMakeMove(1, 'O');
        _board.TryMakeMove(4, 'O');
        _board.TryMakeMove(7, 'O');

        var (win, playerMark) = _board.CheckForWin();

        Assert.True(win);
        Assert.Equal('O', playerMark);
    }

    [Fact]
    public void CheckForWin_SecondColumnOWinsWithSomeXInput()
    {
        _board.TryMakeMove(0, 'X');
        _board.TryMakeMove(3, 'X');
        _board.TryMakeMove(2, 'X');
        _board.TryMakeMove(5, 'X');

        _board.TryMakeMove(1, 'O');
        _board.TryMakeMove(4, 'O');
        _board.TryMakeMove(7, 'O');

        var (win, playerMark) = _board.CheckForWin();

        Assert.True(win);
        Assert.Equal('O', playerMark);
    }

    [Fact]
    public void CheckForWin_LastColumnOWinsWithSomeXInput()
    {
        _board.TryMakeMove(1, 'X');
        _board.TryMakeMove(3, 'X');
        _board.TryMakeMove(4, 'X');
        _board.TryMakeMove(6, 'X');

        _board.TryMakeMove(2, 'O');
        _board.TryMakeMove(5, 'O');
        _board.TryMakeMove(8, 'O');

        var (win, playerMark) = _board.CheckForWin();

        Assert.True(win);
        Assert.Equal('O', playerMark);
    }

    [Fact]
    public void CheckForWin_LeftToRightXWins()
    {
        _board.TryMakeMove(0, 'X');
        _board.TryMakeMove(4, 'X');
        _board.TryMakeMove(8, 'X');

        var (win, playerMark) = _board.CheckForWin();

        Assert.True(win);
        Assert.Equal('X', playerMark);
    }

    [Fact]
    public void CheckForWin_LeftToRightXWinsWithSomeOInput()
    {
        _board.TryMakeMove(1, 'O');
        _board.TryMakeMove(2, 'O');
        _board.TryMakeMove(3, 'O');
        _board.TryMakeMove(5, 'O');

        _board.TryMakeMove(0, 'X');
        _board.TryMakeMove(4, 'X');
        _board.TryMakeMove(8, 'X');

        var (win, playerMark) = _board.CheckForWin();

        Assert.True(win);
        Assert.Equal('X', playerMark);
    }

    [Fact]
    public void CheckForWin_RightToLeftOWins()
    {
        _board.TryMakeMove(2, 'O');
        _board.TryMakeMove(4, 'O');
        _board.TryMakeMove(6, 'O');

        var (win, playerMark) = _board.CheckForWin();

        Assert.True(win);
        Assert.Equal('O', playerMark);
    }

    [Fact]
    public void CheckForWin_RightToLeftOWinsWithSomeXInput()
    {
        _board.TryMakeMove(1, 'X');
        _board.TryMakeMove(5, 'X');
        _board.TryMakeMove(7, 'X');

        _board.TryMakeMove(2, 'O');
        _board.TryMakeMove(4, 'O');
        _board.TryMakeMove(6, 'O');

        var (win, playerMark) = _board.CheckForWin();

        Assert.True(win);
        Assert.Equal('O', playerMark);
    }

    [Fact]
    public void CheckForWin_ReturnsFalseAndNoCharOnFullBoard()
    {
        _board.TryMakeMove(0, 'X');
        _board.TryMakeMove(1, 'O');
        _board.TryMakeMove(2, 'X');
        _board.TryMakeMove(3, 'O');
        _board.TryMakeMove(4, 'X');
        _board.TryMakeMove(5, 'O');
        _board.TryMakeMove(6, 'O');
        _board.TryMakeMove(7, 'X');
        _board.TryMakeMove(8, 'O');

        var (win, playerMark) = _board.CheckForWin();

        Assert.False(win);
        Assert.Equal('\0', playerMark);
    }

    [Fact]
    public void CheckForWin_ReturnsFalseAndNoCharOnEmptyBoard()
    {
        var (win, playerMark) = _board.CheckForWin();

        Assert.False(win);
        Assert.Equal('\0', playerMark);
    }

    [Fact]
    public void CheckForWin_ReturnsFalseAndNoCharOnPartiallyFilledBoard()
    {

        _board.TryMakeMove(2, 'X');
        _board.TryMakeMove(3, 'O');
        _board.TryMakeMove(4, 'X');

        _board.TryMakeMove(7, 'X');
        _board.TryMakeMove(8, 'O');

        var (win, playerMark) = _board.CheckForWin();

        Assert.False(win);
        Assert.Equal('\0', playerMark);
    }
}