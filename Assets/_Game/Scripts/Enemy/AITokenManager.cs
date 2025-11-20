using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NeonSyndicate.Enemy
{
    /// <summary>
    /// Token Sistemi: Aynı anda en fazla 2 düşmanın saldırmasını sağlar.
    /// Crazy Flasher'ın "adil" hissini yaratır.
    /// </summary>
    public class AITokenManager : MonoBehaviour
    {
        public static AITokenManager Instance { get; private set; }

        [Header("Token Settings")]
        [SerializeField] private int maxActiveAttackers = 2;
        [SerializeField] private float tokenCheckInterval = 0.5f;

        [Header("Debug")]
        [SerializeField] private List<EnemyAI> allEnemies = new List<EnemyAI>();
        [SerializeField] private List<EnemyAI> enemiesWithTokens = new List<EnemyAI>();

        private float tokenCheckTimer;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            tokenCheckTimer += Time.deltaTime;

            if (tokenCheckTimer >= tokenCheckInterval)
            {
                tokenCheckTimer = 0f;
                ManageTokens();
            }
        }

        /// <summary>
        /// Token dağıtımını yönetir.
        /// </summary>
        private void ManageTokens()
        {
            // Ölü düşmanları temizle
            allEnemies.RemoveAll(e => e == null || e.GetComponent<EnemyController>().IsDead);
            enemiesWithTokens.RemoveAll(e => e == null || e.GetComponent<EnemyController>().IsDead);

            // Token sahibi olan ama artık Chase/Attack state'inde olmayan düşmanlardan tokeni geri al
            for (int i = enemiesWithTokens.Count - 1; i >= 0; i--)
            {
                EnemyAI enemy = enemiesWithTokens[i];
                if (enemy.CurrentState != EnemyAI.AIState.Chase && 
                    enemy.CurrentState != EnemyAI.AIState.Attack)
                {
                    enemy.RevokeAttackToken();
                    enemiesWithTokens.RemoveAt(i);
                }
            }

            // Token sınırına ulaşılmadıysa yeni token dağıt
            if (enemiesWithTokens.Count < maxActiveAttackers)
            {
                DistributeTokens();
            }
        }

        /// <summary>
        /// Yeni tokenleri en yakın düşmanlara dağıtır.
        /// </summary>
        private void DistributeTokens()
        {
            // Oyuncuyu bul
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            // Token almak için uygun düşmanları bul
            List<EnemyAI> eligibleEnemies = allEnemies
                .Where(e => !e.HasAttackToken && 
                           (e.CurrentState == EnemyAI.AIState.Chase || 
                            e.CurrentState == EnemyAI.AIState.Idle))
                .ToList();

            // Oyuncuya yakınlığa göre sırala
            eligibleEnemies = eligibleEnemies
                .OrderBy(e => Vector2.Distance(e.transform.position, player.transform.position))
                .ToList();

            // Token dağıt
            int tokensToDistribute = maxActiveAttackers - enemiesWithTokens.Count;
            for (int i = 0; i < Mathf.Min(tokensToDistribute, eligibleEnemies.Count); i++)
            {
                EnemyAI enemy = eligibleEnemies[i];
                enemy.GrantAttackToken();
                enemiesWithTokens.Add(enemy);
            }
        }

        #region Enemy Registration
        /// <summary>
        /// Düşmanı sisteme kaydeder.
        /// </summary>
        public void RegisterEnemy(EnemyAI enemy)
        {
            if (!allEnemies.Contains(enemy))
            {
                allEnemies.Add(enemy);
                Debug.Log($"Enemy registered: {enemy.name}. Total: {allEnemies.Count}");
            }
        }

        /// <summary>
        /// Düşmanı sistemden çıkarır.
        /// </summary>
        public void UnregisterEnemy(EnemyAI enemy)
        {
            if (allEnemies.Contains(enemy))
            {
                allEnemies.Remove(enemy);
                Debug.Log($"Enemy unregistered: {enemy.name}. Total: {allEnemies.Count}");
            }

            if (enemiesWithTokens.Contains(enemy))
            {
                enemiesWithTokens.Remove(enemy);
            }
        }
        #endregion

        #region Properties
        public int ActiveEnemyCount => allEnemies.Count;
        public int ActiveAttackerCount => enemiesWithTokens.Count;
        #endregion
    }
}

