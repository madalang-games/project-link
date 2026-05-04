using UnityEngine;
using UnityEngine.UI;

namespace ProjectLink.Core
{
    public enum UILayer { Background, HUD, Popup, System }

    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        private readonly Transform[] _layers = new Transform[4];

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            CreateLayers();
        }

        private void CreateLayers()
        {
            var layerDefs = new (string name, int order)[]
            {
                ("Canvas_Background", 0),
                ("Canvas_HUD",        10),
                ("Canvas_Popup",      20),
                ("Canvas_System",     30),
            };

            for (int i = 0; i < layerDefs.Length; i++)
            {
                var go = new GameObject(layerDefs[i].name);
                go.transform.SetParent(transform, false);

                var canvas = go.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = layerDefs[i].order;

                var scaler = go.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1080, 1920);
                scaler.matchWidthOrHeight = 0.5f;

                go.AddComponent<GraphicRaycaster>();

                _layers[i] = go.transform;
            }
        }

        public Transform GetLayer(UILayer layer) => _layers[(int)layer];

        // TODO: Panel/View 인스턴스 생성·캐싱 추가
    }
}
