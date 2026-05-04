using UnityEngine;

namespace ProjectLink.Utils
{
    public static class GridUtils
    {
        public static Vector2 CellToWorld(int x, int y, int width, int height, float cellSize)
        {
            float offsetX = (width  - 1) * cellSize * 0.5f;
            float offsetY = (height - 1) * cellSize * 0.5f;
            return new Vector2(x * cellSize - offsetX, y * cellSize - offsetY);
        }
    }
}
