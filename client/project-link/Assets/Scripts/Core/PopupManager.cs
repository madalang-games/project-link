using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectLink.Core
{
    public abstract class PopupBase : MonoBehaviour
    {
        public virtual void OnBackPressed() => PopupManager.Instance.CloseTop();
    }

    public class PopupManager : MonoBehaviour
    {
        public static PopupManager Instance { get; private set; }

        private readonly Stack<PopupBase> _stack = new();

        public bool HasPopup => _stack.Count > 0;
        public int Count => _stack.Count;

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame && HasPopup)
                CloseTop();
        }

        public void Push(PopupBase popup)
        {
            if (_stack.TryPeek(out var prev))
                prev.gameObject.SetActive(false);

            _stack.Push(popup);
        }

        public void CloseTop()
        {
            if (!HasPopup) return;

            Destroy(_stack.Pop().gameObject);

            if (_stack.TryPeek(out var top))
                top.gameObject.SetActive(true);
        }

        public void CloseAll()
        {
            while (HasPopup)
                Destroy(_stack.Pop().gameObject);
        }

        // TODO: prefab 로드·인스턴스화 후 Push 호출하는 Open<T> 추가
    }
}
