using UnityEngine;
using NeonSyndicate.Characters;
using NeonSyndicate.Core;

namespace NeonSyndicate.Enemy
{
    /// <summary>
    /// Düşman karakterler için base controller.
    /// CharacterBase'den türer ve AI ile entegre çalışır.
    /// </summary>
    [RequireComponent(typeof(EnemyAI))]
    public class EnemyController : CharacterBase
    {
        [Header("Enemy Type")]
        [SerializeField] private EnemyType enemyType;
        
        [Header("Rewards")]
        [SerializeField] private int scoreValue = 100;
        [SerializeField] private int moneyDrop = 10;

        [Header("AI Components")]
        private EnemyAI ai;

        public enum EnemyType
        {
            Thug,       // Temel düşman
            Biker,      // Kasklı, silahlı
            KnifeJack,  // Hızlı bıçakçı
            FatBoy,     // Ağır abi
            Gunner      // Silahlı
        }

        protected override void Awake()
        {
            base.Awake();
            ai = GetComponent<EnemyAI>();
        }

        private void Start()
        {
            // Düşmana özgü stat ayarlamaları
            ApplyEnemyTypeStats();
        }

        protected override void OnDamageReceived(float damage, Vector2 impactDirection)
        {
            // AI'ya hasar alındığını bildir
            ai?.OnDamageReceived();

            // Ekran shake (hafif)
            // Camera shake burada daha hafif olmalı
        }

        protected override void OnDeath()
        {
            Debug.Log($"{enemyType} died!");

            // Skor ve para ver
            GameManager.Instance?.AddScore(scoreValue);
            GameManager.Instance?.AddMoney(moneyDrop);

            // AI'yı durdur
            ai?.OnDeath();

            // Ölü düşmanı 3 saniye sonra yok et
            Destroy(gameObject, 3f);
        }

        private void ApplyEnemyTypeStats()
        {
            switch (enemyType)
            {
                case EnemyType.Thug:
                    maxHealth = 50f;
                    moveSpeed = 3f;
                    attackDamage = 10f;
                    break;

                case EnemyType.Biker:
                    maxHealth = 70f;
                    moveSpeed = 4f;
                    attackDamage = 15f;
                    break;

                case EnemyType.KnifeJack:
                    maxHealth = 40f;
                    moveSpeed = 6f;
                    attackDamage = 12f;
                    break;

                case EnemyType.FatBoy:
                    maxHealth = 150f;
                    moveSpeed = 2f;
                    attackDamage = 25f;
                    knockbackResistance = 3f;
                    break;

                case EnemyType.Gunner:
                    maxHealth = 60f;
                    moveSpeed = 3.5f;
                    attackDamage = 8f;
                    break;
            }

            currentHealth = maxHealth;
        }

        #region Properties
        public EnemyType Type => enemyType;
        public EnemyAI AI => ai;
        #endregion
    }
}

