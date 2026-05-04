using UnityEngine;
using ProjectLink.Utils;

namespace ProjectLink.InGame.Path
{
    public class PathView : MonoBehaviour
    {
        LineRenderer _line;
        PathModel    _pathModel;
        int          _boardWidth;
        int          _boardHeight;
        float        _cellSize;

        public void Init(PathModel pathModel, int boardWidth, int boardHeight, float cellSize)
        {
            _pathModel   = pathModel;
            _boardWidth  = boardWidth;
            _boardHeight = boardHeight;
            _cellSize    = cellSize;

            _line = gameObject.AddComponent<LineRenderer>();
            _line.sortingLayerName = "Path";
            _line.useWorldSpace    = true;
            _line.startWidth       = cellSize * 0.35f;
            _line.endWidth         = cellSize * 0.35f;
            _line.numCornerVertices = 4;
            _line.numCapVertices    = 4;

            var color = ColorPalette.Get(pathModel.ColorId);
            _line.startColor = color;
            _line.endColor   = color;
            _line.material   = new Material(Shader.Find("Sprites/Default"));

            Refresh();
        }

        public void Refresh()
        {
            var cells = _pathModel.Cells;
            _line.positionCount = cells.Count;
            for (int i = 0; i < cells.Count; i++)
            {
                var pos = GridUtils.CellToWorld(cells[i].X, cells[i].Y, _boardWidth, _boardHeight, _cellSize);
                _line.SetPosition(i, new Vector3(pos.x, pos.y, 0f));
            }
        }
    }
}
