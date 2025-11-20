using UnityEngine;
using System.Collections.Generic;

namespace NeonSyndicate.Utils
{
    /// <summary>
    /// Optimization Helper - Utility fonksiyonlar ve cached değerler.
    /// 
    /// Performans için kritik optimizasyonlar:
    /// - Cached WaitForSeconds (GC-free)
    /// - Tag comparison (string allocation yok)
    /// - Component caching
    /// - Animator hash caching
    /// </summary>
    public static class OptimizationHelper
    {
        #region Cached WaitForSeconds
        // Coroutine'lerde sürekli kullanılan wait değerleri
        private static readonly Dictionary<float, WaitForSeconds> waitDictionary = new Dictionary<float, WaitForSeconds>();
        private static readonly WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        private static readonly WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

        /// <summary>
        /// Cached WaitForSeconds döner (GC-free).
        /// 
        /// Kullanım:
        /// yield return OptimizationHelper.GetWait(0.1f);
        /// </summary>
        public static WaitForSeconds GetWait(float duration)
        {
            if (!waitDictionary.ContainsKey(duration))
            {
                waitDictionary[duration] = new WaitForSeconds(duration);
            }
            return waitDictionary[duration];
        }

        public static WaitForEndOfFrame WaitForEndOfFrame => waitForEndOfFrame;
        public static WaitForFixedUpdate WaitForFixedUpdate => waitForFixedUpdate;
        #endregion

        #region Common Durations
        // Sık kullanılan süreler pre-cached
        public static readonly WaitForSeconds Wait01 = new WaitForSeconds(0.1f);
        public static readonly WaitForSeconds Wait02 = new WaitForSeconds(0.2f);
        public static readonly WaitForSeconds Wait03 = new WaitForSeconds(0.3f);
        public static readonly WaitForSeconds Wait05 = new WaitForSeconds(0.5f);
        public static readonly WaitForSeconds Wait1 = new WaitForSeconds(1f);
        public static readonly WaitForSeconds Wait2 = new WaitForSeconds(2f);
        #endregion

        #region String Allocation Prevention
        // Cached strings (GC önleme)
        public const string TAG_PLAYER = "Player";
        public const string TAG_ENEMY = "Enemy";
        public const string TAG_WALL = "Wall";
        public const string TAG_GROUND = "Ground";
        public const string TAG_PROJECTILE = "Projectile";

        /// <summary>
        /// Tag karşılaştırma (string allocation yok).
        /// 
        /// Kullanım:
        /// if (OptimizationHelper.CompareTag(gameObject, TAG_ENEMY))
        /// </summary>
        public static bool CompareTag(GameObject obj, string tag)
        {
            return obj.CompareTag(tag);
        }
        #endregion

        #region Vector2 Cache
        // Sık kullanılan Vector2 değerleri cached
        public static readonly Vector2 Vector2Zero = Vector2.zero;
        public static readonly Vector2 Vector2One = Vector2.one;
        public static readonly Vector2 Vector2Up = Vector2.up;
        public static readonly Vector2 Vector2Down = Vector2.down;
        public static readonly Vector2 Vector2Left = Vector2.left;
        public static readonly Vector2 Vector2Right = Vector2.right;
        #endregion

        #region Animator Hashes
        // Animator parametreleri için cached hash'ler
        // Animation.AnimData.Hash'te de var ama burası genel utility
        public static class AnimatorHashes
        {
            public static readonly int Speed = Animator.StringToHash("Speed");
            public static readonly int IsWalking = Animator.StringToHash("IsWalking");
            public static readonly int IsRunning = Animator.StringToHash("IsRunning");
            public static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
            public static readonly int Attack = Animator.StringToHash("Attack");
            public static readonly int Hit = Animator.StringToHash("Hit");
            public static readonly int Die = Animator.StringToHash("Die");
        }
        #endregion

        #region Performance Tips
        /// <summary>
        /// Bulk SetActive için optimize edilmiş metod.
        /// </summary>
        public static void SetActiveMultiple(GameObject[] objects, bool active)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] != null)
                {
                    objects[i].SetActive(active);
                }
            }
        }

        /// <summary>
        /// Component caching helper.
        /// Bir kez GetComponent yap, sonra cache'ten kullan.
        /// </summary>
        public class ComponentCache<T> where T : Component
        {
            private Dictionary<GameObject, T> cache = new Dictionary<GameObject, T>();

            public T Get(GameObject obj)
            {
                if (!cache.ContainsKey(obj))
                {
                    cache[obj] = obj.GetComponent<T>();
                }
                return cache[obj];
            }

            public void Clear()
            {
                cache.Clear();
            }
        }
        #endregion

        #region Object Pooling Helpers
        /// <summary>
        /// Transform array pool (allocation yok).
        /// </summary>
        private static Transform[] transformBuffer = new Transform[100];

        public static Transform[] GetChildrenNonAlloc(Transform parent)
        {
            int count = parent.childCount;
            if (count > transformBuffer.Length)
            {
                transformBuffer = new Transform[count];
            }

            for (int i = 0; i < count; i++)
            {
                transformBuffer[i] = parent.GetChild(i);
            }

            return transformBuffer;
        }
        #endregion
    }

    /// <summary>
    /// FPS Counter - Basit FPS göstergesi.
    /// PerformanceMonitor'dan daha hafif.
    /// </summary>
    public class SimpleFPSCounter : MonoBehaviour
    {
        private float deltaTime = 0f;

        private void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        private void OnGUI()
        {
            int fps = Mathf.CeilToInt(1.0f / deltaTime);
            
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.normal.textColor = fps < 30 ? Color.red : (fps < 45 ? Color.yellow : Color.green);

            GUILayout.Label($"FPS: {fps}", style);
        }
    }
}

