using UnityEngine;
using System.Collections.Generic;

namespace NeonSyndicate.Level
{
    /// <summary>
    /// Düşman tipleri enum.
    /// </summary>
    public enum EnemyType
    {
        Thug,           // Basic Brawler
        Brawler,        // Basic Brawler variant
        KnifeJack,      // Fast Dodger
        FatBoy,         // Heavy variant
        Tank,           // Heavy Tank
        Gunner,         // Ranged enemy
        Boss            // Boss character
    }

    /// <summary>
    /// Tek bir düşmanın spawn verisi.
    /// </summary>
    [System.Serializable]
    public struct EnemySpawnData
    {
        [Tooltip("Düşman tipi")]
        public EnemyType type;
        
        [Tooltip("Spawn pozisyonu (kamera merkezine göre offset)")]
        public Vector2 spawnOffset;
        
        [Tooltip("Önceki düşmandan kaç saniye sonra spawn olacak?")]
        public float spawnDelay;
        
        [Tooltip("Bu düşman için özel prefab (opsiyonel)")]
        public GameObject customPrefab;
    }

    /// <summary>
    /// Bir dalga (wave) verisi.
    /// "Arena" savaşı - kamera kilitlenir, düşmanlar gelir.
    /// </summary>
    [System.Serializable]
    public class WaveData
    {
        [Header("Wave Info")]
        [Tooltip("Dalga ismi (debug için)")]
        public string waveName = "Wave 1";
        
        [Tooltip("Oyuncu X ekseninde bu noktaya gelince dalga tetiklenir")]
        public float triggerPosX = 20f;
        
        [Header("Enemies")]
        [Tooltip("Bu dalgada spawn olacak düşmanlar")]
        public List<EnemySpawnData> enemies = new List<EnemySpawnData>();
        
        [Header("Camera Lock")]
        [Tooltip("Dalga sırasında kamera kilitlensin mi?")]
        public bool lockCamera = true;
        
        [Tooltip("Kamera kilidi pozisyonu (X)")]
        public float cameraLockX = 0f;
        
        [Header("Completion")]
        [Tooltip("Dalga tamamlandığında spawn olacak ödül (opsiyonel)")]
        public GameObject rewardPrefab;
        
        [Tooltip("Ödül spawn pozisyonu (trigger noktasına göre offset)")]
        public Vector2 rewardSpawnOffset = Vector2.zero;
    }

    /// <summary>
    /// Ana Level Config ScriptableObject.
    /// Her level için bir asset oluşturulur.
    /// 
    /// Kullanım:
    /// Create > Neon Syndicate > Level > Level Config
    /// </summary>
    [CreateAssetMenu(fileName = "New Level Config", menuName = "Neon Syndicate/Level/Level Config")]
    public class LevelConfigSO : ScriptableObject
    {
        [Header("General Settings")]
        [Tooltip("Level ismi (UI'da gösterilir)")]
        public string levelName = "The Slums";
        
        [Tooltip("Level index (sıralama)")]
        public int levelIndex = 1;
        
        [TextArea(3, 5)]
        [Tooltip("Level açıklaması")]
        public string description = "Karanlık sokaklar...";

        [Header("Audio")]
        [Tooltip("Arka plan müziği")]
        public AudioClip backgroundMusic;
        
        [Tooltip("Ambient ses efektleri (yağmur, rüzgar vb.)")]
        public AudioClip ambientSound;

        [Header("Environment")]
        [Tooltip("Environment prefab (arka plan, zemin, props)")]
        public GameObject environmentPrefab;
        
        [Tooltip("Paralllax layer prefab'ı (opsiyonel)")]
        public GameObject parallaxPrefab;

        [Header("Boundaries")]
        [Tooltip("Level başlangıç X pozisyonu")]
        public float levelStartX = 0f;
        
        [Tooltip("Level bitiş X pozisyonu")]
        public float levelEndX = 150f;
        
        [Tooltip("Yürünebilir alan Y minimum (derinlik alt sınır)")]
        public float minY = -3f;
        
        [Tooltip("Yürünebilir alan Y maksimum (derinlik üst sınır)")]
        public float maxY = 1f;

        [Header("Combat Encounters")]
        [Tooltip("Bu level'daki tüm dalgalar")]
        public List<WaveData> waves = new List<WaveData>();

        [Header("Boss Fight")]
        [Tooltip("Bu level'da boss var mı?")]
        public bool hasBoss = false;
        
        [Tooltip("Boss prefab'ı")]
        public GameObject bossPrefab;
        
        [Tooltip("Boss spawn pozisyonu")]
        public Vector2 bossSpawnPosition = Vector2.zero;

        [Header("Victory Conditions")]
        [Tooltip("Level tamamlandığında açılacak sahne")]
        public string nextSceneName = "";
        
        [Tooltip("Level tamamlama bonusu (para)")]
        public int completionBonus = 500;

        #region Validation
        private void OnValidate()
        {
            // Editor'de hatalı değerleri kontrol et
            if (levelEndX <= levelStartX)
            {
                Debug.LogWarning($"[{name}] levelEndX must be greater than levelStartX!");
            }

            if (maxY <= minY)
            {
                Debug.LogWarning($"[{name}] maxY must be greater than minY!");
            }

            // Wave trigger'ları sıralı mı?
            for (int i = 1; i < waves.Count; i++)
            {
                if (waves[i].triggerPosX <= waves[i - 1].triggerPosX)
                {
                    Debug.LogWarning($"[{name}] Wave {i} trigger position should be greater than Wave {i - 1}!");
                }
            }
        }
        #endregion
    }
}

