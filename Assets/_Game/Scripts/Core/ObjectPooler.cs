using System.Collections.Generic;
using UnityEngine;

namespace NeonSyndicate.Core
{
    /// <summary>
    /// Object Pooling sistemi. Performans için düşman, mermi ve efektleri havuzda tutar.
    /// Sürekli Instantiate/Destroy yerine Activate/Deactivate kullanır.
    /// </summary>
    public class ObjectPooler : MonoBehaviour
    {
        public static ObjectPooler Instance { get; private set; }

        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
        }

        [Header("Pool Configuration")]
        [SerializeField] private List<Pool> pools = new List<Pool>();
        
        private Dictionary<string, Queue<GameObject>> poolDictionary;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializePools();
        }

        private void InitializePools()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab, transform);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }

                poolDictionary.Add(pool.tag, objectPool);
            }

            Debug.Log($"Object Pooler initialized with {pools.Count} pools.");
        }

        /// <summary>
        /// Havuzdan bir obje çeker ve aktifleştirir.
        /// </summary>
        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag '{tag}' doesn't exist!");
                return null;
            }

            GameObject objectToSpawn = poolDictionary[tag].Dequeue();

            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            // IPoolable interface varsa OnSpawn çağır
            IPoolable poolable = objectToSpawn.GetComponent<IPoolable>();
            poolable?.OnSpawn();

            poolDictionary[tag].Enqueue(objectToSpawn);

            return objectToSpawn;
        }

        /// <summary>
        /// Objeyi havuza geri gönderir (deaktif eder).
        /// </summary>
        public void ReturnToPool(string tag, GameObject obj)
        {
            IPoolable poolable = obj.GetComponent<IPoolable>();
            poolable?.OnDespawn();

            obj.SetActive(false);
        }

        /// <summary>
        /// Runtime'da yeni bir pool ekler.
        /// </summary>
        public void AddPool(string tag, GameObject prefab, int size)
        {
            if (poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag '{tag}' already exists!");
                return;
            }

            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < size; i++)
            {
                GameObject obj = Instantiate(prefab, transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(tag, objectPool);
        }
    }

    /// <summary>
    /// Poolable objeler bu interface'i kullanır.
    /// Spawn ve Despawn anlarında custom logic çalıştırabilir.
    /// </summary>
    public interface IPoolable
    {
        void OnSpawn();
        void OnDespawn();
    }
}

