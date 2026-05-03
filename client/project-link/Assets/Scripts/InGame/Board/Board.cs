using System.Collections.Generic;
using ProjectLink.Data;

namespace ProjectLink.InGame.Board
{
    public class Board
    {
        public int Width  { get; }
        public int Height { get; }

        public IReadOnlyCollection<int> ColorIds => _colorIds;

        readonly Cell[,]      _cells;
        readonly HashSet<int> _colorIds = new HashSet<int>();

        public Board(StageData stageData)
        {
            Width  = stageData.Info.width;
            Height = stageData.Info.height;
            _cells = new Cell[Width, Height];

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    _cells[x, y] = new Cell(x, y);

            foreach (var node in stageData.Nodes)
            {
                _cells[node.x, node.y].SetNode(node.colorId);
                _colorIds.Add(node.colorId);
            }
        }

        public Cell GetCell(int x, int y) => _cells[x, y];

        public bool IsInBounds(int x, int y) =>
            x >= 0 && x < Width && y >= 0 && y < Height;

        public IEnumerable<Cell> GetAdjacentCells(int x, int y)
        {
            if (IsInBounds(x,     y - 1)) yield return _cells[x,     y - 1];
            if (IsInBounds(x,     y + 1)) yield return _cells[x,     y + 1];
            if (IsInBounds(x - 1, y    )) yield return _cells[x - 1, y    ];
            if (IsInBounds(x + 1, y    )) yield return _cells[x + 1, y    ];
        }

        public void SetPath(int x, int y, int colorId)
        {
            _cells[x, y].SetPath(colorId);
        }

        public void ClearPathCells(int colorId)
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    if (_cells[x, y].IsPath && _cells[x, y].ColorId == colorId)
                        _cells[x, y].Clear();
        }
    }
}
