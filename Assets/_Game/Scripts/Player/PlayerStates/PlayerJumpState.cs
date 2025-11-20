using UnityEngine;
using System.Collections;
using NeonSyndicate.StateMachine;
using NeonSyndicate.Core;

namespace NeonSyndicate.Player
{
    /// <summary>
    /// Oyuncu zıpladığında (Jump) state.
    /// HYBRID: Coroutine bazlı fake height jump sistemi.
    /// Crazy Flasher'daki "havada kombo" mekanizması için temel.
    /// </summary>
    public class PlayerJumpState : StateBase
    {
        private PlayerStateMachine playerSM;
        private bool jumpComplete = false;

        public PlayerJumpState(StateMachineController stateMachine) : base(stateMachine)
        {
            playerSM = stateMachine as PlayerStateMachine;
        }

        public override void Enter()
        {
            jumpComplete = false;

            // Zıplama animasyonu
            playerSM.Animator.SetTrigger("Jump");
            
            // Ses efekti
            SoundManager.Instance?.PlaySFX("Jump");

            // Coroutine başlat (PlayerController'dan)
            playerSM.Controller.StartActionCoroutine(JumpWithCallback());
        }

        public override void Update()
        {
            // Havadayken saldırı yapabilme (Air combo)
            if (InputHandler.Instance.IsAttackPressed)
            {
                playerSM.Controller.StopCurrentAction();
                playerSM.ChangeState(playerSM.AttackState);
                return;
            }

            // Coroutine tamamlandığında Idle'a dön
            if (jumpComplete)
            {
                playerSM.ChangeState(playerSM.IdleState);
            }
        }

        public override void FixedUpdate()
        {
            // Fizik PlayerController'daki coroutine tarafından yönetiliyor
        }

        public override void Exit()
        {
            playerSM.Controller.isGrounded = true;
        }

        /// <summary>
        /// Jump coroutine'i callback ile sarmalayan wrapper.
        /// </summary>
        private IEnumerator JumpWithCallback()
        {
            yield return playerSM.Controller.JumpCoroutine();
            jumpComplete = true;
        }
    }
}

