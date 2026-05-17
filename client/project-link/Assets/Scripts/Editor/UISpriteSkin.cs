using UnityEngine;

namespace ProjectLink.EditorTools
{
    public class UISpriteSkin : ScriptableObject
    {
        [System.Serializable]
        public struct Entry
        {
            public string elementName;
            public Sprite sprite;
        }

        public Entry[] sprites = System.Array.Empty<Entry>();

        public Sprite Get(string name)
        {
            foreach (var e in sprites)
                if (e.elementName == name) return e.sprite;
            return null;
        }
    }
}
