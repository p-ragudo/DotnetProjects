using TicTacToeOnline.Dto;
using TicTacToeOnline.GameEngine;

namespace TicTacToeOnline.Extensions;

public static class BoardMappingExtensions
{
    private static Random random = new();

    public static BoardDto ToDto(this Board board)
    {
        return new BoardDto
        {
            BoardId = board.Id,
            Grid = board.GetBoard()
                .Select(c => c == '\0' ? string.Empty : c.ToString())
                .ToArray(),
            CurrentTurn = board.CurrentTurn
        };
    }

    public static char GetMark(this Board board)
    {
        if (board.XAssigned)
        {
            board.Assignmark('O');
            return 'O';
        }
        if (board.OAssigned)
        {
            board.Assignmark('X');
            return 'X';
        }

        int randomNum = random.Next(0, 2);

        if (randomNum == 0)
        {
            board.Assignmark('O');
            return 'O';
        }

        board.Assignmark('X');
        return 'X';
    }
}