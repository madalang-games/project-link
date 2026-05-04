using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectLink.Core
{
    public class DataManager : MonoBehaviour
    {
        public static DataManager Instance { get; private set; }

        // JsonUtility는 HashSet/Dictionary를 직렬화하지 못하므로 List 기반 pair로 변환
        [Serializable]
        class StageStarPair
        {
            public int stageId;
            public int stars;
        }

        [Serializable]
        class SaveData
        {
            public List<int> clearedStages = new();
            public List<StageStarPair> starRatings = new();
            public float soundVolume = 1f;
            public float sfxVolume = 1f;
            public bool hapticEnabled = true;
        }

        private HashSet<int> _clearedStages = new();
        private Dictionary<int, int> _starRatings = new();
        private SaveData _save = new();

        private const string PREFS_KEY = "SaveData";

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }

        // --- Stage Progress ---

        public void ClearStage(int stageId, int stars)
        {
            _clearedStages.Add(stageId);
            if (!_starRatings.TryGetValue(stageId, out int current) || stars > current)
                _starRatings[stageId] = stars;
            Save();
        }

        public bool IsStageCleared(int stageId) => _clearedStages.Contains(stageId);

        public int GetStarRating(int stageId) =>
            _starRatings.TryGetValue(stageId, out int stars) ? stars : 0;

        public bool IsStageUnlocked(int stageId) =>
            stageId == 1 || _clearedStages.Contains(stageId - 1);

        // --- Settings ---

        public float SoundVolume
        {
            get => _save.soundVolume;
            set { _save.soundVolume = value; Save(); }
        }

        public float SfxVolume
        {
            get => _save.sfxVolume;
            set { _save.sfxVolume = value; Save(); }
        }

        public bool HapticEnabled
        {
            get => _save.hapticEnabled;
            set { _save.hapticEnabled = value; Save(); }
        }

        // --- Persistence ---

        public void Save()
        {
            _save.clearedStages = new List<int>(_clearedStages);

            _save.starRatings = new List<StageStarPair>();
            foreach (var kv in _starRatings)
                _save.starRatings.Add(new StageStarPair { stageId = kv.Key, stars = kv.Value });

            PlayerPrefs.SetString(PREFS_KEY, JsonUtility.ToJson(_save));
            PlayerPrefs.Save();
        }

        public void Load()
        {
            string json = PlayerPrefs.GetString(PREFS_KEY, null);
            if (!string.IsNullOrEmpty(json))
                _save = JsonUtility.FromJson<SaveData>(json);

            _clearedStages = new HashSet<int>(_save.clearedStages);

            _starRatings = new Dictionary<int, int>();
            foreach (var pair in _save.starRatings)
                _starRatings[pair.stageId] = pair.stars;

            // TODO: 서버 동기화 - 로컬 데이터와 서버 데이터 병합 처리
        }
    }
}
