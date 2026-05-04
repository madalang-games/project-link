using System.Collections.Generic;
using UnityEngine;

namespace ProjectLink.Core
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance { get; private set; }

        private readonly Dictionary<string, Queue<GameObject>> _pools = new();
        private readonly Dictionary<string, GameObject> _prefabs = new();
        // 키별 컨테이너: 씬 전환 시 DontDestroyOnLoad 하위로 귀속되어 파괴 방지
        private readonly Dictionary<string, Transform> _containers = new();

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Register(string key, GameObject prefab, int initialSize = 5)
        {
            if (_pools.ContainsKey(key)) return;

            _prefabs[key] = prefab;
            _pools[key] = new Queue<GameObject>();

            var container = new GameObject($"Pool_{key}");
            container.transform.SetParent(transform);
            _containers[key] = container.transform;

            for (int i = 0; i < initialSize; i++)
                Enqueue(key, Instantiate(prefab, _containers[key]));
        }

        public GameObject Get(string key)
        {
            var pool = _pools[key];
            var obj = pool.Count > 0 ? pool.Dequeue() : Instantiate(_prefabs[key], _containers[key]);
            obj.SetActive(true);
            return obj;
        }

        public void Return(string key, GameObject obj)
        {
            obj.transform.SetParent(_containers[key]);
            Enqueue(key, obj);
        }

        public void ReturnAll(string key)
        {
            foreach (Transform child in _containers[key])
            {
                if (child.gameObject.activeSelf)
                    Enqueue(key, child.gameObject);
            }
        }

        public bool HasPool(string key) => _pools.ContainsKey(key);

        private void Enqueue(string key, GameObject obj)
        {
            obj.SetActive(false);
            _pools[key].Enqueue(obj);
        }
    }
}
