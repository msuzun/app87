using UnityEngine;
using NeonSyndicate.StateMachine;

namespace NeonSyndicate.Player
{
    /// <summary>
    /// Oyuncu zıpladığında (Jump) state.
    /// Crazy Flasher'daki "havada kombo" mekanizması için temel.
    /// </summary>
    public class PlayerJumpState : StateBase
    {
        private PlayerStateMachine playerSM;
        private float jumpHeight = 2f;
        private float jumpDuration = 0.6f;
        private float jumpTimer;
        private Vector3 startPosition;
        private GameObject shadowObject;

        public PlayerJumpState(StateMachineController stateMachine) : base(stateMachine)
        {
            playerSM = stateMachine as PlayerStateMachine;
        }

        public override void Enter()
        {
            jumpTimer = 0f;
            startPosition = playerSM.transform.position;

            // Zıplama animasyonu
            playerSM.Animator.SetTrigger("Jump");

            // Gölgeyi yerinde tut (shadow prefab'ı varsa)
            CreateShadow();
        }

        public override void Update()
        {
            jumpTimer += Time.deltaTime;

            // Parabolic jump (yukarı çık, sonra aşağı in)
            float progress = jumpTimer / jumpDuration;
            float height = Mathf.Sin(progress * Mathf.PI) * jumpHeight;

            Vector3 newPos = startPosition + Vector3.up * height;
            playerSM.transform.position = newPos;

            // Havadayken saldırı yapabilme
            if (InputHandler.Instance.IsAttackPressed)
            {
                playerSM.ChangeState(playerSM.AttackState);
            }

            // Zıplama tamamlandı
            if (jumpTimer >= jumpDuration)
            {
                playerSM.ChangeState(playerSM.IdleState);
            }
        }

        public override void FixedUpdate()
        {
            // Havadayken hafif hareket edebilme
            Vector2 input = InputHandler.Instance.MovementInput;
            playerSM.Rb.velocity = input.normalized * (playerSM.Controller.MoveSpeed * 0.5f);
        }

        public override void Exit()
        {
            // Gölgeyi temizle
            if (shadowObject != null)
            {
                Object.Destroy(shadowObject);
            }

            playerSM.transform.position = new Vector3(
                playerSM.transform.position.x,
                startPosition.y,
                playerSM.transform.position.z
            );
        }

        private void CreateShadow()
        {
            // Basit bir gölge oluştur (placeholder)
            // Gerçek üretimde shadow sprite prefab kullanılmalı
            shadowObject = new GameObject("Shadow");
            shadowObject.transform.position = startPosition;
            
            SpriteRenderer shadowSR = shadowObject.AddComponent<SpriteRenderer>();
            shadowSR.color = new Color(0, 0, 0, 0.5f);
            shadowSR.sortingOrder = -1;
        }
    }
}

