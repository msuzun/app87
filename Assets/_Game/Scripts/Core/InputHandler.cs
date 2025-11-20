using UnityEngine;

namespace NeonSyndicate.Core
{
    /// <summary>
    /// Unity Input System ile entegre çalışan input yöneticisi.
    /// Tuş kombinasyonlarını (combo) algılayıp event fırlatır.
    /// </summary>
    public class InputHandler : MonoBehaviour
    {
        public static InputHandler Instance { get; private set; }

        [Header("Input States")]
        public Vector2 MovementInput { get; private set; }
        public bool IsAttackPressed { get; private set; }
        public bool IsHeavyAttackPressed { get; private set; }
        public bool IsJumpPressed { get; private set; }
        public bool IsDodgePressed { get; private set; }
        public bool IsGrabPressed { get; private set; }
        public bool IsRunPressed { get; private set; }

        [Header("Combo Detection")]
        [SerializeField] private float comboWindowTime = 0.5f;
        private float lastAttackTime;

        // Events for combat system
        public delegate void InputAction();
        public static event InputAction OnLightAttack;
        public static event InputAction OnHeavyAttack;
        public static event InputAction OnJump;
        public static event InputAction OnDodge;
        public static event InputAction OnGrab;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            // Oyun duraklıysa input almayı engelle
            if (GameManager.Instance != null && GameManager.Instance.IsPaused)
            {
                ResetInputs();
                return;
            }

            ReadInputs();
            ProcessInputs();
        }

        private void ReadInputs()
        {
            // Movement (WASD + Arrow Keys)
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            MovementInput = new Vector2(horizontal, vertical);

            // Attack buttons
            IsAttackPressed = Input.GetButtonDown("Fire1"); // J veya Sol Mouse
            IsHeavyAttackPressed = Input.GetButtonDown("Fire2"); // K veya Sağ Mouse
            
            // Jump
            IsJumpPressed = Input.GetButtonDown("Jump"); // Space
            
            // Dodge/Dash
            IsDodgePressed = Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
            
            // Grab
            IsGrabPressed = Input.GetKeyDown(KeyCode.E);
            
            // Run/Sprint
            IsRunPressed = Input.GetKey(KeyCode.LeftShift);
        }

        private void ProcessInputs()
        {
            // Event fırlatma
            if (IsAttackPressed)
            {
                OnLightAttack?.Invoke();
                lastAttackTime = Time.time;
            }

            if (IsHeavyAttackPressed)
            {
                OnHeavyAttack?.Invoke();
            }

            if (IsJumpPressed)
            {
                OnJump?.Invoke();
            }

            if (IsDodgePressed)
            {
                OnDodge?.Invoke();
            }

            if (IsGrabPressed)
            {
                OnGrab?.Invoke();
            }
        }

        private void ResetInputs()
        {
            MovementInput = Vector2.zero;
            IsAttackPressed = false;
            IsHeavyAttackPressed = false;
            IsJumpPressed = false;
            IsDodgePressed = false;
            IsGrabPressed = false;
        }

        /// <summary>
        /// Combo sistemi için son saldırıdan bu yana geçen süreyi döner.
        /// </summary>
        public bool IsWithinComboWindow()
        {
            return Time.time - lastAttackTime <= comboWindowTime;
        }

        public void ResetComboWindow()
        {
            lastAttackTime = 0f;
        }
    }
}

