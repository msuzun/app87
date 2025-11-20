using UnityEngine;
using NeonSyndicate.Core;

namespace NeonSyndicate.Environment
{
    /// <summary>
    /// Kırılabilir objeler (kasalar, variller, lambalar).
    /// Crazy Flasher'daki gibi içinden item çıkabilir.
    /// 
    /// Kullanım:
    /// - Crate (Kasa) → Health pickup
    /// - Barrel (Varil) → Weapon
    /// - Trashcan → Money
    /// </summary>
    public class DestructibleObject : MonoBehaviour, IDamageable
    {
        [Header("Settings")]
        [SerializeField] private float maxHealth = 10f;
        [SerializeField] private bool isAlive = true;

        [Header("Rewards")]
        [Tooltip("Kırılınca spawn olacak item (opsiyonel)")]
        [SerializeField] private GameObject dropItem;
        
        [Tooltip("Item spawn şansı (0-1)")]
        [Range(0f, 1f)]
        [SerializeField] private float dropChance = 0.5f;

        [Header("Effects")]
        [SerializeField] private GameObject destroyEffectPrefab;
        [SerializeField] private Sprite brokenSprite;
        [SerializeField] private string destroySoundName = "Crate_Break";

        [Header("Physics")]
        [SerializeField] private bool applyPhysicsOnBreak = true;
        [SerializeField] private int fragmentCount = 4;

        private SpriteRenderer spriteRenderer;
        private Collider2D col;
        private float currentHealth;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            col = GetComponent<Collider2D>();
            currentHealth = maxHealth;
        }

        #region IDamageable Implementation
        public void TakeDamage(float damage, Vector2 impactDirection, Transform attacker = null)
        {
            if (!isAlive) return;

            currentHealth -= damage;

            // Hafif titreme efekti
            StartCoroutine(ShakeRoutine());

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public void Die()
        {
            if (!isAlive) return;

            isAlive = false;

            // Sprite değiştir (kırık hali)
            if (brokenSprite != null)
            {
                spriteRenderer.sprite = brokenSprite;
            }

            // Collider'ı kapat
            if (col != null)
            {
                col.enabled = false;
            }

            // Efekt spawn
            SpawnDestroyEffect();

            // Ses efekti
            SoundManager.Instance?.PlaySFX(destroySoundName);

            // Item drop
            if (Random.value < dropChance && dropItem != null)
            {
                DropItem();
            }

            // Fiziksel parçalara ayrılma (opsiyonel)
            if (applyPhysicsOnBreak)
            {
                CreateFragments();
            }

            // Objeyi yok et
            Destroy(gameObject, 1f);
        }

        public bool IsAlive()
        {
            return isAlive;
        }
        #endregion

        #region Effects
        private void SpawnDestroyEffect()
        {
            if (destroyEffectPrefab != null)
            {
                GameObject effect = Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);
                Destroy(effect, 2f);
            }
            else if (ObjectPooler.Instance != null)
            {
                ObjectPooler.Instance.SpawnFromPool("Debris_VFX", transform.position, Quaternion.identity);
            }
        }

        private void DropItem()
        {
            Vector3 dropPos = transform.position + Vector3.up * 0.5f;
            GameObject item = Instantiate(dropItem, dropPos, Quaternion.identity);

            // Item'e hafif upward force
            Rigidbody2D itemRb = item.GetComponent<Rigidbody2D>();
            if (itemRb != null)
            {
                itemRb.AddForce(Vector2.up * 3f, ForceMode2D.Impulse);
            }
        }

        private void CreateFragments()
        {
            // Basit fragment sistemi
            for (int i = 0; i < fragmentCount; i++)
            {
                GameObject fragment = new GameObject($"Fragment_{i}");
                fragment.transform.position = transform.position;

                // Sprite renderer
                SpriteRenderer fragSR = fragment.AddComponent<SpriteRenderer>();
                fragSR.sprite = spriteRenderer.sprite;
                fragSR.color = spriteRenderer.color;
                fragSR.sortingLayerName = spriteRenderer.sortingLayerName;
                fragSR.sortingOrder = spriteRenderer.sortingOrder;

                // Transform (smaller)
                fragment.transform.localScale = transform.localScale * 0.5f;

                // Rigidbody
                Rigidbody2D fragRb = fragment.AddComponent<Rigidbody2D>();
                fragRb.gravityScale = 1f;

                // Random force
                Vector2 randomForce = Random.insideUnitCircle * 5f + Vector2.up * 3f;
                fragRb.AddForce(randomForce, ForceMode2D.Impulse);
                fragRb.AddTorque(Random.Range(-10f, 10f));

                // Auto destroy
                Destroy(fragment, 3f);
            }
        }

        private System.Collections.IEnumerator ShakeRoutine()
        {
            Vector3 originalPos = transform.position;
            float duration = 0.1f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                transform.position = originalPos + (Vector3)Random.insideUnitCircle * 0.05f;
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = originalPos;
        }
        #endregion
    }
}

