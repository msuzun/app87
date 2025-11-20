using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NeonSyndicate.Core;

namespace NeonSyndicate.Level
{
    /// <summary>
    /// Wave Spawner - Düşman spawn sistemini yönetir.
    /// LevelManager tarafından kontrol edilir.
    /// 
    /// Özellikler:
    /// - Delay ile spawn (spawnDelay)
    /// - Enemy type'a göre prefab seçimi
    /// - Object pooling desteği
    /// - Spawn effect'leri
    /// </summary>
    public class WaveSpawner : MonoBehaviour
    {
        [Header("Enemy Prefabs")]
        [Tooltip("Düşman tipine göre prefab mapping")]
        [SerializeField] private List<EnemyPrefabMapping> enemyPrefabs = new List<EnemyPrefabMapping>();

        [Header("Spawn Settings")]
        [SerializeField] private GameObject spawnEffectPrefab;
        [SerializeField] private float spawnEffectDuration = 0.5f;
        [SerializeField] private bool useObjectPooling = true;

        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = false;

        /// <summary>
        /// Düşman tipi → Prefab mapping
        /// </summary>
        [System.Serializable]
        public class EnemyPrefabMapping
        {
            public EnemyType type;
            public GameObject prefab;
        }

        /// <summary>
        /// Bir wave'i spawn eder.
        /// </summary>
        /// <param name="wave">Spawn edilecek wave</param>
        /// <param name="onEnemySpawned">Her düşman spawn olduğunda callback</param>
        public IEnumerator SpawnWave(WaveData wave, System.Action<GameObject> onEnemySpawned)
        {
            if (wave == null || wave.enemies == null || wave.enemies.Count == 0)
            {
                Debug.LogWarning("[WaveSpawner] Wave has no enemies!");
                yield break;
            }

            Debug.Log($"[WaveSpawner] Spawning wave: {wave.waveName} ({wave.enemies.Count} enemies)");

            // Her düşmanı spawn et (delay ile)
            foreach (var enemyData in wave.enemies)
            {
                // Delay
                if (enemyData.spawnDelay > 0)
                {
                    yield return new WaitForSeconds(enemyData.spawnDelay);
                }

                // Spawn
                GameObject enemy = SpawnEnemy(enemyData, wave.triggerPosX);
                
                // Callback çağır
                onEnemySpawned?.Invoke(enemy);

                if (showDebugInfo)
                {
                    Debug.Log($"[WaveSpawner] Spawned: {enemyData.type} at {enemy?.transform.position}");
                }
            }
        }

        /// <summary>
        /// Tek bir düşman spawn eder.
        /// </summary>
        private GameObject SpawnEnemy(EnemySpawnData data, float triggerX)
        {
            // Spawn pozisyonu hesapla
            Vector3 spawnPosition = new Vector3(
                triggerX + data.spawnOffset.x,
                data.spawnOffset.y,
                0
            );

            GameObject enemyPrefab = GetEnemyPrefab(data);
            if (enemyPrefab == null)
            {
                Debug.LogWarning($"[WaveSpawner] No prefab found for enemy type: {data.type}");
                return null;
            }

            GameObject enemy = null;

            // Object Pooling kullan
            if (useObjectPooling && ObjectPooler.Instance != null)
            {
                enemy = ObjectPooler.Instance.SpawnFromPool(
                    data.type.ToString(),
                    spawnPosition,
                    Quaternion.identity
                );
            }
            else
            {
                // Direct instantiate
                enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            }

            // Spawn effect
            if (spawnEffectPrefab != null)
            {
                GameObject effect = Instantiate(spawnEffectPrefab, spawnPosition, Quaternion.identity);
                Destroy(effect, spawnEffectDuration);
            }

            // Ses efekti
            SoundManager.Instance?.PlaySFX("Enemy_Spawn");

            return enemy;
        }

        /// <summary>
        /// Enemy type'a göre prefab döner.
        /// </summary>
        private GameObject GetEnemyPrefab(EnemySpawnData data)
        {
            // Custom prefab varsa onu kullan
            if (data.customPrefab != null)
            {
                return data.customPrefab;
            }

            // Mapping'den bul
            foreach (var mapping in enemyPrefabs)
            {
                if (mapping.type == data.type)
                {
                    return mapping.prefab;
                }
            }

            return null;
        }

        #region Debug Methods
        /// <summary>
        /// Test için manuel spawn.
        /// </summary>
        [ContextMenu("Test Spawn Enemy")]
        private void TestSpawnEnemy()
        {
            if (enemyPrefabs.Count == 0) return;

            EnemySpawnData testData = new EnemySpawnData
            {
                type = enemyPrefabs[0].type,
                spawnOffset = new Vector2(5, 0),
                spawnDelay = 0
            };

            SpawnEnemy(testData, 0);
        }
        #endregion
    }
}

