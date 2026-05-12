using UnityEngine;

namespace ProjectLink.EditorTools
{
    public class UIButtonSkin : ScriptableObject
    {
        [System.Serializable]
        public struct Entry
        {
            public string elementName;
            public Sprite sprite;
        }

        public Entry[] buttons = System.Array.Empty<Entry>();
        public Entry[] imageSlots = System.Array.Empty<Entry>();

        public Sprite GetButton(string name) => Find(buttons, name);
        public Sprite GetSlot(string name) => Find(imageSlots, name);

        static Sprite Find(Entry[] entries, string name)
        {
            foreach (var e in entries)
                if (e.elementName == name) return e.sprite;
            return null;
        }
    }
}
