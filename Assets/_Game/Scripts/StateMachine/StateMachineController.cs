using UnityEngine;

namespace NeonSyndicate.StateMachine
{
    /// <summary>
    /// State Machine'in ana kontrolcüsü.
    /// Karakter davranışlarını yöneten Finite State Machine (FSM).
    /// </summary>
    public class StateMachineController : MonoBehaviour
    {
        public StateBase CurrentState { get; private set; }
        public StateBase PreviousState { get; private set; }

        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        [SerializeField] private string currentStateName;

        // Components (alt sınıflar erişebilsin diye public)
        public Rigidbody2D Rb { get; private set; }
        public Animator Animator { get; private set; }
        public SpriteRenderer SpriteRenderer { get; private set; }

        protected virtual void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected virtual void Update()
        {
            CurrentState?.Update();

            if (showDebugInfo)
            {
                currentStateName = CurrentState?.GetType().Name ?? "None";
            }
        }

        protected virtual void FixedUpdate()
        {
            CurrentState?.FixedUpdate();
        }

        /// <summary>
        /// Yeni bir state'e geçiş yapar.
        /// </summary>
        public void ChangeState(StateBase newState)
        {
            if (CurrentState == newState) return;

            CurrentState?.Exit();
            PreviousState = CurrentState;
            CurrentState = newState;
            CurrentState?.Enter();

            if (showDebugInfo)
            {
                Debug.Log($"State changed: {PreviousState?.GetType().Name} -> {CurrentState?.GetType().Name}");
            }
        }

        /// <summary>
        /// Önceki state'e geri döner.
        /// </summary>
        public void RevertToPreviousState()
        {
            if (PreviousState != null)
            {
                ChangeState(PreviousState);
            }
        }
    }
}

