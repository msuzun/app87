using System.Collections.Generic;
using UnityEngine;

namespace NeonSyndicate.Data
{
    /// <summary>
    /// Level verilerini tutan ScriptableObject.
    /// Hangi dalgada hangi düşmanların çıkacağını belirler.
    /// </summary>
    [CreateAssetMenu(fileName = "New Level Data", menuName = "Neon Syndicate/Level Data")]
    public class LevelData : ScriptableObject
    {
        [Header("Level Information")]
        [SerializeField] private string levelName = "The Slums";
        [SerializeField] private int levelIndex = 1;
        [TextArea(3, 5)]
        [SerializeField] private string levelDescription = "Karanlık sokaklar...";

        [Header("Music")]
        [SerializeField] private AudioClip backgroundMusic;

        [Header("Enemy Waves")]
        [SerializeField] private List<EnemyWave> enemyWaves = new List<EnemyWave>();

        [Header("Boss")]
        [SerializeField] private bool hasBoss = false;
        [SerializeField] private GameObject bossPrefab;

        #region Properties
        public string LevelName => levelName;
        public int LevelIndex => levelIndex;
        public string LevelDescription => levelDescription;
        public AudioClip BackgroundMusic => backgroundMusic;
        public List<EnemyWave> EnemyWaves => enemyWaves;
        public bool HasBoss => hasBoss;
        public GameObject BossPrefab => bossPrefab;
        #endregion
    }

    /// <summary>
    /// Tek bir düşman dalgasını temsil eder.
    /// </summary>
    [System.Serializable]
    public class EnemyWave
    {
        [Header("Wave Info")]
        public int waveNumber = 1;
        public float delayBeforeWave = 2f;

        [Header("Enemies")]
        public List<EnemySpawn> enemies = new List<EnemySpawn>();
    }

    /// <summary>
    /// Tek bir düşman spawn noktasını temsil eder.
    /// </summary>
    [System.Serializable]
    public class EnemySpawn
    {
        public GameObject enemyPrefab;
        public Vector2 spawnPosition;
        public float spawnDelay = 0f;
    }
}

