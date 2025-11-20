using UnityEngine;
using NeonSyndicate.StateMachine;
using NeonSyndicate.Core;

namespace NeonSyndicate.Player
{
    /// <summary>
    /// Oyuncu hareketsiz durduğunda (Idle) state.
    /// </summary>
    public class PlayerIdleState : StateBase
    {
        private PlayerStateMachine playerSM;

        public PlayerIdleState(StateMachineController stateMachine) : base(stateMachine)
        {
            playerSM = stateMachine as PlayerStateMachine;
        }

        public override void Enter()
        {
            playerSM.Animator.SetBool("IsWalking", false);
            playerSM.Rb.velocity = Vector2.zero;
        }

        public override void Update()
        {
            // Input kontrolü
            Vector2 input = InputHandler.Instance.MovementInput;

            // Hareket varsa Walk state'e geç
            if (input.magnitude > 0.1f)
            {
                playerSM.ChangeState(playerSM.WalkState);
            }

            // Saldırı tuşuna basıldıysa
            if (InputHandler.Instance.IsAttackPressed)
            {
                playerSM.ChangeState(playerSM.AttackState);
            }

            // Zıplama
            if (InputHandler.Instance.IsJumpPressed)
            {
                playerSM.ChangeState(playerSM.JumpState);
            }

            // Dodge
            if (InputHandler.Instance.IsDodgePressed)
            {
                playerSM.ChangeState(playerSM.DodgeState);
            }
        }

        public override void FixedUpdate()
        {
            // Idle'da fizik güncelleme gerekmez
        }

        public override void Exit()
        {
            // Temizlik işlemleri
        }
    }
}

