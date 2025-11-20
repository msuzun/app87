using UnityEngine;
using NeonSyndicate.StateMachine;

namespace NeonSyndicate.Player
{
    /// <summary>
    /// Oyuncu hasar aldığında (Hurt) state.
    /// Kısa süreli stun (sersemleme) sağlar.
    /// </summary>
    public class PlayerHurtState : StateBase
    {
        private PlayerStateMachine playerSM;
        private float hurtDuration = 0.4f;
        private float hurtTimer;

        public PlayerHurtState(StateMachineController stateMachine) : base(stateMachine)
        {
            playerSM = stateMachine as PlayerStateMachine;
        }

        public override void Enter()
        {
            hurtTimer = 0f;

            // Hareketi durdur
            playerSM.Rb.velocity = Vector2.zero;

            // Animasyon
            playerSM.Animator.SetTrigger("Hit");
        }

        public override void Update()
        {
            hurtTimer += Time.deltaTime;

            if (hurtTimer >= hurtDuration)
            {
                playerSM.ChangeState(playerSM.IdleState);
            }
        }

        public override void FixedUpdate()
        {
            // Hurt state'te hareket etme
        }

        public override void Exit()
        {
            // Temizlik
        }
    }
}

