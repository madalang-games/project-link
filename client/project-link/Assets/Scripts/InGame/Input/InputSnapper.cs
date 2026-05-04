using UnityEngine;
using Cell = ProjectLink.InGame.Board.Cell;

namespace ProjectLink.InGame.Input
{
    public static class InputSnapper
    {
        public static Cell Snap(Vector2 worldPos, ProjectLink.InGame.Board.Board board, float cellSize)
        {
            float offsetX = (board.Width  - 1) * cellSize * 0.5f;
            float offsetY = (board.Height - 1) * cellSize * 0.5f;

            int x = Mathf.RoundToInt((worldPos.x + offsetX) / cellSize);
            int y = Mathf.RoundToInt((worldPos.y + offsetY) / cellSize);

            x = Mathf.Clamp(x, 0, board.Width  - 1);
            y = Mathf.Clamp(y, 0, board.Height - 1);

            return board.GetCell(x, y);
        }
    }
}
