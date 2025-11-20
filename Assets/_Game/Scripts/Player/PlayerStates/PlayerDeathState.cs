using UnityEngine;
using NeonSyndicate.StateMachine;
using NeonSyndicate.Core;

namespace NeonSyndicate.Player
{
    /// <summary>
    /// Oyuncu öldüğünde (Death) state.
    /// Ragdoll fiziği burada aktifleşir.
    /// </summary>
    public class PlayerDeathState : StateBase
    {
        private PlayerStateMachine playerSM;

        public PlayerDeathState(StateMachineController stateMachine) : base(stateMachine)
        {
            playerSM = stateMachine as PlayerStateMachine;
        }

        public override void Enter()
        {
            // Animator'ı durdur
            playerSM.Animator.enabled = false;

            // Ragdoll'u aktifleştir
            // (Ragdoll controller varsa)
            RagdollController ragdoll = playerSM.GetComponent<RagdollController>();
            if (ragdoll != null)
            {
                ragdoll.ActivateRagdoll();
            }

            // Game Over ekranını 2 saniye sonra göster
            GameManager.Instance.Invoke("GameOver", 2f);

            // Müziği durdur
            SoundManager.Instance?.StopMusic(1f);
            SoundManager.Instance?.PlaySFX("Player_Death");
        }

        public override void Update()
        {
            // Ölü state'te güncelleme yok
        }

        public override void FixedUpdate()
        {
            // Fizik ragdoll tarafından yönetilir
        }

        public override void Exit()
        {
            // Death state'ten çıkış genelde olmaz (restart gerekir)
        }
    }
}

