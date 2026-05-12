using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProjectLink.OutGame.UI
{
    public sealed class RepeatButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] float initialDelay = 0.45f;
        [SerializeField] float repeatInterval = 0.12f;

        readonly UnityEvent _repeated = new();
        Coroutine _routine;
        Button _button;

        public UnityEvent Repeated => _repeated;

        void Awake()
        {
            _button = GetComponent<Button>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            StopRepeat();
            _routine = StartCoroutine(RepeatRoutine());
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            StopRepeat();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StopRepeat();
        }

        IEnumerator RepeatRoutine()
        {
            yield return new WaitForSecondsRealtime(initialDelay);
            while (_button == null || _button.interactable)
            {
                _repeated.Invoke();
                yield return new WaitForSecondsRealtime(repeatInterval);
            }
        }

        void StopRepeat()
        {
            if (_routine == null) return;
            StopCoroutine(_routine);
            _routine = null;
        }
    }
}
