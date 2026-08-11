using TicTacToeOnline.Dto;
using TicTacToeOnline.GameEngine;

namespace TicTacToeOnline.Extensions;

public static class BoardMappingExtensions
{
    public static BoardDto ToDto(this Board board)
    {
        return new BoardDto
        {
            BoardId = board.Id,
            Grid = board.GetBoard()
                .Select(c => c == '\0' ? string.Empty : c.ToString())
                .ToArray()
        };
    }
}