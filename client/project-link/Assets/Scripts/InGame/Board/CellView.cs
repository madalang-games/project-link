using UnityEngine;
using ProjectLink.Utils;

namespace ProjectLink.InGame.Board
{
    public class CellView : MonoBehaviour
    {
        SpriteRenderer _renderer;
        Cell _cell;

        static Sprite _sharedSprite;

        public void Init(Cell cell, float cellSize)
        {
            _cell = cell;
            _renderer = gameObject.AddComponent<SpriteRenderer>();
            _renderer.sprite = GetSharedSprite();
            _renderer.sortingLayerName = "Board";
            transform.localScale = Vector3.one * (cellSize * 0.9f);
            Refresh();
        }

        public void Refresh()
        {
            _renderer.color = _cell.State switch
            {
                CellState.Node  => ColorPalette.Get(_cell.ColorId),
                CellState.Path  => ColorPalette.Get(_cell.ColorId) * new Color(0.65f, 0.65f, 0.65f, 1f),
                _               => new Color(0.15f, 0.15f, 0.15f),
            };
        }

        static Sprite GetSharedSprite()
        {
            if (_sharedSprite != null) return _sharedSprite;
            var tex = new Texture2D(1, 1) { filterMode = FilterMode.Point };
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            _sharedSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.one * 0.5f, 1f);
            return _sharedSprite;
        }
    }
}
