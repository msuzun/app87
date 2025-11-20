using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
#if DOTWEEN_ENABLED
using DG.Tweening;
#endif

namespace NeonSyndicate.Audio
{
    /// <summary>
    /// Pro Audio Manager - Gelişmiş ses sistemi.
    /// 
    /// Crazy Flasher tarzı özellikler:
    /// - Object pooling (performance)
    /// - Layered audio (çok katmanlı ses)
    /// - Randomization (robot sesi önleme)
    /// - Music crossfade (smooth geçiş)
    /// - Rage mode audio (low-pass filter)
    /// - Priority system
    /// 
    /// Temel SoundManager'dan farkları:
    /// - Multi-clip support (varyasyon)
    /// - Audio source pooling
    /// - Layered sound playback
    /// - Advanced music control
    /// </summary>
    public class ProAudioManager : MonoBehaviour
    {
        public static ProAudioManager Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private List<SoundData> soundEffects = new List<SoundData>();
        [SerializeField] private List<SoundData> musicTracks = new List<SoundData>();

        [Header("Pool Settings")]
        [Tooltip("Aynı anda çalabilecek maksimum SFX sayısı")]
        [SerializeField] private int poolSize = 20;

        [Header("Volume Settings")]
        [Range(0f, 1f)]
        [SerializeField] private float masterVolume = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float musicVolume = 0.7f;
        [Range(0f, 1f)]
        [SerializeField] private float sfxVolume = 1f;

        [Header("Music Crossfade")]
        [SerializeField] private float defaultCrossfadeDuration = 1.5f;

        [Header("Audio Mixer")]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioMixerGroup sfxGroup;
        [SerializeField] private AudioMixerGroup musicGroup;

        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = false;

        // Data structures
        private Dictionary<string, SoundData> soundMap;
        private List<AudioSource> sfxPool;
        private Dictionary<string, float> lastPlayTime; // Spam önleme

        // Music sources (crossfade için 2 adet)
        private AudioSource musicSourceA;
        private AudioSource musicSourceB;
        private bool isMusicSourceAActive = true;

        private void Awake()
        {
            // Singleton
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeSystem();
        }

        #region Initialization
        private void InitializeSystem()
        {
            // Dictionary oluştur (O(1) lookup)
            soundMap = new Dictionary<string, SoundData>();
            lastPlayTime = new Dictionary<string, float>();

            foreach (var sound in soundEffects)
            {
                if (!soundMap.ContainsKey(sound.name))
                {
                    soundMap[sound.name] = sound;
                }
            }

            foreach (var music in musicTracks)
            {
                if (!soundMap.ContainsKey(music.name))
                {
                    soundMap[music.name] = music;
                }
            }

            // SFX Pool oluştur
            CreateSFXPool();

            // Music sources oluştur
            CreateMusicSources();

            Debug.Log($"[ProAudioManager] Initialized: {soundMap.Count} sounds, {poolSize} pool size");
        }

        private void CreateSFXPool()
        {
            GameObject poolParent = new GameObject("SFX_Pool");
            poolParent.transform.SetParent(transform);
            sfxPool = new List<AudioSource>();

            for (int i = 0; i < poolSize; i++)
            {
                GameObject go = new GameObject($"SFX_Source_{i}");
                go.transform.SetParent(poolParent.transform);
                
                AudioSource source = go.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.loop = false;
                source.outputAudioMixerGroup = sfxGroup;
                
                sfxPool.Add(source);
            }
        }

        private void CreateMusicSources()
        {
            musicSourceA = gameObject.AddComponent<AudioSource>();
            musicSourceB = gameObject.AddComponent<AudioSource>();

            musicSourceA.loop = true;
            musicSourceB.loop = true;
            musicSourceA.playOnAwake = false;
            musicSourceB.playOnAwake = false;
            musicSourceA.outputAudioMixerGroup = musicGroup;
            musicSourceB.outputAudioMixerGroup = musicGroup;
        }
        #endregion

        #region SFX Playback
        /// <summary>
        /// Ses efekti çalar.
        /// </summary>
        public void PlaySFX(string soundName)
        {
            if (!soundMap.ContainsKey(soundName))
            {
                if (showDebugInfo)
                    Debug.LogWarning($"[ProAudio] Sound not found: {soundName}");
                return;
            }

            SoundData sound = soundMap[soundName];

            // Spam kontrolü
            if (sound.minRepeatDelay > 0)
            {
                if (lastPlayTime.ContainsKey(soundName))
                {
                    float timeSinceLast = Time.time - lastPlayTime[soundName];
                    if (timeSinceLast < sound.minRepeatDelay)
                    {
                        return; // Çok erken, çalma
                    }
                }
                lastPlayTime[soundName] = Time.time;
            }

            // Havuzdan AudioSource al
            AudioSource source = GetAvailableSource();
            if (source == null)
            {
                if (showDebugInfo)
                    Debug.LogWarning("[ProAudio] Pool full! Sound skipped.");
                return;
            }

            // Setup ve çal
            PlaySoundOnSource(source, sound);

            // Layered sounds çal
            if (sound.layeredSounds != null && sound.layeredSounds.Length > 0)
            {
                foreach (string layeredName in sound.layeredSounds)
                {
                    PlaySFX(layeredName);
                }
            }
        }

        /// <summary>
        /// 3D pozisyonunda ses çalar (spatial audio).
        /// </summary>
        public void PlaySFXAtPosition(string soundName, Vector3 position)
        {
            if (!soundMap.ContainsKey(soundName)) return;

            SoundData sound = soundMap[soundName];
            AudioSource source = GetAvailableSource();
            if (source == null) return;

            source.transform.position = position;
            PlaySoundOnSource(source, sound);
            
            // 3D ayarları
            source.spatialBlend = 1f; // Full 3D
            source.rolloffMode = AudioRolloffMode.Linear;
            source.maxDistance = 15f;
        }

        private void PlaySoundOnSource(AudioSource source, SoundData sound)
        {
            // Clip seç
            AudioClip clip = sound.GetRandomClip();
            if (clip == null) return;

            // Randomize ayarlar
            source.clip = clip;
            source.volume = sound.GetRandomizedVolume() * sfxVolume * masterVolume;
            source.pitch = sound.GetRandomizedPitch();
            source.loop = sound.loop;

            source.Play();

            if (showDebugInfo)
            {
                Debug.Log($"[ProAudio] Playing: {sound.name} (vol: {source.volume:F2}, pitch: {source.pitch:F2})");
            }
        }

        private AudioSource GetAvailableSource()
        {
            // İlk kullanılabilir source'u bul
            foreach (var source in sfxPool)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }

            // Pool doluysa en eski çalan sesi bul ve kullan (priority)
            AudioSource oldestSource = sfxPool[0];
            float oldestTime = oldestSource.time;

            foreach (var source in sfxPool)
            {
                if (source.time > oldestTime)
                {
                    oldestTime = source.time;
                    oldestSource = source;
                }
            }

            return oldestSource;
        }
        #endregion

        #region Music System
        /// <summary>
        /// Müzik çalar (crossfade ile).
        /// </summary>
        public void PlayMusic(string musicName, float fadeDuration = -1f)
        {
            if (!soundMap.ContainsKey(musicName))
            {
                Debug.LogWarning($"[ProAudio] Music not found: {musicName}");
                return;
            }

            float crossfade = fadeDuration > 0 ? fadeDuration : defaultCrossfadeDuration;
            SoundData music = soundMap[musicName];

            AudioSource activeSource = isMusicSourceAActive ? musicSourceA : musicSourceB;
            AudioSource nextSource = isMusicSourceAActive ? musicSourceB : musicSourceA;

            // Aynı müzik zaten çalıyorsa skip
            if (activeSource.clip == music.clips[0] && activeSource.isPlaying)
            {
                return;
            }

            // Yeni müziği hazırla
            nextSource.clip = music.GetRandomClip();
            nextSource.volume = 0;
            nextSource.pitch = music.pitch;
            nextSource.Play();

            // Crossfade
            #if DOTWEEN_ENABLED
            activeSource.DOFade(0, crossfade);
            nextSource.DOFade(music.volume * musicVolume * masterVolume, crossfade);
            #else
            StartCoroutine(CrossfadeRoutine(activeSource, nextSource, music.volume * musicVolume * masterVolume, crossfade));
            #endif

            isMusicSourceAActive = !isMusicSourceAActive;

            Debug.Log($"[ProAudio] Music crossfade: {musicName}");
        }

        /// <summary>
        /// Müziği durdurur (fade out ile).
        /// </summary>
        public void StopMusic(float fadeDuration = -1f)
        {
            float fade = fadeDuration > 0 ? fadeDuration : 1f;
            AudioSource activeSource = isMusicSourceAActive ? musicSourceA : musicSourceB;

            #if DOTWEEN_ENABLED
            activeSource.DOFade(0, fade).OnComplete(() => activeSource.Stop());
            #else
            StartCoroutine(FadeOutRoutine(activeSource, fade));
            #endif
        }

        private IEnumerator CrossfadeRoutine(AudioSource fadeOut, AudioSource fadeIn, float targetVolume, float duration)
        {
            float startVolume = fadeOut.volume;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                fadeOut.volume = Mathf.Lerp(startVolume, 0, t);
                fadeIn.volume = Mathf.Lerp(0, targetVolume, t);

                yield return null;
            }

            fadeOut.Stop();
            fadeOut.volume = startVolume;
            fadeIn.volume = targetVolume;
        }

        private IEnumerator FadeOutRoutine(AudioSource source, float duration)
        {
            float startVolume = source.volume;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, 0, elapsed / duration);
                yield return null;
            }

            source.Stop();
            source.volume = startVolume;
        }
        #endregion

        #region Layered Audio
        /// <summary>
        /// Çok katmanlı ses çalar (punch impact = flesh + bone + air).
        /// </summary>
        public void PlayLayeredSound(params string[] soundNames)
        {
            foreach (string soundName in soundNames)
            {
                PlaySFX(soundName);
            }
        }

        /// <summary>
        /// Punch ses paketi (Heavy attack için).
        /// Örnek: PlayPunchCombo("Whoosh_Heavy", "Impact_Flesh", "Impact_Bone")
        /// </summary>
        public void PlayPunchCombo(string whoosh, string impact, string detail)
        {
            PlaySFX(whoosh);
            Invoke(nameof(PlayDelayedImpact), 0.05f); // Hafif delay (whoosh önce)
            
            void PlayDelayedImpact()
            {
                PlaySFX(impact);
                PlaySFX(detail);
            }
        }
        #endregion

        #region Rage Mode Audio
        /// <summary>
        /// Rage mode audio efektlerini aktif/deaktif eder.
        /// Müzik boğuklaşır, kalp atış sesi eklenir.
        /// </summary>
        public void ToggleRageMode(bool isActive)
        {
            AudioSource activeMusic = isMusicSourceAActive ? musicSourceA : musicSourceB;

            if (isActive)
            {
                // Müziği boğuklaştır (low-pass filter veya pitch düşür)
                #if DOTWEEN_ENABLED
                activeMusic.DOPitch(0.8f, 0.5f);
                #else
                activeMusic.pitch = 0.8f;
                #endif

                // Mixer snapshot kullan (daha profesyonel)
                if (audioMixer != null)
                {
                    // audioMixer.FindSnapshot("RageMode").TransitionTo(0.5f);
                }

                // Kalp atış sesi loop
                PlaySFX("Heartbeat_Loop");

                Debug.Log("[ProAudio] Rage mode ENABLED");
            }
            else
            {
                // Normal'e dön
                #if DOTWEEN_ENABLED
                activeMusic.DOPitch(1f, 0.5f);
                #else
                activeMusic.pitch = 1f;
                #endif

                if (audioMixer != null)
                {
                    // audioMixer.FindSnapshot("Normal").TransitionTo(0.5f);
                }

                Debug.Log("[ProAudio] Rage mode DISABLED");
            }
        }
        #endregion

        #region Volume Control
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            
            if (audioMixer != null)
            {
                audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
            }
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            
            AudioSource activeMusic = isMusicSourceAActive ? musicSourceA : musicSourceB;
            activeMusic.volume = musicVolume * masterVolume;
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
        }
        #endregion

        #region Utility
        /// <summary>
        /// Tüm sesleri durdurur.
        /// </summary>
        public void StopAllSounds()
        {
            foreach (var source in sfxPool)
            {
                source.Stop();
            }
        }

        /// <summary>
        /// Belirli bir ses çalıyor mu?
        /// </summary>
        public bool IsPlaying(string soundName)
        {
            foreach (var source in sfxPool)
            {
                if (source.isPlaying && soundMap.ContainsKey(soundName))
                {
                    if (source.clip == soundMap[soundName].GetRandomClip())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region Debug
        private void OnGUI()
        {
            if (!showDebugInfo) return;

            GUILayout.BeginArea(new Rect(10, 650, 350, 150));
            GUILayout.Label("=== PRO AUDIO MANAGER ===");
            GUILayout.Label($"SFX Pool: {GetActiveSourceCount()} / {poolSize}");
            GUILayout.Label($"Music: {GetCurrentMusicName()}");
            GUILayout.Label($"Master Vol: {masterVolume:F2}");
            GUILayout.Label($"Music Vol: {musicVolume:F2}");
            GUILayout.Label($"SFX Vol: {sfxVolume:F2}");
            GUILayout.EndArea();
        }

        private int GetActiveSourceCount()
        {
            int count = 0;
            foreach (var source in sfxPool)
            {
                if (source.isPlaying) count++;
            }
            return count;
        }

        private string GetCurrentMusicName()
        {
            AudioSource active = isMusicSourceAActive ? musicSourceA : musicSourceB;
            if (active.isPlaying && active.clip != null)
            {
                return active.clip.name;
            }
            return "None";
        }
        #endregion
    }
}

