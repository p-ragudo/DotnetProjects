using TicTacToeOnline.GameEngine;

namespace TicTacToeOnline.Tests;

public class BoardTests
{
    private readonly Board _board = new();

    [Fact]
    public void GetCell_ReturnsNoCharAndErrorMessageForIndexNine()
    {
        const int index = 9;

        var (playerMark, message) = _board.GetCell(index);

        Assert.Equal('\0', playerMark);
        Assert.Equal("Index out of range", message);
    }

    [Fact]
    public void GetCell_ReturnsNoCharAndErrorMessageForIndexNegativeOne()
    {
        const int index = -1;

        var (playerMark, message) = _board.GetCell(index);

        Assert.Equal('\0', playerMark);
        Assert.Equal("Index out of range", message);
    }

    [Fact]
    public void GetCell_ReturnsNoCharAndSuccessMessage()
    {
        const int index = 0;

        var (playerMark, message) = _board.GetCell(index);

        Assert.Equal('\0', playerMark);
        Assert.Equal("Cell fetch success", message);
    }

    [Fact]
    public void GetCell_ReturnsXAndSuccessMessage()
    {
        const int index = 0;
        _board.TryMakeMove(index, 'X');

        var (playerMark, message) = _board.GetCell(index);

        Assert.Equal('X', playerMark);
        Assert.Equal("Cell fetch success", message);
    }

    [Fact]
    public void TryMakeMove_ReturnsTrueAndSuccessMessage()
    {
        const int index = 0;
        var (isSuccessful, message) = _board.TryMakeMove(index, 'X');

        Assert.True(isSuccessful);
        Assert.Equal("Move success", message);
    }

    [Fact]
    public void TryMakeMove_ReturnsFalseAndIndexOutOfRangeMessageForIndexNine()
    {
        const int index = 9;

        var (isSuccessful, message) = _board.TryMakeMove(index, 'X');

        Assert.False(isSuccessful);
        Assert.Equal("Index out of range", message);
    }

    [Fact]
    public void TryMakeMove_ReturnsFalseAndIndexOutOfRangeMessageForIndexNegativeOne()
    {
        const int index = -1;

        var (isSuccessful, message) = _board.TryMakeMove(index, 'X');

        Assert.False(isSuccessful);
        Assert.Equal("Index out of range", message);
    }

    [Fact]
    public void TryMakeMove_ReturnsFalseAndCellNotEmptyMessageWithInputX()
    {
        const int index = 0;
        _board.TryMakeMove(index, 'X');

        var (isSuccessful, message) = _board.TryMakeMove(index, 'X');

        Assert.False(isSuccessful);
        Assert.Equal("Cell not empty", message);
    }

    [Fact]
    public void TryMakeMove_ReturnsFalseAndCellNotEmptyMessageWithInputO()
    {
        const int index = 0;
        _board.TryMakeMove(index, 'X');

        var (isSuccessful, message) = _board.TryMakeMove(index, 'O');

        Assert.False(isSuccessful);
        Assert.Equal("Cell not empty", message);
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