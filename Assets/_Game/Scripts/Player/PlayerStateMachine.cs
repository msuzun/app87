using UnityEngine;
using NeonSyndicate.StateMachine;
using NeonSyndicate.Characters;

namespace NeonSyndicate.Player
{
    /// <summary>
    /// Oyuncuya özel State Machine kontrolcüsü.
    /// Oyuncu state'lerini yönetir.
    /// </summary>
    public class PlayerStateMachine : StateMachineController
    {
        [Header("Player States")]
        public PlayerIdleState IdleState { get; private set; }
        public PlayerWalkState WalkState { get; private set; }
        public PlayerJumpState JumpState { get; private set; }
        public PlayerAttackState AttackState { get; private set; }
        public PlayerHurtState HurtState { get; private set; }
        public PlayerDeathState DeathState { get; private set; }
        public PlayerDodgeState DodgeState { get; private set; }

        [Header("Player Components")]
        public PlayerController Controller { get; private set; }
        public PlayerCombat Combat { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            Controller = GetComponent<PlayerController>();
            Combat = GetComponent<PlayerCombat>();

            // State'leri initialize et
            IdleState = new PlayerIdleState(this);
            WalkState = new PlayerWalkState(this);
            JumpState = new PlayerJumpState(this);
            AttackState = new PlayerAttackState(this);
            HurtState = new PlayerHurtState(this);
            DeathState = new PlayerDeathState(this);
            DodgeState = new PlayerDodgeState(this);
        }

        private void Start()
        {
            // Oyun başlangıcında Idle state'e geç
            ChangeState(IdleState);
        }
    }
}

