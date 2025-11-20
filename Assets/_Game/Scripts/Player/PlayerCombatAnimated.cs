using UnityEngine;
using NeonSyndicate.Combat;
using NeonSyndicate.Animation;
using NeonSyndicate.Core;

namespace NeonSyndicate.Player
{
    /// <summary>
    /// PlayerCombat with Animation Event Integration
    /// Frame-perfect combat sistemi.
    /// 
    /// Eski PlayerCombat'tan farkı:
    /// - Hitbox activation tamamen animation event'lerle kontrol edilir
    /// - Timing süre sayımı yerine event-driven
    /// - Crazy Flasher tarzı frame-perfect vuruşlar
    /// </summary>
    [RequireComponent(typeof(CharacterAnimator))]
    public class PlayerCombatAnimated : MonoBehaviour
    {
        [Header("Combat Settings")]
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private float grabRange = 1.5f;

        [Header("Hitboxes")]
        [SerializeField] private Hitbox punchHitbox;
        [SerializeField] private Hitbox kickHitbox;

        [Header("Combo Settings")]
        [SerializeField] private int currentComboIndex = 0;
        [SerializeField] private int maxComboCount = 3;
        [SerializeField] private bool canCombo = false;

        // Components
        private CharacterAnimator characterAnimator;
        private PlayerController controller;

        private void Awake()
        {
            characterAnimator = GetComponent<CharacterAnimator>();
            controller = GetComponent<PlayerController>();
        }

        private void Start()
        {
            // Animation Event'lere abone ol
            SubscribeToEvents();

            // Başlangıçta hitbox'ları kapat
            punchHitbox?.Deactivate();
            kickHitbox?.Deactivate();
        }

        private void OnDestroy()
        {
            // Memory leak önleme
            UnsubscribeFromEvents();
        }

        #region Event Subscription
        private void SubscribeToEvents()
        {
            if (characterAnimator.EventReceiver == null) return;

            var receiver = characterAnimator.EventReceiver;

            // Combat events
            receiver.OnHitboxEnable += EnableCurrentHitbox;
            receiver.OnHitboxDisable += DisableAllHitboxes;
            receiver.OnComboWindowOpen += OpenComboWindow;
            receiver.OnComboWindowClose += CloseComboWindow;
            receiver.OnAnimationComplete += OnAttackComplete;

            // VFX events
            receiver.OnSpawnVFX += SpawnVFXEffect;
            receiver.OnCameraShake += TriggerCameraShake;

            // Movement events
            receiver.OnFootstep += PlayFootstepSound;
        }

        private void UnsubscribeFromEvents()
        {
            if (characterAnimator.EventReceiver == null) return;

            var receiver = characterAnimator.EventReceiver;

            receiver.OnHitboxEnable -= EnableCurrentHitbox;
            receiver.OnHitboxDisable -= DisableAllHitboxes;
            receiver.OnComboWindowOpen -= OpenComboWindow;
            receiver.OnComboWindowClose -= CloseComboWindow;
            receiver.OnAnimationComplete -= OnAttackComplete;
            receiver.OnSpawnVFX -= SpawnVFXEffect;
            receiver.OnCameraShake -= TriggerCameraShake;
            receiver.OnFootstep -= PlayFootstepSound;
        }
        #endregion

        #region Combat Input
        /// <summary>
        /// Light attack input (Z tuşu).
        /// </summary>
        public void OnLightAttackInput()
        {
            // Combo window içinde miyiz?
            if (canCombo)
            {
                ContinueCombo();
            }
            else if (!characterAnimator.IsPlayingState(AnimData.ATTACK_LIGHT_1) &&
                     !characterAnimator.IsPlayingState(AnimData.ATTACK_LIGHT_2) &&
                     !characterAnimator.IsPlayingState(AnimData.ATTACK_LIGHT_3))
            {
                StartCombo();
            }
        }

        /// <summary>
        /// Heavy attack input (X tuşu).
        /// </summary>
        public void OnHeavyAttackInput()
        {
            if (!CanAttack()) return;

            characterAnimator.PlayAnimation(AnimData.ATTACK_HEAVY, isCombatAction: true);
            SoundManager.Instance?.PlaySFX("Heavy_Swing");
        }

        private bool CanAttack()
        {
            // Oyuncu başka bir animasyon oynatıyorsa saldıramasın
            return !characterAnimator.IsPlayingState(AnimData.HIT) &&
                   !characterAnimator.IsPlayingState(AnimData.DEATH);
        }
        #endregion

        #region Combo System
        private void StartCombo()
        {
            currentComboIndex = 0;
            PlayComboAttack();
        }

        private void ContinueCombo()
        {
            currentComboIndex++;
            if (currentComboIndex >= maxComboCount)
            {
                currentComboIndex = 0;
            }

            PlayComboAttack();
            canCombo = false; // Window kapatıldı, event tekrar açacak
        }

        private void PlayComboAttack()
        {
            string attackAnim = AnimData.GetLightAttack(currentComboIndex + 1);
            characterAnimator.PlayAnimation(attackAnim, isCombatAction: true);

            SoundManager.Instance?.PlaySFX("Whoosh_Attack");
        }

        private void OnAttackComplete()
        {
            // Saldırı tamamlandı, idle'a dön
            currentComboIndex = 0;
            canCombo = false;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Animation Event Handler: Hitbox'ı aktifleştir.
        /// Animasyonun vuruş karesinde çağrılır.
        /// </summary>
        private void EnableCurrentHitbox()
        {
            // Hangi animasyon oynuyor?
            string currentAnim = characterAnimator.GetCurrentStateName();

            // Heavy attack ise kick hitbox, diğerleri punch
            if (currentAnim == AnimData.ATTACK_HEAVY || 
                currentAnim == AnimData.ATTACK_HEAVY_LAUNCHER)
            {
                kickHitbox?.Activate(transform);
            }
            else
            {
                punchHitbox?.Activate(transform);
            }

            Debug.Log("[PlayerCombat] Hitbox ENABLED (frame-perfect!)");
        }

        /// <summary>
        /// Animation Event Handler: Tüm hitbox'ları deaktif et.
        /// </summary>
        private void DisableAllHitboxes()
        {
            punchHitbox?.Deactivate();
            kickHitbox?.Deactivate();

            Debug.Log("[PlayerCombat] Hitboxes DISABLED");
        }

        /// <summary>
        /// Animation Event Handler: Combo window aç.
        /// </summary>
        private void OpenComboWindow()
        {
            canCombo = true;
            Debug.Log("[PlayerCombat] Combo Window OPEN");
        }

        /// <summary>
        /// Animation Event Handler: Combo window kapat.
        /// </summary>
        private void CloseComboWindow()
        {
            canCombo = false;
            Debug.Log("[PlayerCombat] Combo Window CLOSED");
        }

        /// <summary>
        /// Animation Event Handler: VFX spawn.
        /// </summary>
        private void SpawnVFXEffect(string effectName)
        {
            if (ObjectPooler.Instance != null)
            {
                ObjectPooler.Instance.SpawnFromPool(effectName, transform.position, Quaternion.identity);
            }
        }

        /// <summary>
        /// Animation Event Handler: Kamera sarsma.
        /// </summary>
        private void TriggerCameraShake(float intensity)
        {
            // CameraShake script'i varsa çağır
            Camera.main.transform.position += (Vector3)Random.insideUnitCircle * intensity;
        }

        /// <summary>
        /// Animation Event Handler: Ayak sesi.
        /// </summary>
        private void PlayFootstepSound()
        {
            SoundManager.Instance?.PlaySFX("Footstep");
        }
        #endregion

        #region Grab System
        public void AttemptGrab()
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, grabRange, enemyLayer);

            if (hitEnemies.Length > 0)
            {
                Transform closestEnemy = GetClosestEnemy(hitEnemies);
                if (closestEnemy != null)
                {
                    characterAnimator.PlayAnimation(AnimData.ATTACK_GRAB, isCombatAction: true);
                    Debug.Log($"Grabbed {closestEnemy.name}!");
                }
            }
        }

        private Transform GetClosestEnemy(Collider2D[] enemies)
        {
            Transform closest = null;
            float minDistance = Mathf.Infinity;

            foreach (var enemy in enemies)
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = enemy.transform;
                }
            }

            return closest;
        }
        #endregion

        private void OnDrawGizmosSelected()
        {
            // Grab range
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, grabRange);
        }
    }
}

