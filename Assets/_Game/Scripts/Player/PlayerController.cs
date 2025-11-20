using UnityEngine;
using System.Collections;
using NeonSyndicate.Characters;
using NeonSyndicate.Core;

namespace NeonSyndicate.Player
{
    /// <summary>
    /// Oyuncu karakteri (Axel) için ana kontrolcü.
    /// CharacterBase'den türer ve oyuncuya özel özellikleri içerir.
    /// HYBRID SYSTEM: Class-based FSM + Coroutine actions
    /// </summary>
    [RequireComponent(typeof(PlayerStateMachine))]
    [RequireComponent(typeof(PlayerCombat))]
    public class PlayerController : CharacterBase
    {
        [Header("Movement Settings")]
        [SerializeField] private float runSpeedMultiplier = 1.6f;
        [SerializeField] private float dashForce = 15f;
        [SerializeField] private float dashDuration = 0.2f;
        
        [Header("Jump Settings")]
        [SerializeField] private float jumpHeight = 2f;
        [SerializeField] private float jumpDuration = 0.6f;
        [SerializeField] private float airControlMultiplier = 0.5f;

        [Header("Player Specific Stats")]
        [SerializeField] private float staminaMax = 100f;
        [SerializeField] private float staminaCurrent;
        [SerializeField] private float staminaRegenRate = 10f;
        [SerializeField] private float dodgeStaminaCost = 20f;

        [Header("Rage System")]
        [SerializeField] private float rageMax = 100f;
        [SerializeField] private float rageCurrent = 0f;
        [SerializeField] private float rageGainPerHit = 5f;

        [Header("State Flags")]
        [HideInInspector] public bool isGrounded = true;
        [HideInInspector] public bool isFacingRight = true;
        [HideInInspector] public bool isRunning = false;

        [Header("Components")]
        private PlayerStateMachine stateMachine;
        private PlayerCombat combat;
        
        // Coroutine tracking
        private Coroutine currentActionCoroutine;

        protected override void Awake()
        {
            base.Awake();

            stateMachine = GetComponent<PlayerStateMachine>();
            combat = GetComponent<PlayerCombat>();

            staminaCurrent = staminaMax;
        }

        private void Update()
        {
            // Stamina regeneration
            if (staminaCurrent < staminaMax)
            {
                staminaCurrent += staminaRegenRate * Time.deltaTime;
                staminaCurrent = Mathf.Clamp(staminaCurrent, 0, staminaMax);
            }

            // Check run input
            isRunning = InputHandler.Instance.IsRunPressed && staminaCurrent > 0;
            
            // Consume stamina while running
            if (isRunning && InputHandler.Instance.MovementInput.magnitude > 0.1f)
            {
                staminaCurrent -= 5f * Time.deltaTime;
            }
        }
        
        private void LateUpdate()
        {
            // Auto-flip sprite based on movement
            UpdateFacing();
        }
        
        /// <summary>
        /// Karakterin baktığı yönü otomatik günceller.
        /// </summary>
        private void UpdateFacing()
        {
            Vector2 input = InputHandler.Instance.MovementInput;
            
            if (input.x > 0.1f && !isFacingRight)
            {
                Flip();
            }
            else if (input.x < -0.1f && isFacingRight)
            {
                Flip();
            }
        }
        
        /// <summary>
        /// Sprite'ı ters çevirir.
        /// </summary>
        public void Flip()
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

        #region Override Methods
        protected override void OnDamageReceived(float damage, Vector2 impactDirection)
        {
            // State'i Hurt'e çevir
            if (stateMachine != null)
            {
                stateMachine.ChangeState(stateMachine.HurtState);
            }

            // Ekran shake efekti
            CameraShake();
        }

        protected override void OnDeath()
        {
            Debug.Log("Player Died!");

            // Death state'e geç
            if (stateMachine != null)
            {
                stateMachine.ChangeState(stateMachine.DeathState);
            }

            // Skor kaydet
            PlayerPrefs.SetInt("LastScore", GameManager.Instance.CurrentScore);
        }
        #endregion

        #region Stamina System
        public bool UseStamina(float amount)
        {
            if (staminaCurrent >= amount)
            {
                staminaCurrent -= amount;
                return true;
            }
            return false;
        }

        public void RestoreStamina(float amount)
        {
            staminaCurrent = Mathf.Clamp(staminaCurrent + amount, 0, staminaMax);
        }
        #endregion

        #region Rage System
        public void AddRage(float amount)
        {
            rageCurrent = Mathf.Clamp(rageCurrent + amount, 0, rageMax);
        }

        public bool CanUseExecutionMove()
        {
            return rageCurrent >= rageMax;
        }

        public void ConsumeRage()
        {
            rageCurrent = 0f;
        }
        #endregion

        #region Visual Feedback
        private void CameraShake()
        {
            // Basit kamera sarsma efekti
            // Gerçek üretimde Cinemachine kullanılabilir
            Camera.main.transform.position += (Vector3)Random.insideUnitCircle * 0.1f;
        }
        #endregion

        #region Coroutine Actions (Hybrid System)
        /// <summary>
        /// Dash coroutine (i-frame ile).
        /// State'den çağrılır.
        /// </summary>
        public IEnumerator DashCoroutine()
        {
            // Stamina kontrolü
            if (!UseStamina(dodgeStaminaCost))
            {
                yield break;
            }

            // Dash yönü belirle
            Vector2 input = InputHandler.Instance.MovementInput;
            Vector2 dashDirection = input.magnitude > 0.1f 
                ? input.normalized 
                : (isFacingRight ? Vector2.right : Vector2.left);

            // I-frame aktif et
            SetInvulnerable(dashDuration);

            // Dash hareketi
            float elapsed = 0f;
            while (elapsed < dashDuration)
            {
                rb.velocity = dashDirection * dashForce;
                elapsed += Time.deltaTime;
                yield return null;
            }

            rb.velocity = Vector2.zero;
        }

        /// <summary>
        /// Jump coroutine (fake height ile 2.5D jump).
        /// State'den çağrılır.
        /// </summary>
        public IEnumerator JumpCoroutine()
        {
            isGrounded = false;
            
            // Başlangıç pozisyonu
            Vector3 startPos = transform.position;
            float elapsed = 0f;

            // Shadow oluştur (opsiyonel)
            GameObject shadow = CreateJumpShadow(startPos);

            while (elapsed < jumpDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / jumpDuration;

                // Parabolic arc (sinüs dalgası)
                float heightOffset = Mathf.Sin(progress * Mathf.PI) * jumpHeight;
                
                // Havada hareket kontrolü
                Vector2 airInput = InputHandler.Instance.MovementInput;
                Vector2 airMovement = airInput.normalized * (moveSpeed * airControlMultiplier);
                rb.velocity = airMovement;

                // Y pozisyonu güncelle (fake height)
                Vector3 newPos = startPos + new Vector3(rb.velocity.x * Time.deltaTime, heightOffset, 0);
                transform.position = newPos;
                startPos = new Vector3(startPos.x + rb.velocity.x * Time.deltaTime, startPos.y, startPos.z);

                yield return null;
            }

            // Yere in
            transform.position = new Vector3(transform.position.x, startPos.y, transform.position.z);
            isGrounded = true;

            // Shadow'u temizle
            if (shadow != null)
            {
                Destroy(shadow);
            }
        }

        /// <summary>
        /// Zıplama sırasında zeminde gölge oluşturur.
        /// </summary>
        private GameObject CreateJumpShadow(Vector3 position)
        {
            GameObject shadow = new GameObject("JumpShadow");
            shadow.transform.position = position;
            
            SpriteRenderer shadowSR = shadow.AddComponent<SpriteRenderer>();
            shadowSR.color = new Color(0, 0, 0, 0.4f);
            shadowSR.sortingOrder = -10;
            
            // Basit bir oval shadow (daha sonra sprite ile değiştirilebilir)
            // Placeholder olarak circle sprite kullanılabilir
            
            return shadow;
        }

        /// <summary>
        /// Aktif coroutine'i durdurur (interrupt için).
        /// </summary>
        public void StopCurrentAction()
        {
            if (currentActionCoroutine != null)
            {
                StopCoroutine(currentActionCoroutine);
                currentActionCoroutine = null;
            }
        }

        /// <summary>
        /// State'den coroutine başlatmak için helper.
        /// </summary>
        public void StartActionCoroutine(IEnumerator coroutine)
        {
            StopCurrentAction();
            currentActionCoroutine = StartCoroutine(coroutine);
        }
        #endregion

        #region Properties
        public float StaminaCurrent => staminaCurrent;
        public float StaminaMax => staminaMax;
        public float RageCurrent => rageCurrent;
        public float RageMax => rageMax;
        public float RunSpeedMultiplier => runSpeedMultiplier;
        public PlayerStateMachine StateMachine => stateMachine;
        public PlayerCombat Combat => combat;
        #endregion
    }
}

