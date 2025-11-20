using UnityEngine;
using TMPro;
#if DOTWEEN_ENABLED
using DG.Tweening;
#endif

namespace NeonSyndicate.UI
{
    /// <summary>
    /// Damage Popup - Uçan hasar yazıları.
    /// 
    /// Crazy Flasher tarzı fizik bazlı hasar göstergesi:
    /// - Normal hit: Beyaz, küçük, yukarı çıkar
    /// - Critical: Sarı, büyük, titrer
    /// - Player hurt: Kırmızı, aşağı dökülen
    /// 
    /// Object Pooler ile kullanılmalıdır (performance).
    /// </summary>
    [RequireComponent(typeof(TextMeshPro))]
    public class DamagePopupUI : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float lifetime = 1f;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float gravity = 2f;

        [Header("Size")]
        [SerializeField] private float normalSize = 0.8f;
        [SerializeField] private float criticalSize = 1.5f;

        [Header("Colors")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color criticalColor = Color.yellow;
        [SerializeField] private Color playerHurtColor = Color.red;

        private TextMeshPro textMesh;
        private float disappearTimer;
        private Vector3 moveVector;
        private Color currentColor;
        private bool isCritical;

        private void Awake()
        {
            textMesh = GetComponent<TextMeshPro>();
        }

        /// <summary>
        /// Damage popup'ı başlatır.
        /// </summary>
        public void Setup(int damageAmount, bool critical = false, bool isPlayerDamage = false)
        {
            isCritical = critical;
            disappearTimer = lifetime;

            // Text ayarla
            textMesh.text = damageAmount.ToString();

            if (isPlayerDamage)
            {
                // Player hasar aldı (kırmızı, aşağı dökülen)
                textMesh.fontSize = normalSize;
                currentColor = playerHurtColor;
                moveVector = new Vector3(Random.Range(-0.5f, 0.5f), -1f) * moveSpeed;
            }
            else if (critical)
            {
                // Critical hit (büyük, sarı, sağa yukarı fırlayan)
                textMesh.fontSize = criticalSize;
                currentColor = criticalColor;
                moveVector = new Vector3(0.7f, 1.5f) * (moveSpeed * 1.5f);
                textMesh.text += "!"; // Ünlem ekle
            }
            else
            {
                // Normal hit
                textMesh.fontSize = normalSize;
                currentColor = normalColor;
                moveVector = new Vector3(Random.Range(-0.3f, 0.3f), 1f) * moveSpeed;
            }

            textMesh.color = currentColor;

            // Animation
            AnimateSpawn();
        }

        private void AnimateSpawn()
        {
            #if DOTWEEN_ENABLED
            if (isCritical)
            {
                // Critical hit punch scale
                transform.localScale = Vector3.zero;
                transform.DOScale(Vector3.one * 1.2f, 0.1f).SetEase(Ease.OutBack);
                
                // Shake
                transform.DOShakeRotation(0.2f, 20f);
            }
            else
            {
                // Normal pop
                transform.localScale = Vector3.zero;
                transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutQuad);
            }
            #else
            transform.localScale = Vector3.one;
            #endif
        }

        private void Update()
        {
            // Yukarı hareket (fizik simülasyonu)
            transform.position += moveVector * Time.deltaTime;
            
            // Yavaşlama (sürtünme + yerçekimi)
            moveVector.y -= gravity * Time.deltaTime;
            moveVector.x *= 0.95f; // Yatay sürtünme

            // Fade out
            disappearTimer -= Time.deltaTime;
            
            if (disappearTimer < 0.3f)
            {
                // Son 0.3 saniyede fade
                float alpha = disappearTimer / 0.3f;
                currentColor.a = alpha;
                textMesh.color = currentColor;
            }

            if (disappearTimer <= 0)
            {
                // Pool'a geri dön veya destroy
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Pool reset için.
        /// </summary>
        public void OnSpawn()
        {
            disappearTimer = lifetime;
            currentColor.a = 1f;
            textMesh.color = currentColor;
        }

        public void OnDespawn()
        {
            #if DOTWEEN_ENABLED
            transform.DOKill();
            #endif
        }
    }
}

