using UnityEngine;

namespace ProjectLink.Core
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        AudioSource _bgmSource;
        AudioSource _sfxSource;

        public float BgmVolume
        {
            get => _bgmSource.volume;
            set => _bgmSource.volume = value;
        }

        public float SfxVolume
        {
            get => _sfxSource.volume;
            set => _sfxSource.volume = value;
        }

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _bgmSource = gameObject.AddComponent<AudioSource>();
            _bgmSource.playOnAwake = false;

            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.playOnAwake = false;
        }

        void Start()
        {
            if (DataManager.Instance != null)
                ApplySettings(DataManager.Instance.SoundVolume, DataManager.Instance.SfxVolume);
        }

        public void PlayBGM(AudioClip clip, bool loop = true)
        {
            _bgmSource.clip = clip;
            _bgmSource.loop = loop;
            _bgmSource.Play();
        }

        public void StopBGM() => _bgmSource.Stop();

        public void PlaySFX(AudioClip clip) => _sfxSource.PlayOneShot(clip, _sfxSource.volume);

        public void ApplySettings(float bgm, float sfx)
        {
            BgmVolume = bgm;
            SfxVolume = sfx;
        }
    }
}
