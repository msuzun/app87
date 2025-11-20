using UnityEngine;
using NeonSyndicate.Core;

namespace NeonSyndicate.Characters
{
    /// <summary>
    /// Hem oyuncu hem düşmanların ortak özellikleri.
    /// Abstract class olarak tasarlanmıştır.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public abstract class CharacterBase : MonoBehaviour, IDamageable, IKnockbackable
    {
        [Header("Stats")]
        [SerializeField] protected float maxHealth = 100f;
        [SerializeField] protected float currentHealth;
        [SerializeField] protected float moveSpeed = 5f;
        [SerializeField] protected float attackDamage = 10f;

        [Header("Combat State")]
        [SerializeField] protected bool isInvulnerable = false;
        [SerializeField] protected bool isDead = false;
        [SerializeField] protected bool isStunned = false;

        [Header("Knockback")]
        [SerializeField] protected float knockbackResistance = 1f;

        // Components
        protected Rigidbody2D rb;
        protected Animator animator;
        protected SpriteRenderer spriteRenderer;

        // Animation Hashes (Performans için string yerine hash kullan)
        protected static readonly int IsWalking = Animator.StringToHash("IsWalking");
        protected static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
        protected static readonly int IsDead = Animator.StringToHash("IsDead");
        protected static readonly int Hit = Animator.StringToHash("Hit");

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            currentHealth = maxHealth;
        }

        #region IDamageable Implementation
        public virtual void TakeDamage(float damage, Vector2 impactDirection, Transform attacker = null)
        {
            if (isDead || isInvulnerable) return;

            currentHealth -= damage;
            
            // Hasar efektleri
            OnDamageReceived(damage, impactDirection);
            
            // Kan efekti spawn (Object Pooler'dan)
            SpawnBloodEffect(transform.position);

            // Animator trigger
            animator.SetTrigger(Hit);

            // Ses efekti
            SoundManager.Instance?.PlaySFX("Hit_Impact");

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public virtual void Die()
        {
            if (isDead) return;

            isDead = true;
            animator.SetBool(IsDead, true);

            // Collider'ı kapat (artık hasar almasın)
            GetComponent<Collider2D>().enabled = false;

            OnDeath();
        }

        public bool IsAlive()
        {
            return !isDead;
        }
        #endregion

        #region IKnockbackable Implementation
        public virtual void ApplyKnockback(Vector2 direction, float force)
        {
            if (isDead) return;

            float finalForce = force / knockbackResistance;
            rb.AddForce(direction.normalized * finalForce, ForceMode2D.Impulse);
        }
        #endregion

        #region Abstract Methods (Alt sınıflar tarafından override edilmeli)
        protected abstract void OnDamageReceived(float damage, Vector2 impactDirection);
        protected abstract void OnDeath();
        #endregion

        #region Utility Methods
        protected void SpawnBloodEffect(Vector3 position)
        {
            if (ObjectPooler.Instance != null)
            {
                ObjectPooler.Instance.SpawnFromPool("BloodSplatter", position, Quaternion.identity);
            }
        }

        /// <summary>
        /// Karakteri belirli bir süre için "invulnerable" (hasar almaz) yapar.
        /// Dodge mekanizması için kullanılır.
        /// </summary>
        public void SetInvulnerable(float duration)
        {
            StartCoroutine(InvulnerabilityCoroutine(duration));
        }

        private System.Collections.IEnumerator InvulnerabilityCoroutine(float duration)
        {
            isInvulnerable = true;
            
            // Görsel feedback (yanıp sönme)
            float elapsed = 0f;
            while (elapsed < duration)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                yield return new WaitForSeconds(0.1f);
                elapsed += 0.1f;
            }

            spriteRenderer.enabled = true;
            isInvulnerable = false;
        }

        /// <summary>
        /// Karakterin baktığı yönü flip eder (sağa/sola dönme).
        /// </summary>
        protected void FlipSprite(float direction)
        {
            if (direction != 0)
            {
                spriteRenderer.flipX = direction < 0;
            }
        }
        #endregion

        #region Properties
        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;
        public float MoveSpeed => moveSpeed;
        public float AttackDamage => attackDamage;
        public bool IsDead => isDead;
        public bool IsStunned => isStunned;
        #endregion
    }
}

