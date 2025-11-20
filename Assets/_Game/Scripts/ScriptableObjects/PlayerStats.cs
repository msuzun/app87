using UnityEngine;

namespace NeonSyndicate.Data
{
    /// <summary>
    /// Oyuncu istatistiklerini tutan ScriptableObject.
    /// Upgrade sistemi için kullanılır.
    /// </summary>
    [CreateAssetMenu(fileName = "Player Stats", menuName = "Neon Syndicate/Player Stats")]
    public class PlayerStats : ScriptableObject
    {
        [Header("Base Stats")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float attackDamage = 10f;

        [Header("Advanced Stats")]
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float staminaRegenRate = 10f;
        [SerializeField] private float maxRage = 100f;
        [SerializeField] private float rageGainPerHit = 5f;

        [Header("Upgrade Costs")]
        [SerializeField] private int healthUpgradeCost = 100;
        [SerializeField] private int damageUpgradeCost = 150;
        [SerializeField] private int speedUpgradeCost = 120;

        #region Properties
        public float MaxHealth => maxHealth;
        public float MoveSpeed => moveSpeed;
        public float AttackDamage => attackDamage;
        public float MaxStamina => maxStamina;
        public float StaminaRegenRate => staminaRegenRate;
        public float MaxRage => maxRage;
        public float RageGainPerHit => rageGainPerHit;
        public int HealthUpgradeCost => healthUpgradeCost;
        public int DamageUpgradeCost => damageUpgradeCost;
        public int SpeedUpgradeCost => speedUpgradeCost;
        #endregion

        /// <summary>
        /// Stat'ı upgrade eder (Editor'de çağrılabilir).
        /// </summary>
        public void UpgradeHealth(float amount)
        {
            maxHealth += amount;
        }

        public void UpgradeDamage(float amount)
        {
            attackDamage += amount;
        }

        public void UpgradeSpeed(float amount)
        {
            moveSpeed += amount;
        }
    }
}

