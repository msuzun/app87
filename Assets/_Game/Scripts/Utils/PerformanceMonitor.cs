using UnityEngine;
using System.Collections.Generic;

namespace NeonSyndicate.Utils
{
    /// <summary>
    /// Performance Monitor - FPS, memory ve draw call tracker.
    /// 
    /// Kullanım:
    /// - Development build'de aktif
    /// - Release'de otomatik deaktif
    /// - Screen'de performans metrikleri gösterir
    /// 
    /// Crazy Flasher'da 15 düşman + efektlerde bile 60 FPS tutmak için kritik!
    /// </summary>
    public class PerformanceMonitor : MonoBehaviour
    {
        public static PerformanceMonitor Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private bool showFPS = true;
        [SerializeField] private bool showMemory = true;
        [SerializeField] private bool showDrawCalls = true;
        [SerializeField] private bool showPhysics = true;

        [Header("Warning Thresholds")]
        [SerializeField] private float targetFPS = 60f;
        [SerializeField] private float warningFPS = 45f;
        [SerializeField] private float criticalFPS = 30f;

        [Header("Position")]
        [SerializeField] private Vector2 displayPosition = new Vector2(10, 10);

        // FPS Calculation
        private float deltaTime = 0f;
        private float currentFPS = 60f;
        private Queue<float> fpsHistory = new Queue<float>();
        private const int FPS_HISTORY_SIZE = 60;

        // Memory
        private float totalMemory = 0f;
        private float usedMemory = 0f;

        // Counters
        private int activeEnemyCount = 0;
        private int activeVFXCount = 0;
        private int pooledObjectCount = 0;

        private GUIStyle warningStyle;
        private GUIStyle normalStyle;
        private GUIStyle headerStyle;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            #if !UNITY_EDITOR && !DEVELOPMENT_BUILD
            // Release build'de kapat
            enabled = false;
            #endif
        }

        private void Start()
        {
            InitializeStyles();
        }

        private void Update()
        {
            UpdateFPS();
            UpdateMemory();
            UpdateCounters();
        }

        #region FPS Tracking
        private void UpdateFPS()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            currentFPS = 1.0f / deltaTime;

            // History tracking (average hesaplamak için)
            fpsHistory.Enqueue(currentFPS);
            if (fpsHistory.Count > FPS_HISTORY_SIZE)
            {
                fpsHistory.Dequeue();
            }
        }

        private float GetAverageFPS()
        {
            if (fpsHistory.Count == 0) return 60f;

            float sum = 0f;
            foreach (float fps in fpsHistory)
            {
                sum += fps;
            }
            return sum / fpsHistory.Count;
        }
        #endregion

        #region Memory Tracking
        private void UpdateMemory()
        {
            totalMemory = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / 1048576f; // MB
            usedMemory = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / 1048576f; // MB
        }
        #endregion

        #region Counters
        private void UpdateCounters()
        {
            // Active enemy count
            activeEnemyCount = FindObjectsOfType<Enemy.EnemyController>().Length;

            // VFX count (particle systems)
            activeVFXCount = FindObjectsOfType<ParticleSystem>().Length;

            // Object pool stats
            if (Core.ObjectPooler.Instance != null)
            {
                // pooledObjectCount = ObjectPooler.Instance.GetTotalPooledObjects();
            }
        }
        #endregion

        #region GUI Display
        private void InitializeStyles()
        {
            normalStyle = new GUIStyle();
            normalStyle.fontSize = 14;
            normalStyle.normal.textColor = Color.green;

            warningStyle = new GUIStyle();
            warningStyle.fontSize = 14;
            warningStyle.normal.textColor = Color.yellow;

            headerStyle = new GUIStyle();
            headerStyle.fontSize = 16;
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.normal.textColor = Color.cyan;
        }

        private void OnGUI()
        {
            if (normalStyle == null) InitializeStyles();

            GUILayout.BeginArea(new Rect(displayPosition.x, displayPosition.y, 350, 300));
            
            // Header
            GUILayout.Label("=== PERFORMANCE MONITOR ===", headerStyle);

            // FPS
            if (showFPS)
            {
                GUIStyle fpsStyle = GetFPSStyle(currentFPS);
                GUILayout.Label($"FPS: {currentFPS:F1} (Avg: {GetAverageFPS():F1})", fpsStyle);
                GUILayout.Label($"Frame Time: {deltaTime * 1000f:F1} ms", normalStyle);
            }

            // Memory
            if (showMemory)
            {
                GUILayout.Space(5);
                GUILayout.Label($"Memory: {usedMemory:F1} MB / {totalMemory:F1} MB", normalStyle);
                float memoryPercent = (usedMemory / totalMemory) * 100f;
                GUILayout.Label($"Usage: {memoryPercent:F1}%", GetMemoryStyle(memoryPercent));
            }

            // Rendering
            if (showDrawCalls)
            {
                GUILayout.Space(5);
                #if UNITY_EDITOR
                int triangles = UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(Camera.main) / 1000;
                GUILayout.Label($"Triangles: {triangles}K (approx)", normalStyle);
                #endif
            }

            // Counters
            GUILayout.Space(5);
            GUILayout.Label($"Active Enemies: {activeEnemyCount}", normalStyle);
            GUILayout.Label($"Active VFX: {activeVFXCount}", normalStyle);

            // Physics
            if (showPhysics)
            {
                GUILayout.Space(5);
                int rigidbodies = FindObjectsOfType<Rigidbody2D>().Length;
                int colliders = FindObjectsOfType<Collider2D>().Length;
                GUILayout.Label($"Rigidbodies: {rigidbodies}", normalStyle);
                GUILayout.Label($"Colliders: {colliders}", normalStyle);
            }

            GUILayout.EndArea();
        }

        private GUIStyle GetFPSStyle(float fps)
        {
            if (fps < criticalFPS)
            {
                GUIStyle criticalStyle = new GUIStyle(normalStyle);
                criticalStyle.normal.textColor = Color.red;
                return criticalStyle;
            }
            else if (fps < warningFPS)
            {
                return warningStyle;
            }
            return normalStyle;
        }

        private GUIStyle GetMemoryStyle(float percent)
        {
            if (percent > 80f)
            {
                GUIStyle criticalStyle = new GUIStyle(normalStyle);
                criticalStyle.normal.textColor = Color.red;
                return criticalStyle;
            }
            else if (percent > 60f)
            {
                return warningStyle;
            }
            return normalStyle;
        }
        #endregion

        #region Public API
        /// <summary>
        /// Mevcut FPS'i döner.
        /// </summary>
        public float GetCurrentFPS() => currentFPS;

        /// <summary>
        /// Ortalama FPS'i döner.
        /// </summary>
        public float GetAverageFPS() => GetAverageFPS();

        /// <summary>
        /// Performans uyarısı var mı?
        /// </summary>
        public bool HasPerformanceWarning()
        {
            return currentFPS < warningFPS || (usedMemory / totalMemory) > 0.6f;
        }
        #endregion
    }
}

