using UnityEngine;
using NeonSyndicate.Core;

namespace NeonSyndicate.Combat
{
    /// <summary>
    /// Hasar veren bölge (el, ayak, silah).
    /// Animasyon eventleri ile aktif/deaktif edilir.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Hitbox : MonoBehaviour
    {
        [Header("Damage Settings")]
        [SerializeField] private float damage = 10f;
        [SerializeField] private float knockbackForce = 5f;
        
        [Header("Hit Properties")]
        [SerializeField] private LayerMask targetLayers;
        [SerializeField] private bool canHitMultiple = false;
        [SerializeField] private float activeTime = 0.1f;

        [Header("Visual Effects")]
        [SerializeField] private GameObject hitSparkPrefab;
        [SerializeField] private bool showDebugGizmo = true;

        private Collider2D hitboxCollider;
        private Transform owner;
        private float timer;
        private bool isActive;

        private void Awake()
        {
            hitboxCollider = GetComponent<Collider2D>();
            hitboxCollider.isTrigger = true;
            hitboxCollider.enabled = false;
        }

        /// <summary>
        /// Hitbox'ı aktifleştirir (Animasyon eventi ile çağrılır).
        /// </summary>
        public void Activate(Transform ownerTransform, float customDamage = -1f)
        {
            owner = ownerTransform;
            isActive = true;
            timer = 0f;
            hitboxCollider.enabled = true;

            if (customDamage > 0)
            {
                damage = customDamage;
            }
        }

        /// <summary>
        /// Hitbox'ı deaktif eder.
        /// </summary>
        public void Deactivate()
        {
            isActive = false;
            hitboxCollider.enabled = false;
        }

        private void Update()
        {
            if (isActive)
            {
                timer += Time.deltaTime;
                if (timer >= activeTime)
                {
                    Deactivate();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isActive) return;

            // Kendine vurmayı engelle
            if (other.transform == owner || other.transform.IsChildOf(owner))
                return;

            // Layer check
            if (((1 << other.gameObject.layer) & targetLayers) == 0)
                return;

            // IDamageable interface kontrolü
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsAlive())
            {
                // Darbe yönünü hesapla
                Vector2 impactDirection = (other.transform.position - owner.position).normalized;

                // Hasar ver
                damageable.TakeDamage(damage, impactDirection, owner);

                // Knockback uygula
                IKnockbackable knockbackable = other.GetComponent<IKnockbackable>();
                knockbackable?.ApplyKnockback(impactDirection, knockbackForce);

                // Hit efekti oluştur
                SpawnHitEffect(other.ClosestPoint(transform.position));

                // Ses efekti
                SoundManager.Instance?.PlaySFX("Hit_Impact");

                // Hitstop (zaman yavaşlatma) efekti
                ApplyHitstop();

                // Tek vuruşsa deaktif et
                if (!canHitMultiple)
                {
                    Deactivate();
                }
            }
        }

        private void SpawnHitEffect(Vector3 position)
        {
            if (ObjectPooler.Instance != null)
            {
                ObjectPooler.Instance.SpawnFromPool("HitSpark", position, Quaternion.identity);
            }
            else if (hitSparkPrefab != null)
            {
                Instantiate(hitSparkPrefab, position, Quaternion.identity);
            }
        }

        /// <summary>
        /// Crazy Flasher'daki gibi vuruş anında milisaniyelik duraklama.
        /// </summary>
        private void ApplyHitstop()
        {
            Time.timeScale = 0.1f;
            Invoke(nameof(ResetTimeScale), 0.05f);
        }

        private void ResetTimeScale()
        {
            Time.timeScale = 1f;
        }

        private void OnDrawGizmos()
        {
            if (!showDebugGizmo) return;

            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                Gizmos.color = isActive ? Color.red : Color.yellow;
                Gizmos.DrawWireSphere(transform.position, 0.3f);
            }
        }
    }
}

