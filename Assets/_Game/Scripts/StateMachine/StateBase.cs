using UnityEngine;

namespace NeonSyndicate.StateMachine
{
    /// <summary>
    /// Tüm State'lerin türediği base class.
    /// Finite State Machine (FSM) yapısının temelini oluşturur.
    /// </summary>
    public abstract class StateBase
    {
        protected StateMachineController stateMachine;

        public StateBase(StateMachineController stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        /// <summary>
        /// State'e girildiğinde bir kez çağrılır.
        /// </summary>
        public abstract void Enter();

        /// <summary>
        /// State aktifken her frame çağrılır.
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// State aktifken her fixed frame çağrılır (fizik için).
        /// </summary>
        public abstract void FixedUpdate();

        /// <summary>
        /// State'ten çıkarken bir kez çağrılır.
        /// </summary>
        public abstract void Exit();
    }
}

