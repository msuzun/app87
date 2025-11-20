using UnityEngine;
using System.Collections;
using NeonSyndicate.StateMachine;
using NeonSyndicate.Core;

namespace NeonSyndicate.Player
{
    /// <summary>
    /// Oyuncu dodge (kaçınma) yaparken state.
    /// HYBRID: Coroutine bazlı dash sistemi.
    /// İ-Frame (invulnerability) sağlar.
    /// </summary>
    public class PlayerDodgeState : StateBase
    {
        private PlayerStateMachine playerSM;
        private bool dodgeComplete = false;

        public PlayerDodgeState(StateMachineController stateMachine) : base(stateMachine)
        {
            playerSM = stateMachine as PlayerStateMachine;
        }

        public override void Enter()
        {
            dodgeComplete = false;

            // Stamina kontrolü (PlayerController'da yapılıyor)
            // Animasyon
            playerSM.Animator.SetTrigger("Dodge");

            // Ses efekti
            SoundManager.Instance?.PlaySFX("Dodge_Whoosh");

            // Coroutine başlat
            playerSM.Controller.StartActionCoroutine(DashWithCallback());
        }

        public override void Update()
        {
            // Dash tamamlandığında Idle'a dön
            if (dodgeComplete)
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
            playerSM.Rb.velocity = Vector2.zero;
        }

        /// <summary>
        /// Dash coroutine'i callback ile sarmalayan wrapper.
        /// </summary>
        private IEnumerator DashWithCallback()
        {
            yield return playerSM.Controller.DashCoroutine();
            dodgeComplete = true;
        }
    }
}

