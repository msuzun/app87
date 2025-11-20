using UnityEngine;
using NeonSyndicate.StateMachine;
using NeonSyndicate.Core;

namespace NeonSyndicate.Player
{
    /// <summary>
    /// Oyuncu saldırı yaparken (Attack) state.
    /// Combo sistemi burada işlenir.
    /// </summary>
    public class PlayerAttackState : StateBase
    {
        private PlayerStateMachine playerSM;
        private float attackTimer;
        private bool attackComplete;

        public PlayerAttackState(StateMachineController stateMachine) : base(stateMachine)
        {
            playerSM = stateMachine as PlayerStateMachine;
        }

        public override void Enter()
        {
            attackTimer = 0f;
            attackComplete = false;

            // Hareketi durdur
            playerSM.Rb.velocity = Vector2.zero;

            // Combo sistemini tetikle
            playerSM.Combat.ExecuteNextCombo();

            // Ses efekti
            SoundManager.Instance?.PlaySFX("Whoosh_Attack");
        }

        public override void Update()
        {
            attackTimer += Time.deltaTime;

            // Animasyon bittiğinde (örn: 0.5 saniye)
            if (attackTimer >= 0.5f)
            {
                attackComplete = true;
            }

            // Combo penceresi içinde tekrar saldırı tuşuna basıldıysa
            if (InputHandler.Instance.IsAttackPressed && playerSM.Combat.CanContinueCombo())
            {
                // Aynı state'te kal ama combonun sonraki adımına geç
                Enter(); // State'i resetle
            }

            // Saldırı tamamlandıysa Idle'a dön
            if (attackComplete)
            {
                playerSM.ChangeState(playerSM.IdleState);
            }
        }

        public override void FixedUpdate()
        {
            // Saldırı sırasında hafif ileri hareket (momentum)
            Vector2 attackDirection = playerSM.SpriteRenderer.flipX ? Vector2.left : Vector2.right;
            playerSM.Rb.velocity = attackDirection * 2f;
        }

        public override void Exit()
        {
            playerSM.Animator.SetBool("IsAttacking", false);
        }
    }
}

