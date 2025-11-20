using UnityEngine;
using NeonSyndicate.StateMachine;
using NeonSyndicate.Core;

namespace NeonSyndicate.Player
{
    /// <summary>
    /// Oyuncu dodge (kaçınma) yaparken state.
    /// İ-Frame (invulnerability) sağlar.
    /// </summary>
    public class PlayerDodgeState : StateBase
    {
        private PlayerStateMachine playerSM;
        private float dodgeDuration = 0.3f;
        private float dodgeSpeed = 15f;
        private float dodgeTimer;
        private Vector2 dodgeDirection;

        public PlayerDodgeState(StateMachineController stateMachine) : base(stateMachine)
        {
            playerSM = stateMachine as PlayerStateMachine;
        }

        public override void Enter()
        {
            dodgeTimer = 0f;

            // Dodge yönünü belirle (input varsa ona göre, yoksa baktığı yöne)
            Vector2 input = InputHandler.Instance.MovementInput;
            if (input.magnitude > 0.1f)
            {
                dodgeDirection = input.normalized;
            }
            else
            {
                dodgeDirection = playerSM.SpriteRenderer.flipX ? Vector2.left : Vector2.right;
            }

            // I-Frame aktif et
            playerSM.Controller.SetInvulnerable(dodgeDuration);

            // Animasyon
            playerSM.Animator.SetTrigger("Dodge");

            // Ses efekti
            SoundManager.Instance?.PlaySFX("Dodge_Whoosh");
        }

        public override void Update()
        {
            dodgeTimer += Time.deltaTime;

            if (dodgeTimer >= dodgeDuration)
            {
                playerSM.ChangeState(playerSM.IdleState);
            }
        }

        public override void FixedUpdate()
        {
            // Hızlı ileri hareket (dash)
            playerSM.Rb.velocity = dodgeDirection * dodgeSpeed;
        }

        public override void Exit()
        {
            playerSM.Rb.velocity = Vector2.zero;
        }
    }
}

