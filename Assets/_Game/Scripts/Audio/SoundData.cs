using UnityEngine;
using UnityEngine.Audio;

namespace NeonSyndicate.Audio
{
    /// <summary>
    /// Sound Data - Tek bir ses efekti veya müziğin verisi.
    /// 
    /// Crazy Flasher tarzı özellikler:
    /// - Multi-clip support (varyasyon)
    /// - Randomization (pitch, volume)
    /// - Layering support
    /// - Mixer group integration
    /// </summary>
    [System.Serializable]
    public class SoundData
    {
        [Header("Identification")]
        [Tooltip("Koddan çağırılacak ses ismi (örn: 'Punch_Heavy')")]
        public string name;

        [Header("Audio Clips")]
        [Tooltip("Ses klipleri (birden fazla olabilir, random seçilir)")]
        public AudioClip[] clips;

        [Header("Playback Settings")]
        [Tooltip("Ses seviyesi")]
        [Range(0f, 1f)]
        public float volume = 0.7f;
        
        [Tooltip("Perde (pitch) - 1.0 = normal")]
        [Range(0.1f, 3f)]
        public float pitch = 1f;

        [Header("Randomization")]
        [Tooltip("Her çalışta ses seviyesi varyasyonu")]
        [Range(0f, 0.5f)]
        public float volumeVariance = 0.1f;
        
        [Tooltip("Her çalışta perde varyasyonu (robot sesi önleme)")]
        [Range(0f, 0.5f)]
        public float pitchVariance = 0.1f;

        [Header("Properties")]
        [Tooltip("Ses loop olsun mu? (müzik için)")]
        public bool loop = false;
        
        [Tooltip("3D pozisyonel ses (spatial audio)")]
        public bool is3D = false;
        
        [Tooltip("Bu sesin minimum ve maksimum tekrar süresi (spam önleme)")]
        public float minRepeatDelay = 0f;

        [Header("Mixer")]
        [Tooltip("Audio Mixer Group (SFX/Music/Ambience)")]
        public AudioMixerGroup mixerGroup;

        [Header("Layering (Advanced)")]
        [Tooltip("Bu ses ile birlikte çalacak ek sesler (layering)")]
        public string[] layeredSounds;

        /// <summary>
        /// Random bir clip seçer.
        /// </summary>
        public AudioClip GetRandomClip()
        {
            if (clips == null || clips.Length == 0)
                return null;

            return clips[Random.Range(0, clips.Length)];
        }

        /// <summary>
        /// Randomize edilmiş volume döner.
        /// </summary>
        public float GetRandomizedVolume()
        {
            float variance = Random.Range(-volumeVariance / 2f, volumeVariance / 2f);
            return Mathf.Clamp01(volume * (1f + variance));
        }

        /// <summary>
        /// Randomize edilmiş pitch döner.
        /// </summary>
        public float GetRandomizedPitch()
        {
            float variance = Random.Range(-pitchVariance / 2f, pitchVariance / 2f);
            return pitch * (1f + variance);
        }
    }
}

