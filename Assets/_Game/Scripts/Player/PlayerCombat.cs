using UnityEngine;
using NeonSyndicate.Combat;
using NeonSyndicate.Core;

namespace NeonSyndicate.Player
{
    /// <summary>
    /// Oyuncunun dövüş mekanizmalarını yöneten sınıf.
    /// Combo sistemi ve hitbox aktivasyonu burada gerçekleşir.
    /// </summary>
    [RequireComponent(typeof(ComboSystem))]
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Combat Settings")]
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private float grabRange = 1.5f;

        [Header("Hitboxes")]
        [SerializeField] private Hitbox punchHitbox;
        [SerializeField] private Hitbox kickHitbox;

        [Header("Style System")]
        [SerializeField] private float styleDecayRate = 10f;
        [SerializeField] private float currentStyle = 0f;
        private int consecutiveHits = 0;

        private ComboSystem comboSystem;
        private PlayerController controller;
        private Transform currentTarget;

        private void Awake()
        {
            comboSystem = GetComponent<ComboSystem>();
            controller = GetComponent<PlayerController>();
        }

        private void Start()
        {
            // Combo sistemi event'lerine abone ol
            comboSystem.OnComboAdvanced += OnComboProgressed;
            comboSystem.OnComboReset += OnComboEnded;
        }

        private void Update()
        {
            // Stil puanı zaman içinde azalır
            if (currentStyle > 0)
            {
                currentStyle -= styleDecayRate * Time.deltaTime;
                currentStyle = Mathf.Max(0, currentStyle);
            }
        }

        #region Combo Execution
        /// <summary>
        /// Kombonun sonraki adımını çalıştırır.
        /// </summary>
        public void ExecuteNextCombo()
        {
            comboSystem.ExecuteNextAttack();
        }

        /// <summary>
        /// Kombonun devam edip edemeyeceğini kontrol eder.
        /// </summary>
        public bool CanContinueCombo()
        {
            return comboSystem.CanContinueCombo();
        }
        #endregion

        #region Hitbox Activation (Animation Events'ten çağrılır)
        /// <summary>
        /// Yumruk hitbox'ını aktifleştirir (Animator Event).
        /// </summary>
        public void ActivatePunchHitbox()
        {
            if (punchHitbox != null)
            {
                float damage = comboSystem.GetCurrentDamage();
                punchHitbox.Activate(transform, damage);
            }
        }

        /// <summary>
        /// Tekme hitbox'ını aktifleştirir (Animator Event).
        /// </summary>
        public void ActivateKickHitbox()
        {
            if (kickHitbox != null)
            {
                float damage = comboSystem.GetCurrentDamage();
                kickHitbox.Activate(transform, damage);
            }
        }

        /// <summary>
        /// Hitbox'ları deaktif eder (Animator Event).
        /// </summary>
        public void DeactivateHitboxes()
        {
            punchHitbox?.Deactivate();
            kickHitbox?.Deactivate();
        }
        #endregion

        #region Grab System
        /// <summary>
        /// En yakın düşmanı yakalar.
        /// </summary>
        public void AttemptGrab()
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, grabRange, enemyLayer);

            if (hitEnemies.Length > 0)
            {
                // En yakın düşmanı bul
                Transform closestEnemy = null;
                float closestDistance = Mathf.Infinity;

                foreach (Collider2D enemy in hitEnemies)
                {
                    float distance = Vector2.Distance(transform.position, enemy.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = enemy.transform;
                    }
                }

                if (closestEnemy != null)
                {
                    GrabEnemy(closestEnemy);
                }
            }
        }

        private void GrabEnemy(Transform enemy)
        {
            currentTarget = enemy;
            Debug.Log($"Grabbed {enemy.name}!");

            // Düşmanı "grabbed" state'e al (EnemyAI'da implement edilmeli)
            // enemy.GetComponent<EnemyAI>()?.SetGrabbed(true);

            SoundManager.Instance?.PlaySFX("Grab_Success");
        }
        #endregion

        #region Style System
        private void OnComboProgressed(int comboIndex)
        {
            consecutiveHits++;
            currentStyle += 10f * consecutiveHits;

            // Rage kazan
            controller.AddRage(controller.RageMax * 0.1f);

            // Stil derecesi hesapla
            string styleRank = CalculateStyleRank();
            Debug.Log($"Style: {styleRank} ({currentStyle:F0})");
        }

        private void OnComboEnded(int index)
        {
            consecutiveHits = 0;
        }

        private string CalculateStyleRank()
        {
            if (currentStyle < 30) return "D";
            if (currentStyle < 60) return "C";
            if (currentStyle < 100) return "B";
            if (currentStyle < 150) return "A";
            if (currentStyle < 200) return "S";
            if (currentStyle < 300) return "SS";
            return "SSS";
        }
        #endregion

        #region Properties
        public float CurrentStyle => currentStyle;
        public int ConsecutiveHits => consecutiveHits;
        #endregion

        private void OnDrawGizmosSelected()
        {
            // Grab range'i göster
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, grabRange);
        }
    }
}

