using UnityEngine;

namespace NeonSyndicate.Data
{
    /// <summary>
    /// Düşman istatistiklerini tutan ScriptableObject.
    /// Unity Editor'de Create > Neon Syndicate > Enemy Stats ile oluşturulur.
    /// </summary>
    [CreateAssetMenu(fileName = "New Enemy Stats", menuName = "Neon Syndicate/Enemy Stats")]
    public class EnemyStats : ScriptableObject
    {
        [Header("Enemy Information")]
        [SerializeField] private string enemyName = "Thug";
        [TextArea(2, 4)]
        [SerializeField] private string description = "Temel düşman tipi";

        [Header("Combat Stats")]
        [SerializeField] private float maxHealth = 50f;
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float attackCooldown = 2f;

        [Header("Defense")]
        [SerializeField] private float knockbackResistance = 1f;
        [SerializeField] private bool hasArmor = false;

        [Header("Rewards")]
        [SerializeField] private int scoreValue = 100;
        [SerializeField] private int moneyDrop = 10;

        [Header("AI Behavior")]
        [SerializeField] private float detectionRange = 8f;
        [SerializeField] private float aggressiveness = 0.5f; // 0-1 arası

        #region Properties
        public string EnemyName => enemyName;
        public string Description => description;
        public float MaxHealth => maxHealth;
        public float MoveSpeed => moveSpeed;
        public float AttackDamage => attackDamage;
        public float AttackRange => attackRange;
        public float AttackCooldown => attackCooldown;
        public float KnockbackResistance => knockbackResistance;
        public bool HasArmor => hasArmor;
        public int ScoreValue => scoreValue;
        public int MoneyDrop => moneyDrop;
        public float DetectionRange => detectionRange;
        public float Aggressiveness => aggressiveness;
        #endregion
    }
}

