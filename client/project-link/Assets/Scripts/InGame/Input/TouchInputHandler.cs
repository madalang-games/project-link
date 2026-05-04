using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectLink.InGame.Input
{
    public class TouchInputHandler : MonoBehaviour
    {
        public event Action<Vector2> OnDragStart;
        public event Action<Vector2> OnDragMove;
        public event Action<Vector2> OnDragEnd;
        public event Action<Vector2> OnLongPressStart;
        public event Action          OnLongPressCanceled;

        [SerializeField] float _longPressThreshold = 0.7f;
        [SerializeField] float _longPressMoveLimit  = 0.15f;

        bool    _isPressing;
        bool    _longPressFired;
        float   _pressTime;
        Vector2 _pressStartWorld;

        void Update()
        {
            var pointer = Pointer.current;
            if (pointer == null) return;

            if (pointer.press.wasPressedThisFrame)
            {
                _isPressing      = true;
                _longPressFired  = false;
                _pressTime       = 0f;
                _pressStartWorld = ToWorld(pointer.position.ReadValue());
                OnDragStart?.Invoke(_pressStartWorld);
            }

            if (_isPressing && pointer.press.isPressed)
            {
                var worldPos = ToWorld(pointer.position.ReadValue());
                OnDragMove?.Invoke(worldPos);

                _pressTime += Time.deltaTime;
                float moved = Vector2.Distance(worldPos, _pressStartWorld);

                if (!_longPressFired
                    && _pressTime  >= _longPressThreshold
                    && moved       <= _longPressMoveLimit)
                {
                    _longPressFired = true;
                    OnLongPressStart?.Invoke(_pressStartWorld);
                }
            }

            if (pointer.press.wasReleasedThisFrame)
            {
                _isPressing = false;
                var worldPos = ToWorld(pointer.position.ReadValue());

                if (_longPressFired)
                    OnLongPressCanceled?.Invoke();
                else
                    OnDragEnd?.Invoke(worldPos);
            }
        }

        static Vector2 ToWorld(Vector2 screenPos) =>
            Camera.main.ScreenToWorldPoint(screenPos);
    }
}
