using UnityEngine;
using UnityEngine.InputSystem;

namespace NeonSyndicate.Core
{
    /// <summary>
    /// Unity NEW Input System ile entegre çalışan input yöneticisi.
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
            // NEW INPUT SYSTEM kullanımı
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;

            if (keyboard == null) return; // Input device yoksa çık

            // Movement (WASD + Arrow Keys)
            Vector2 moveInput = Vector2.zero;
            
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) moveInput.y += 1;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) moveInput.y -= 1;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) moveInput.x -= 1;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) moveInput.x += 1;
            
            MovementInput = moveInput.normalized;

            // Attack buttons (Z veya Sol Mouse)
            IsAttackPressed = keyboard.zKey.wasPressedThisFrame || 
                             (mouse != null && mouse.leftButton.wasPressedThisFrame);
            
            // Heavy attack (X veya Sağ Mouse)
            IsHeavyAttackPressed = keyboard.xKey.wasPressedThisFrame || 
                                  (mouse != null && mouse.rightButton.wasPressedThisFrame);
            
            // Jump (Space)
            IsJumpPressed = keyboard.spaceKey.wasPressedThisFrame;
            
            // Dodge/Dash (Shift tap)
            IsDodgePressed = keyboard.leftShiftKey.wasPressedThisFrame || 
                            keyboard.rightShiftKey.wasPressedThisFrame;
            
            // Grab (C)
            IsGrabPressed = keyboard.cKey.wasPressedThisFrame;
            
            // Run/Sprint (Shift hold)
            IsRunPressed = keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed;
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

