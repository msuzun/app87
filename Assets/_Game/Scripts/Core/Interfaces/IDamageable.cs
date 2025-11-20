using UnityEngine;

namespace NeonSyndicate.Core
{
    /// <summary>
    /// Hasar alabilecek tüm objeler (Oyuncu, Düşman, Kırılabilir objeler) 
    /// bu interface'i implement eder.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Obje hasar aldığında çağrılır.
        /// </summary>
        /// <param name="damage">Alınan hasar miktarı</param>
        /// <param name="impactDirection">Darbenin geldiği yön (knockback için)</param>
        /// <param name="attacker">Saldıranın Transform'u (opsiyonel)</param>
        void TakeDamage(float damage, Vector2 impactDirection, Transform attacker = null);

        /// <summary>
        /// Objenin öldüğünde çağrılır.
        /// </summary>
        void Die();

        /// <summary>
        /// Objenin hala hayatta olup olmadığını döner.
        /// </summary>
        bool IsAlive();
    }
}

