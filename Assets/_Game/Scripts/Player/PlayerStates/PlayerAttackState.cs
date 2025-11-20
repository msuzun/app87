using UnityEngine;
using NeonSyndicate.StateMachine;
using NeonSyndicate.Core;

namespace NeonSyndicate.Player
{
    /// <summary>
    /// Oyuncu saldırı yaparken (Attack) state.
    /// HYBRID: Responsive combo sistemi.
    /// Air attack desteği ile Crazy Flasher tarzı havada dövüş.
    /// </summary>
    public class PlayerAttackState : StateBase
    {
        private PlayerStateMachine playerSM;
        private float attackTimer;
        private bool attackComplete;
        private float attackDuration = 0.4f; // Her saldırının süresi

        public PlayerAttackState(StateMachineController stateMachine) : base(stateMachine)
        {
            playerSM = stateMachine as PlayerStateMachine;
        }

        public override void Enter()
        {
            attackTimer = 0f;
            attackComplete = false;

            // Hareketi durdur (yer saldırısı için)
            if (playerSM.Controller.isGrounded)
            {
                playerSM.Rb.velocity = Vector2.zero;
            }

            // Combo sistemini tetikle
            playerSM.Combat.ExecuteNextCombo();
            
            // Animator'a bildir
            playerSM.Animator.SetBool("IsAttacking", true);
        }

        public override void Update()
        {
            attackTimer += Time.deltaTime;

            // Animasyon bitme süresi (Animation Event ile daha hassas yapılabilir)
            if (attackTimer >= attackDuration)
            {
                attackComplete = true;
            }

            // COMBO WINDOW: Belirli bir süre içinde tekrar tuşa basılırsa combo devam eder
            if (InputHandler.Instance.IsAttackPressed && playerSM.Combat.CanContinueCombo())
            {
                // Combo'nun sonraki adımına geç
                Enter(); // State'i resetle
                return;
            }

            // Heavy attack ile launcher yapılabilir (havaya kaldırma)
            if (InputHandler.Instance.IsHeavyAttackPressed && playerSM.Combat.CanContinueCombo())
            {
                ExecuteHeavyAttack();
                return;
            }

            // Havadayken zıplama ile combo uzatabilme (Juggle)
            if (!playerSM.Controller.isGrounded && InputHandler.Instance.IsJumpPressed)
            {
                playerSM.ChangeState(playerSM.JumpState);
                return;
            }

            // Saldırı tamamlandıysa Idle veya Walk'a dön
            if (attackComplete)
            {
                // Input varsa direkt Walk'a geç (responsive)
                if (InputHandler.Instance.MovementInput.magnitude > 0.1f)
                {
                    playerSM.ChangeState(playerSM.WalkState);
                }
                else
                {
                    playerSM.ChangeState(playerSM.IdleState);
                }
            }
        }

        public override void FixedUpdate()
        {
            // Saldırı sırasında hafif ileri hareket (momentum) - Combat tarafından yapılıyor
            // Havadayken momentum korunur
        }

        public override void Exit()
        {
            playerSM.Animator.SetBool("IsAttacking", false);
            
            // Hitbox'ları temizle (güvenlik)
            playerSM.Combat.DeactivateHitboxes();
        }

        /// <summary>
        /// Heavy attack (Launcher) çalıştırır.
        /// </summary>
        private void ExecuteHeavyAttack()
        {
            attackTimer = 0f;
            attackComplete = false;
            
            playerSM.Animator.Play("Attack_Heavy");
            SoundManager.Instance?.PlaySFX("Heavy_Attack");
            
            Debug.Log("Heavy Attack (Launcher)!");
        }
    }
}

