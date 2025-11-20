using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using NeonSyndicate.Core;

namespace NeonSyndicate.Combat
{
    /// <summary>
    /// PRO-LEVEL COMBO SYSTEM
    /// Data-driven, branching combos, input buffering, cancel windows.
    /// Street Fighter / Devil May Cry tarzı gelişmiş combo motoru.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(InputBuffer))]
    public class ProComboSystem : MonoBehaviour
    {
        [Header("Defaults")]
        [Tooltip("İlk saldırı (örn: Light Attack başlangıcı)")]
        [SerializeField] private ComboMoveSO defaultLightOpener;
        
        [Tooltip("Heavy attack başlangıcı")]
        [SerializeField] private ComboMoveSO defaultHeavyOpener;

        [Header("Current State")]
        [SerializeField] private ComboMoveSO currentMove;
        [SerializeField] private bool isAttacking = false;
        [SerializeField] private bool isInCancelWindow = false;

        [Header("Combo Counter")]
        [SerializeField] private int comboCounter = 0;
        [SerializeField] private float comboTimeout = 2f;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = false;

        [Header("Events")]
        public UnityEvent<int> OnComboCountUpdate;
        public UnityEvent OnComboReset;
        public UnityEvent<float> OnHitConfirm; // Hit confirm (hasar)

        // Components
        private Animator animator;
        private InputBuffer inputBuffer;
        private Rigidbody2D rb;

        // Timing
        private float attackTimer;
        private float lastHitTime;

        // Cache
        private WaitForSecondsRealtime hitStopWait;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            inputBuffer = GetComponent<InputBuffer>();
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            // Combo timeout kontrolü
            HandleComboTimeout();

            // Input okuma ve buffer'a kaydetme
            ReadInputs();

            if (isAttacking)
            {
                attackTimer += Time.deltaTime;
                UpdateCancelWindow();
                CheckForNextMove();
            }
            else
            {
                // Saldırmıyorsak, buffer'dan yeni kombo başlat
                TryStartCombo();
            }
        }

        #region Input Handling
        /// <summary>
        /// Inputları okur ve buffer'a kaydeder.
        /// NEW INPUT SYSTEM kullanımı.
        /// </summary>
        private void ReadInputs()
        {
            // InputHandler'dan input al (zaten New Input System kullanıyor)
            if (InputHandler.Instance.IsAttackPressed)
            {
                inputBuffer.RegisterInput(InputType.Light);
            }

            if (InputHandler.Instance.IsHeavyAttackPressed)
            {
                inputBuffer.RegisterInput(InputType.Heavy);
            }

            if (InputHandler.Instance.IsGrabPressed)
            {
                inputBuffer.RegisterInput(InputType.Special);
            }

            if (InputHandler.Instance.IsJumpPressed)
            {
                inputBuffer.RegisterInput(InputType.Jump);
            }
        }
        #endregion

        #region Combo Flow
        /// <summary>
        /// Yeni kombo başlatmaya çalışır.
        /// </summary>
        private void TryStartCombo()
        {
            InputType input = inputBuffer.TryGetBufferedInput();
            if (input == InputType.None) return;

            ComboMoveSO opener = null;

            // Input tipine göre opener seç
            switch (input)
            {
                case InputType.Light:
                    opener = defaultLightOpener;
                    break;

                case InputType.Heavy:
                    opener = defaultHeavyOpener;
                    break;

                case InputType.Special:
                    // Special opener (ileride eklenebilir)
                    break;
            }

            if (opener != null)
            {
                inputBuffer.ConsumeInput();
                PlayMove(opener);
            }
        }

        /// <summary>
        /// Mevcut combo'nun devamını kontrol eder.
        /// </summary>
        private void CheckForNextMove()
        {
            // Animasyon bitti mi?
            if (attackTimer >= currentMove.animationLength)
            {
                ResetToIdle();
                return;
            }

            // Cancel window içinde miyiz?
            if (!isInCancelWindow) return;

            // Buffer'da input var mı?
            InputType nextInput = inputBuffer.TryGetBufferedInput();
            if (nextInput == InputType.None) return;

            // Mevcut hareketin dallanma listesine bak
            foreach (var branch in currentMove.nextMoves)
            {
                if (branch.requiredInput == nextInput)
                {
                    // Şartları kontrol et
                    if (branch.requiresAirborne && IsGrounded())
                    {
                        continue; // Havada olmalıydı ama yerdeyiz
                    }

                    if (branch.minimumComboCount > comboCounter)
                    {
                        continue; // Combo counter yeterli değil
                    }

                    // Şartlar sağlandı, zinciri devam ettir
                    inputBuffer.ConsumeInput();
                    PlayMove(branch.nextMove);
                    return;
                }
            }
        }

        /// <summary>
        /// Cancel window'u günceller.
        /// </summary>
        private void UpdateCancelWindow()
        {
            float progress = attackTimer / currentMove.animationLength;
            isInCancelWindow = progress >= currentMove.minCancelTime && 
                               progress <= currentMove.maxCancelTime;
        }
        #endregion

        #region Move Execution
        /// <summary>
        /// Hareketi çalıştırır.
        /// </summary>
        private void PlayMove(ComboMoveSO move)
        {
            currentMove = move;
            isAttacking = true;
            attackTimer = 0f;
            isInCancelWindow = false;

            // Animasyon çal (CrossFade smooth geçiş sağlar)
            animator.CrossFade(move.animationName, 0.1f);

            // Ses efekti
            SoundManager.Instance?.PlaySFX(move.attackSoundName);

            // Momentum (ileri hareket)
            if (rb != null && move.forwardMomentum > 0)
            {
                Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
                rb.velocity = direction * move.forwardMomentum;
            }

            if (showDebugInfo)
            {
                Debug.Log($"[ProCombo] Playing: {move.name}");
            }
        }

        /// <summary>
        /// Idle state'e dön.
        /// </summary>
        private void ResetToIdle()
        {
            isAttacking = false;
            currentMove = null;
            attackTimer = 0f;
            isInCancelWindow = false;

            animator.CrossFade("Idle", 0.2f);

            if (rb != null)
            {
                rb.velocity = Vector2.zero;
            }
        }
        #endregion

        #region Hit Confirmation
        /// <summary>
        /// Düşmana vurulduğunda çağrılır (Hitbox'tan).
        /// Public - external scripts çağırabilir.
        /// </summary>
        public void OnHitEnemy()
        {
            if (currentMove == null) return;

            // 1. Hit Stop (Freeze Frame)
            if (currentMove.hitStopDuration > 0)
            {
                StartCoroutine(HitStopRoutine(currentMove.hitStopDuration));
            }

            // 2. Combo Counter
            comboCounter++;
            lastHitTime = Time.time;
            OnComboCountUpdate?.Invoke(comboCounter);

            // 3. Hit Confirm Event (hasar bilgisi)
            OnHitConfirm?.Invoke(currentMove.damage);

            // 4. Ses efekti
            SoundManager.Instance?.PlaySFX(currentMove.hitSoundName);

            if (showDebugInfo)
            {
                Debug.Log($"[ProCombo] Hit Confirm! Combo: {comboCounter}, Damage: {currentMove.damage}");
            }
        }

        /// <summary>
        /// Hit Stop - Vuruş anında oyunun kısa süre donması.
        /// Crazy Flasher'daki o tatmin edici "impact" hissi.
        /// </summary>
        private IEnumerator HitStopRoutine(float duration)
        {
            float originalTimeScale = Time.timeScale;
            Time.timeScale = 0.05f; // Tamamen durmasın, çok yavaşlasın

            yield return new WaitForSecondsRealtime(duration);

            Time.timeScale = originalTimeScale;
        }
        #endregion

        #region Combo Counter Management
        /// <summary>
        /// Combo timeout kontrolü.
        /// </summary>
        private void HandleComboTimeout()
        {
            if (comboCounter > 0 && Time.time - lastHitTime > comboTimeout)
            {
                ResetComboCounter();
            }
        }

        /// <summary>
        /// Combo counter'ı sıfırlar.
        /// </summary>
        public void ResetComboCounter()
        {
            if (comboCounter == 0) return;

            comboCounter = 0;
            OnComboReset?.Invoke();

            if (showDebugInfo)
            {
                Debug.Log("[ProCombo] Combo Reset!");
            }
        }
        #endregion

        #region Utility
        /// <summary>
        /// Karakter yerde mi?
        /// </summary>
        private bool IsGrounded()
        {
            // PlayerController'dan alınabilir veya raycast ile check edilebilir
            // Şimdilik basit versiyon:
            return Physics2D.Raycast(transform.position, Vector2.down, 0.1f);
        }

        /// <summary>
        /// Force interrupt - Harici state değişimlerinde çağrılabilir.
        /// </summary>
        public void ForceInterrupt()
        {
            if (isAttacking)
            {
                ResetToIdle();
                inputBuffer.ClearBuffer();
            }
        }
        #endregion

        #region Public Properties
        public bool IsAttacking => isAttacking;
        public ComboMoveSO CurrentMove => currentMove;
        public int ComboCount => comboCounter;
        public bool IsInCancelWindow => isInCancelWindow;
        #endregion

        #region Debug Gizmos
        private void OnGUI()
        {
            if (!showDebugInfo) return;

            GUILayout.BeginArea(new Rect(10, 100, 300, 200));
            GUILayout.Label($"=== PRO COMBO DEBUG ===");
            GUILayout.Label($"Attacking: {isAttacking}");
            GUILayout.Label($"Current Move: {(currentMove != null ? currentMove.name : "None")}");
            GUILayout.Label($"Timer: {attackTimer:F2} / {(currentMove != null ? currentMove.animationLength : 0):F2}");
            GUILayout.Label($"In Cancel Window: {isInCancelWindow}");
            GUILayout.Label($"Combo Counter: {comboCounter}");
            GUILayout.Label($"Buffer Count: {inputBuffer.BufferCount}");
            GUILayout.EndArea();
        }
        #endregion
    }
}

