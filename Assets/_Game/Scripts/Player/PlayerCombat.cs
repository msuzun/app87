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

            // Combo timeout kontrolü (ek güvenlik)
            // ComboSystem zaten yapıyor ama double-check
            if (Time.time - lastAttackTime > 1.5f && comboSystem.CurrentComboIndex > 0)
            {
                comboSystem.ResetCombo();
            }
        }
        
        private float lastAttackTime;

        #region Combo Execution
        /// <summary>
        /// Kombonun sonraki adımını çalıştırır.
        /// </summary>
        public void ExecuteNextCombo()
        {
            lastAttackTime = Time.time;
            
            // Air attack kontrolü
            if (!controller.isGrounded)
            {
                ExecuteAirAttack();
                return;
            }
            
            comboSystem.ExecuteNextAttack();
            
            // Momentum ekle (Crazy Flasher hissi için)
            ApplyAttackMomentum();
        }

        /// <summary>
        /// Kombonun devam edip edemeyeceğini kontrol eder.
        /// </summary>
        public bool CanContinueCombo()
        {
            return comboSystem.CanContinueCombo();
        }

        /// <summary>
        /// Havada saldırı yapar.
        /// </summary>
        private void ExecuteAirAttack()
        {
            // Air attack animasyonu
            controller.Animator.Play("Attack_Air");
            
            // Havada kısa süre asılı kalma (Gravity defying - Crazy Flasher tarzı)
            controller.Rb.velocity = Vector2.zero;
            
            // Hitbox aktivasyonu
            controller.Invoke("ActivateKickHitbox", 0.1f);
            
            Debug.Log("Air Attack!");
        }

        /// <summary>
        /// Saldırı sırasında hafif ileri momentum (Crazy Flasher hissi).
        /// </summary>
        private void ApplyAttackMomentum()
        {
            Vector2 attackDirection = controller.isFacingRight ? Vector2.right : Vector2.left;
            controller.Rb.velocity = attackDirection * 2f; // Hafif ileri hareket
        }
        #endregion

        #region Hitbox Activation (Animation Events'ten çağrılır)
        /// <summary>
        /// Yumruk hitbox'ını aktifleştirir (Animator Event).
        /// Animasyonun vuruş karesinde çağrılmalı (örn: frame 3).
        /// </summary>
        public void ActivatePunchHitbox()
        {
            if (punchHitbox != null)
            {
                float damage = comboSystem.GetCurrentDamage();
                punchHitbox.Activate(transform, damage);
                
                // Ses efekti
                SoundManager.Instance?.PlaySFX("Whoosh_Attack");
            }
        }

        /// <summary>
        /// Tekme hitbox'ını aktifleştirir (Animator Event).
        /// Animasyonun vuruş karesinde çağrılmalı (örn: frame 4).
        /// </summary>
        public void ActivateKickHitbox()
        {
            if (kickHitbox != null)
            {
                float damage = comboSystem.GetCurrentDamage();
                kickHitbox.Activate(transform, damage);
                
                // Ses efekti
                SoundManager.Instance?.PlaySFX("Whoosh_Kick");
            }
        }

        /// <summary>
        /// Hitbox'ları deaktif eder (Animator Event).
        /// Animasyonun bitişinde çağrılmalı.
        /// </summary>
        public void DeactivateHitboxes()
        {
            punchHitbox?.Deactivate();
            kickHitbox?.Deactivate();
        }

        /// <summary>
        /// Saldırı animasyonu tamamlandığında çağrılır (Animator Event).
        /// State'i Idle'a döndürür.
        /// </summary>
        public void OnAttackComplete()
        {
            // State Machine'e bildir
            if (controller.StateMachine.CurrentState == controller.StateMachine.AttackState)
            {
                controller.StateMachine.ChangeState(controller.StateMachine.IdleState);
            }
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

