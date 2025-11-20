using UnityEngine;
using NeonSyndicate.StateMachine;
using NeonSyndicate.Core;

namespace NeonSyndicate.Player
{
    /// <summary>
    /// Oyuncu hareket halindeyken (Walk) state.
    /// 2.5D hareket (X ve Y ekseni) burada işlenir.
    /// </summary>
    public class PlayerWalkState : StateBase
    {
        private PlayerStateMachine playerSM;

        public PlayerWalkState(StateMachineController stateMachine) : base(stateMachine)
        {
            playerSM = stateMachine as PlayerStateMachine;
        }

        public override void Enter()
        {
            playerSM.Animator.SetBool("IsWalking", true);
        }

        public override void Update()
        {
            Vector2 input = InputHandler.Instance.MovementInput;

            // Hareket yoksa Idle'a dön
            if (input.magnitude < 0.1f)
            {
                playerSM.ChangeState(playerSM.IdleState);
                return;
            }

            // Saldırı
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
            Vector2 input = InputHandler.Instance.MovementInput;
            
            // Run/Sprint sistemi
            float currentSpeed = playerSM.Controller.MoveSpeed;
            if (playerSM.Controller.isRunning)
            {
                currentSpeed *= playerSM.Controller.RunSpeedMultiplier;
            }
            
            // 2.5D hareket (X = sağ/sol, Y = derinlik)
            Vector2 movement = input.normalized * currentSpeed;
            playerSM.Rb.velocity = movement;

            // Animator parametrelerini güncelle
            playerSM.Animator.SetBool("IsRunning", playerSM.Controller.isRunning);
            playerSM.Animator.SetFloat("Speed", input.magnitude);
        }

        public override void Exit()
        {
            playerSM.Animator.SetBool("IsWalking", false);
        }
    }
}

