using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeonSyndicate.Core
{
    /// <summary>
    /// Oyunun tüm ses efektlerini ve müziklerini yöneten manager.
    /// Unity Audio Mixer ile entegre çalışır.
    /// </summary>
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        
        [Header("Sound Effects")]
        [SerializeField] private List<Sound> soundEffects = new List<Sound>();
        
        [Header("Music Tracks")]
        [SerializeField] private List<Sound> musicTracks = new List<Sound>();

        [Header("Volume Settings")]
        [Range(0f, 1f)]
        [SerializeField] private float masterVolume = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float musicVolume = 0.7f;
        [Range(0f, 1f)]
        [SerializeField] private float sfxVolume = 1f;

        private Dictionary<string, AudioClip> sfxDictionary;
        private Dictionary<string, AudioClip> musicDictionary;

        [System.Serializable]
        public class Sound
        {
            public string name;
            public AudioClip clip;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeAudio();
        }

        private void InitializeAudio()
        {
            // Dictionary'leri doldur
            sfxDictionary = new Dictionary<string, AudioClip>();
            foreach (Sound sound in soundEffects)
            {
                if (!sfxDictionary.ContainsKey(sound.name))
                {
                    sfxDictionary.Add(sound.name, sound.clip);
                }
            }

            musicDictionary = new Dictionary<string, AudioClip>();
            foreach (Sound music in musicTracks)
            {
                if (!musicDictionary.ContainsKey(music.name))
                {
                    musicDictionary.Add(music.name, music.clip);
                }
            }

            // Audio Source'ları ayarla
            if (musicSource != null)
            {
                musicSource.loop = true;
                musicSource.volume = musicVolume * masterVolume;
            }

            if (sfxSource != null)
            {
                sfxSource.volume = sfxVolume * masterVolume;
            }
        }

        #region SFX Methods
        /// <summary>
        /// Ses efekti çalar.
        /// </summary>
        public void PlaySFX(string soundName, float volumeMultiplier = 1f)
        {
            if (sfxDictionary.ContainsKey(soundName))
            {
                sfxSource.PlayOneShot(sfxDictionary[soundName], volumeMultiplier);
            }
            else
            {
                Debug.LogWarning($"Sound '{soundName}' not found!");
            }
        }

        /// <summary>
        /// 3D pozisyonda ses efekti çalar (spatial audio).
        /// </summary>
        public void PlaySFXAtPosition(string soundName, Vector3 position, float volumeMultiplier = 1f)
        {
            if (sfxDictionary.ContainsKey(soundName))
            {
                AudioSource.PlayClipAtPoint(sfxDictionary[soundName], position, sfxVolume * masterVolume * volumeMultiplier);
            }
        }
        #endregion

        #region Music Methods
        /// <summary>
        /// Müzik çalmaya başlar (crossfade ile).
        /// </summary>
        public void PlayMusic(string musicName, float fadeTime = 1f)
        {
            if (musicDictionary.ContainsKey(musicName))
            {
                StartCoroutine(CrossfadeMusic(musicDictionary[musicName], fadeTime));
            }
            else
            {
                Debug.LogWarning($"Music '{musicName}' not found!");
            }
        }

        public void StopMusic(float fadeTime = 1f)
        {
            StartCoroutine(FadeOutMusic(fadeTime));
        }

        private IEnumerator CrossfadeMusic(AudioClip newClip, float fadeTime)
        {
            // Eski müziği fade out
            float startVolume = musicSource.volume;
            
            while (musicSource.volume > 0)
            {
                musicSource.volume -= startVolume * Time.deltaTime / fadeTime;
                yield return null;
            }

            // Yeni müziği başlat
            musicSource.clip = newClip;
            musicSource.Play();

            // Fade in
            while (musicSource.volume < musicVolume * masterVolume)
            {
                musicSource.volume += startVolume * Time.deltaTime / fadeTime;
                yield return null;
            }

            musicSource.volume = musicVolume * masterVolume;
        }

        private IEnumerator FadeOutMusic(float fadeTime)
        {
            float startVolume = musicSource.volume;

            while (musicSource.volume > 0)
            {
                musicSource.volume -= startVolume * Time.deltaTime / fadeTime;
                yield return null;
            }

            musicSource.Stop();
            musicSource.volume = startVolume;
        }
        #endregion

        #region Volume Control
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            musicSource.volume = musicVolume * masterVolume;
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            musicSource.volume = musicVolume * masterVolume;
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
        }
        #endregion
    }
}

