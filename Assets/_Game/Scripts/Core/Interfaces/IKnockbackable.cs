using UnityEngine;

namespace NeonSyndicate.Core
{
    /// <summary>
    /// Geri savrulan (knockback) tüm karakterler bu interface'i kullanır.
    /// Crazy Flasher'daki uçan düşman hissi için kritik.
    /// </summary>
    public interface IKnockbackable
    {
        /// <summary>
        /// Karakteri belirli bir yöne ve güçte geri savurur.
        /// </summary>
        /// <param name="direction">Savurma yönü</param>
        /// <param name="force">Savurma gücü</param>
        void ApplyKnockback(Vector2 direction, float force);
    }
}

