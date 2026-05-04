using UnityEngine;

namespace ProjectLink.Utils
{
    public static class ColorPalette
    {
        static readonly Color[] _colors =
        {
            new Color(0.94f, 0.23f, 0.23f), // 1 red
            new Color(0.18f, 0.80f, 0.44f), // 2 green
            new Color(0.20f, 0.60f, 0.90f), // 3 blue
            new Color(0.98f, 0.85f, 0.17f), // 4 yellow
            new Color(0.60f, 0.10f, 0.60f), // 5 maroon
            new Color(0.10f, 0.85f, 0.85f), // 6 cyan
        };

        public static Color Get(int colorId)
        {
            if (colorId <= 0 || colorId > _colors.Length) return Color.white;
            return _colors[colorId - 1];
        }
    }
}
