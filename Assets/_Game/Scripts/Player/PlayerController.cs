using UnityEngine;
using NeonSyndicate.Characters;
using NeonSyndicate.Core;

namespace NeonSyndicate.Player
{
    /// <summary>
    /// Oyuncu karakteri (Axel) için ana kontrolcü.
    /// CharacterBase'den türer ve oyuncuya özel özellikleri içerir.
    /// </summary>
    [RequireComponent(typeof(PlayerStateMachine))]
    [RequireComponent(typeof(PlayerCombat))]
    public class PlayerController : CharacterBase
    {
        [Header("Player Specific Stats")]
        [SerializeField] private float staminaMax = 100f;
        [SerializeField] private float staminaCurrent;
        [SerializeField] private float staminaRegenRate = 10f;

        [Header("Rage System")]
        [SerializeField] private float rageMax = 100f;
        [SerializeField] private float rageCurrent = 0f;
        [SerializeField] private float rageGainPerHit = 5f;

        [Header("Components")]
        private PlayerStateMachine stateMachine;
        private PlayerCombat combat;

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

        #region Properties
        public float StaminaCurrent => staminaCurrent;
        public float StaminaMax => staminaMax;
        public float RageCurrent => rageCurrent;
        public float RageMax => rageMax;
        public PlayerStateMachine StateMachine => stateMachine;
        #endregion
    }
}

