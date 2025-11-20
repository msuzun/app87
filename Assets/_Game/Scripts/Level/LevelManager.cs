using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NeonSyndicate.Core;

namespace NeonSyndicate.Level
{
    /// <summary>
    /// Level Manager - Bölüm akışını yönetir.
    /// 
    /// İş akışı:
    /// 1. Level config yükle
    /// 2. Environment spawn et
    /// 3. Müzik başlat
    /// 4. Oyuncu ilerledikçe wave trigger'larını kontrol et
    /// 5. Wave başlat → Kamera kilitle → Düşman spawn
    /// 6. Tüm düşmanlar öldü → Kamera kilidi aç → Devam et
    /// 7. Boss fight (varsa)
    /// 8. Level complete
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private LevelConfigSO currentLevelConfig;

        [Header("References")]
        [SerializeField] private Transform player;
        [SerializeField] private WaveSpawner waveSpawner;
        [SerializeField] private CameraLockController cameraController;

        [Header("Current State")]
        [SerializeField] private int currentWaveIndex = 0;
        [SerializeField] private bool isWaveActive = false;
        [SerializeField] private bool levelComplete = false;

        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;

        // Events
        public delegate void LevelEvent(int waveIndex);
        public static event LevelEvent OnWaveStart;
        public static event LevelEvent OnWaveComplete;
        public static event LevelEvent OnLevelComplete;

        private List<GameObject> activeEnemies = new List<GameObject>();
        private GameObject environmentInstance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            InitializeLevel();
        }

        private void Update()
        {
            if (levelComplete || currentLevelConfig == null) return;

            // Oyuncu pozisyon kontrolü (wave trigger)
            CheckWaveTriggers();

            // Wave aktifse düşman sayısını kontrol et
            if (isWaveActive)
            {
                CheckWaveCompletion();
            }
        }

        #region Level Initialization
        private void InitializeLevel()
        {
            if (currentLevelConfig == null)
            {
                Debug.LogError("[LevelManager] No level config assigned!");
                return;
            }

            // Environment spawn
            SpawnEnvironment();

            // Müzik başlat
            StartMusic();

            // Kamera sınırlarını ayarla
            SetupCameraBoundaries();

            // Oyuncuyu başlangıç pozisyonuna yerleştir
            if (player != null)
            {
                player.position = new Vector3(currentLevelConfig.levelStartX, 0, 0);
            }

            Debug.Log($"[LevelManager] Level initialized: {currentLevelConfig.levelName}");
        }

        private void SpawnEnvironment()
        {
            if (currentLevelConfig.environmentPrefab != null)
            {
                environmentInstance = Instantiate(
                    currentLevelConfig.environmentPrefab,
                    Vector3.zero,
                    Quaternion.identity
                );
                environmentInstance.name = "Environment";
            }

            // Parallax (opsiyonel)
            if (currentLevelConfig.parallaxPrefab != null)
            {
                GameObject parallax = Instantiate(
                    currentLevelConfig.parallaxPrefab,
                    Vector3.zero,
                    Quaternion.identity
                );
                parallax.name = "Parallax";
            }
        }

        private void StartMusic()
        {
            if (currentLevelConfig.backgroundMusic != null)
            {
                SoundManager.Instance?.PlayMusic(currentLevelConfig.backgroundMusic.name);
            }

            // Ambient sound (opsiyonel)
            if (currentLevelConfig.ambientSound != null)
            {
                // Ambient loop sistemi (SoundManager'a eklenebilir)
            }
        }

        private void SetupCameraBoundaries()
        {
            if (cameraController != null)
            {
                cameraController.SetBoundaries(
                    new Vector2(currentLevelConfig.levelStartX, currentLevelConfig.minY),
                    new Vector2(currentLevelConfig.levelEndX, currentLevelConfig.maxY)
                );
            }
        }
        #endregion

        #region Wave System
        private void CheckWaveTriggers()
        {
            if (player == null || isWaveActive) return;

            // Sonraki wave var mı?
            if (currentWaveIndex >= currentLevelConfig.waves.Count)
            {
                // Tüm wave'ler bitti, boss var mı?
                if (currentLevelConfig.hasBoss)
                {
                    StartBossFight();
                }
                else
                {
                    CompleteLevel();
                }
                return;
            }

            // Wave trigger kontrolü
            WaveData nextWave = currentLevelConfig.waves[currentWaveIndex];
            if (player.position.x >= nextWave.triggerPosX)
            {
                StartWave(currentWaveIndex);
            }
        }

        private void StartWave(int waveIndex)
        {
            if (waveIndex >= currentLevelConfig.waves.Count) return;

            WaveData wave = currentLevelConfig.waves[waveIndex];
            isWaveActive = true;

            Debug.Log($"[LevelManager] Wave {waveIndex + 1} started: {wave.waveName}");

            // Kamera kilitle
            if (wave.lockCamera && cameraController != null)
            {
                cameraController.LockCamera(wave.cameraLockX);
            }

            // Düşmanları spawn et
            if (waveSpawner != null)
            {
                StartCoroutine(waveSpawner.SpawnWave(wave, OnEnemySpawned));
            }

            // UI göster ("Wave 1" text vb.)
            // UIManager?.ShowWaveStart(waveIndex + 1);

            // Event fırlat
            OnWaveStart?.Invoke(waveIndex);
        }

        private void OnEnemySpawned(GameObject enemy)
        {
            if (enemy != null)
            {
                activeEnemies.Add(enemy);
                
                // Düşmanın ölüm event'ine abone ol
                // enemy.GetComponent<EnemyController>().OnDeath += () => OnEnemyDeath(enemy);
            }
        }

        private void CheckWaveCompletion()
        {
            // Ölü düşmanları temizle
            activeEnemies.RemoveAll(e => e == null || !e.activeInHierarchy);

            // Tüm düşmanlar öldü mü?
            if (activeEnemies.Count == 0)
            {
                CompleteWave();
            }
        }

        private void CompleteWave()
        {
            WaveData wave = currentLevelConfig.waves[currentWaveIndex];
            
            Debug.Log($"[LevelManager] Wave {currentWaveIndex + 1} completed!");

            // Kamera kilidini aç
            if (cameraController != null)
            {
                cameraController.UnlockCamera();
            }

            // Ödül spawn et
            SpawnWaveReward(wave);

            // UI göster ("GO!" text)
            // UIManager?.ShowWaveClear();

            // Event fırlat
            OnWaveComplete?.Invoke(currentWaveIndex);

            // Sonraki wave'e geç
            currentWaveIndex++;
            isWaveActive = false;
        }

        private void SpawnWaveReward(WaveData wave)
        {
            if (wave.rewardPrefab != null)
            {
                Vector3 spawnPos = new Vector3(
                    wave.triggerPosX + wave.rewardSpawnOffset.x,
                    wave.rewardSpawnOffset.y,
                    0
                );

                Instantiate(wave.rewardPrefab, spawnPos, Quaternion.identity);
            }
        }
        #endregion

        #region Boss Fight
        private void StartBossFight()
        {
            if (!currentLevelConfig.hasBoss)
            {
                CompleteLevel();
                return;
            }

            Debug.Log("[LevelManager] Boss fight starting!");

            // Kamera kilitle
            if (cameraController != null)
            {
                cameraController.LockCamera(currentLevelConfig.bossSpawnPosition.x);
            }

            // Boss spawn
            if (currentLevelConfig.bossPrefab != null)
            {
                GameObject boss = Instantiate(
                    currentLevelConfig.bossPrefab,
                    currentLevelConfig.bossSpawnPosition,
                    Quaternion.identity
                );

                activeEnemies.Add(boss);
                
                // Boss müziği
                // SoundManager.Instance?.PlayMusic("Boss_Theme");
            }

            isWaveActive = true;
        }
        #endregion

        #region Level Completion
        private void CompleteLevel()
        {
            if (levelComplete) return;

            levelComplete = true;

            Debug.Log($"[LevelManager] Level {currentLevelConfig.levelIndex} completed!");

            // Completion bonus
            GameManager.Instance?.AddMoney(currentLevelConfig.completionBonus);

            // Event fırlat
            OnLevelComplete?.Invoke(currentLevelConfig.levelIndex);

            // Victory müziği
            SoundManager.Instance?.PlayMusic("Victory_Theme");

            // UI göster
            // UIManager?.ShowLevelComplete();

            // 3 saniye sonra sonraki level'a geç
            Invoke(nameof(LoadNextLevel), 3f);
        }

        private void LoadNextLevel()
        {
            if (!string.IsNullOrEmpty(currentLevelConfig.nextSceneName))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(currentLevelConfig.nextSceneName);
            }
            else
            {
                // Sonraki sahne yok, ana menüye dön
                GameManager.Instance?.LoadNextLevel();
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Manuel olarak wave başlatır (debug için).
        /// </summary>
        public void ForceStartWave(int waveIndex)
        {
            if (waveIndex >= 0 && waveIndex < currentLevelConfig.waves.Count)
            {
                currentWaveIndex = waveIndex;
                StartWave(waveIndex);
            }
        }

        /// <summary>
        /// Mevcut wave'i zorla tamamlar (debug için).
        /// </summary>
        public void ForceCompleteWave()
        {
            // Tüm aktif düşmanları yok et
            foreach (var enemy in activeEnemies)
            {
                if (enemy != null)
                {
                    Destroy(enemy);
                }
            }

            activeEnemies.Clear();
            CompleteWave();
        }

        /// <summary>
        /// Level'ı yeniden başlatır.
        /// </summary>
        public void RestartLevel()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }
        #endregion

        #region Properties
        public LevelConfigSO CurrentLevelConfig => currentLevelConfig;
        public int CurrentWaveIndex => currentWaveIndex;
        public bool IsWaveActive => isWaveActive;
        public int ActiveEnemyCount => activeEnemies.Count;
        #endregion

        #region Debug
        private void OnGUI()
        {
            if (!showDebugInfo) return;

            GUILayout.BeginArea(new Rect(10, 450, 350, 200));
            GUILayout.Label("=== LEVEL MANAGER DEBUG ===");
            GUILayout.Label($"Level: {currentLevelConfig?.levelName ?? "None"}");
            GUILayout.Label($"Wave: {currentWaveIndex + 1} / {currentLevelConfig?.waves.Count ?? 0}");
            GUILayout.Label($"Wave Active: {isWaveActive}");
            GUILayout.Label($"Active Enemies: {activeEnemies.Count}");
            GUILayout.Label($"Player X: {player?.position.x:F1}");
            
            if (currentWaveIndex < currentLevelConfig?.waves.Count)
            {
                WaveData nextWave = currentLevelConfig.waves[currentWaveIndex];
                GUILayout.Label($"Next Trigger: {nextWave.triggerPosX:F1}");
            }
            
            GUILayout.EndArea();
        }
        #endregion
    }
}

