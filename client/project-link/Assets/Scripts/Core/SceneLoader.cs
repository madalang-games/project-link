using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectLink.Core
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        public bool IsLoading { get; private set; }

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void LoadScene(string sceneName, Action onComplete = null)
        {
            if (IsLoading) return;
            StartCoroutine(LoadSceneRoutine(sceneName, onComplete));
        }

        public void LoadScene(int buildIndex, Action onComplete = null)
        {
            if (IsLoading) return;
            StartCoroutine(LoadSceneRoutine(buildIndex, onComplete));
        }

        private IEnumerator LoadSceneRoutine(string sceneName, Action onComplete)
        {
            IsLoading = true;
            // TODO: fade-out
            yield return SceneManager.LoadSceneAsync(sceneName);
            // TODO: fade-in
            IsLoading = false;
            onComplete?.Invoke();
        }

        private IEnumerator LoadSceneRoutine(int buildIndex, Action onComplete)
        {
            IsLoading = true;
            // TODO: fade-out
            yield return SceneManager.LoadSceneAsync(buildIndex);
            // TODO: fade-in
            IsLoading = false;
            onComplete?.Invoke();
        }
    }
}
