using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace ProjectLink.Core
{
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance { get; private set; }

        // TODO: 서버 URL 설정
        public string BaseUrl { get; set; }

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Get(string endpoint, Action<bool, string> onComplete)
        {
            StartCoroutine(SendGet(endpoint, onComplete));
        }

        public void Post(string endpoint, string jsonBody, Action<bool, string> onComplete)
        {
            StartCoroutine(SendPost(endpoint, jsonBody, onComplete));
        }

        private IEnumerator SendGet(string endpoint, Action<bool, string> onComplete)
        {
            // TODO: 인증 토큰 헤더 추가
            using var req = UnityWebRequest.Get(BaseUrl + endpoint);
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
                onComplete?.Invoke(true, req.downloadHandler.text);
            else
                onComplete?.Invoke(false, req.error);

            // TODO: 실패 시 재시도 로직
        }

        private IEnumerator SendPost(string endpoint, string jsonBody, Action<bool, string> onComplete)
        {
            // TODO: 인증 토큰 헤더 추가
            using var req = new UnityWebRequest(BaseUrl + endpoint, "POST");
            var bodyBytes = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            req.uploadHandler = new UploadHandlerRaw(bodyBytes);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
                onComplete?.Invoke(true, req.downloadHandler.text);
            else
                onComplete?.Invoke(false, req.error);

            // TODO: 실패 시 재시도 로직
        }

        // TODO: 요청 큐 (동시 요청 수 제한이 필요할 경우)
    }
}
