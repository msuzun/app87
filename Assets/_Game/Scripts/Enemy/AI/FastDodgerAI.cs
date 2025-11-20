using UnityEngine;
using System.Collections;
using NeonSyndicate.Core;

namespace NeonSyndicate.Enemy.AI
{
    /// <summary>
    /// FAST DODGER - Hızlı Kaçan
    /// Hit-and-run taktikleri, dodge spam, kite yapan düşman.
    /// Sinir bozucu ama yakalanınca vulnerable.
    /// </summary>
    public class FastDodgerAI : EnemyAIBase
    {
        [Header("Dodger Specific")]
        [SerializeField] private float preferredDistance = 6f;    // İdeal mesafe
        [SerializeField] private float dodgeChance = 0.4f;        // %40 dodge şansı
        [SerializeField] private float dashSpeed = 10f;           // Dash hızı
        [SerializeField] private float observeTime = 2f;          // Gözlem süresi

        [Header("States")]
        private bool isInvulnerable = false;
        private bool playerIsAttacking = false;
        private float dodgeCooldown = 0f;

        protected override void Start()
        {
            base.Start();
            ChangeState(AIState.Idle); // Observe state gibi
        }

        protected override void UpdateAI()
        {
            dodgeCooldown -= Time.deltaTime;

            // Oyuncu saldırı yapıyor mu? (Basit detection)
            DetectPlayerAttack();

            // Behavior Tree
            BehaviorTreeUpdate();

            // State Machine
            UpdateCurrentState();
        }

        #region Behavior Tree
        private void BehaviorTreeUpdate()
        {
            float distanceToTarget = GetDistanceToTarget();

            // Root Selector
            if (controller.IsDead)
            {
                if (currentState != AIState.Death)
                    ChangeState(AIState.Death);
                return;
            }

            // Player attacking? -> Dodge (40% chance)
            if (playerIsAttacking && dodgeCooldown <= 0 && Random.value < dodgeChance)
            {
                ChangeState(AIState.Stunned); // Stunned state'i dodge için kullanıyoruz
                return;
            }

            // Has Attack Token?
            if (hasAttackToken)
            {
                // In safe distance? -> Hit and Run
                if (distanceToTarget >= 2f && distanceToTarget <= attackRange + 2f && CanAttack())
                {
                    ChangeState(AIState.Attack);
                }
                // Player close? -> Backflip away
                else if (distanceToTarget < 2f)
                {
                    ChangeState(AIState.BackOff);
                }
                // Chase to get in range
                else
                {
                    if (currentState != AIState.Chase)
                        ChangeState(AIState.Chase);
                }
            }
            // No Token -> Keep Distance (Kite)
            else
            {
                // Too close? Run away
                if (distanceToTarget < preferredDistance - 1f)
                {
                    if (currentState != AIState.BackOff)
                        ChangeState(AIState.BackOff);
                }
                // Too far? Stay and observe
                else if (distanceToTarget > preferredDistance + 1f)
                {
                    if (currentState != AIState.Idle)
                        ChangeState(AIState.Idle); // Observe
                }
                // Good distance -> Circle strafe
                else
                {
                    if (currentState != AIState.CircleStrafe)
                        ChangeState(AIState.CircleStrafe);
                }
            }
        }

        private void DetectPlayerAttack()
        {
            // Basit detection: Oyuncu attack state'te mi?
            // (Gerçek üretimde player state'e bakılabilir)
            if (target != null)
            {
                // Placeholder logic
                playerIsAttacking = false; // TODO: Real implementation
            }
        }
        #endregion

        #region State Machine
        private void UpdateCurrentState()
        {
            switch (currentState)
            {
                case AIState.Idle: // Observe
                    UpdateObserveState();
                    break;

                case AIState.Chase:
                    UpdateChaseState();
                    break;

                case AIState.CircleStrafe:
                    UpdateCircleStrafeState();
                    break;

                case AIState.Attack:
                    // Coroutine kontrolünde
                    break;

                case AIState.BackOff: // Keep Distance / Backflip
                    UpdateKeepDistanceState();
                    break;

                case AIState.Stunned: // Dodge Roll
                    // Coroutine kontrolünde
                    break;

                case AIState.Hurt:
                    UpdateHurtState();
                    break;
            }
        }

        private void UpdateObserveState()
        {
            StopMovement();

            // Oyuncuyu izle (sprite flip)
            if (target != null)
            {
                float dirX = target.position.x - transform.position.x;
                spriteRenderer.flipX = dirX < 0;
            }

            // Observe süresi doldu mu?
            if (stateTimer > observeTime)
            {
                ChangeState(AIState.CircleStrafe);
            }
        }

        private void UpdateChaseState()
        {
            // Hızlı yaklaş (ama attack range'e girmeden dur)
            float distanceToTarget = GetDistanceToTarget();
            
            if (distanceToTarget > attackRange + 1f)
            {
                MoveTowardsTarget(controller.MoveSpeed);
            }
            else
            {
                // Yeterince yakın, idle'a geç
                ChangeState(AIState.Idle);
            }
        }

        private void UpdateCircleStrafeState()
        {
            // Hızlı circle strafe
            CircleStrafeTarget(controller.MoveSpeed * 0.8f);
        }

        private void UpdateKeepDistanceState()
        {
            // Oyuncudan kaç
            MoveAwayFromTarget(controller.MoveSpeed);

            // İdeal mesafeye ulaştı mı?
            if (GetDistanceToTarget() >= preferredDistance)
            {
                ChangeState(AIState.Idle); // Observe
            }
        }

        private void UpdateHurtState()
        {
            if (stateTimer > 0.3f)
            {
                ChangeState(AIState.BackOff); // Hasar aldıktan sonra uzaklaş
            }
        }
        #endregion

        #region State Enter/Exit
        protected override void OnStateEnter(AIState state)
        {
            base.OnStateEnter(state);

            switch (state)
            {
                case AIState.Attack:
                    StartBehaviorCoroutine(HitAndRunRoutine());
                    break;

                case AIState.Stunned: // Dodge Roll
                    StartBehaviorCoroutine(DodgeRollRoutine());
                    break;

                case AIState.BackOff:
                    // Backflip animation
                    animator.SetTrigger("Backflip");
                    break;

                case AIState.Hurt:
                    StopMovement();
                    animator.SetTrigger("Hurt");
                    break;

                case AIState.Death:
                    StopMovement();
                    animator.SetTrigger("Die");
                    OnDeath();
                    break;
            }
        }
        #endregion

        #region Attack Patterns
        /// <summary>
        /// Pattern 1: Hit-and-Run (Ana taktik)
        /// Dash in -> Quick strike -> Dash out
        /// </summary>
        private IEnumerator HitAndRunRoutine()
        {
            // Phase 1: Dash In (0.2s)
            Vector2 dashDirection = (target.position - transform.position).normalized;
            float dashDuration = 0.2f;
            float elapsed = 0f;

            animator.Play("Dash_In");
            while (elapsed < dashDuration)
            {
                rb.velocity = dashDirection * dashSpeed;
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Phase 2: Quick Strike (0.15s)
            StopMovement();
            animator.Play("Quick_Stab");
            yield return new WaitForSeconds(0.05f);
            ActivateAttackHitbox();
            yield return new WaitForSeconds(0.1f);

            // Phase 3: Dash Out (0.2s)
            dashDirection = -dashDirection;
            elapsed = 0f;
            isInvulnerable = true; // I-frame during dash out

            animator.Play("Dash_Out");
            while (elapsed < dashDuration)
            {
                rb.velocity = dashDirection * dashSpeed;
                elapsed += Time.deltaTime;
                yield return null;
            }

            isInvulnerable = false;
            StopMovement();

            // Return to observe
            ChangeState(AIState.Idle);
        }

        /// <summary>
        /// Pattern 2: Feint Attack
        /// Fake dash in -> punish player
        /// </summary>
        private IEnumerator FeintAttackRoutine()
        {
            // Fake dash in
            Vector2 dashDirection = (target.position - transform.position).normalized;
            
            animator.Play("Dash_In");
            rb.velocity = dashDirection * dashSpeed;
            yield return new WaitForSeconds(0.1f);

            // Stop abruptly
            StopMovement();
            animator.Play("Idle");

            // Wait for player reaction
            yield return new WaitForSeconds(0.3f);

            // Check if player whiffed an attack
            // If yes, punish with quick strike
            if (GetDistanceToTarget() < attackRange)
            {
                animator.Play("Quick_Stab");
                yield return new WaitForSeconds(0.05f);
                ActivateAttackHitbox();
            }

            yield return new WaitForSeconds(0.2f);
            ChangeState(AIState.BackOff);
        }

        /// <summary>
        /// Pattern 3: Desperation Combo (HP < 20%)
        /// Abandon hit-and-run, commit to combo
        /// </summary>
        private IEnumerator DesperationComboRoutine()
        {
            // Dash in
            Vector2 dashDirection = (target.position - transform.position).normalized;
            rb.velocity = dashDirection * dashSpeed;
            yield return new WaitForSeconds(0.2f);

            StopMovement();

            // Multi-hit combo (3 hits)
            for (int i = 0; i < 3; i++)
            {
                animator.Play("Quick_Stab");
                yield return new WaitForSeconds(0.1f);
                ActivateAttackHitbox();
                yield return new WaitForSeconds(0.2f);
            }

            // No dash out (committed)
            yield return new WaitForSeconds(0.3f);
            ChangeState(AIState.BackOff);
        }

        /// <summary>
        /// Dodge Roll (I-frame dodge)
        /// </summary>
        private IEnumerator DodgeRollRoutine()
        {
            dodgeCooldown = 2f; // Cooldown

            // Dodge direction (away from player)
            Vector2 dodgeDirection = (transform.position - target.position).normalized;

            isInvulnerable = true;
            animator.Play("Dodge_Roll");

            float dodgeDuration = 0.3f;
            float elapsed = 0f;

            while (elapsed < dodgeDuration)
            {
                rb.velocity = dodgeDirection * dashSpeed;
                elapsed += Time.deltaTime;
                yield return null;
            }

            isInvulnerable = false;
            StopMovement();

            // Return to observe
            ChangeState(AIState.Idle);
        }
        #endregion

        #region Combat Callbacks
        public override void OnDamageReceived()
        {
            base.OnDamageReceived();

            // Hasar alınca çok savunmacı ol
            if (currentState != AIState.Hurt)
            {
                ChangeState(AIState.Hurt);
            }

            // Düşük can -> desperation mode
            if (controller.CurrentHealth < controller.MaxHealth * 0.2f)
            {
                aggressiveness = 1f;
                dodgeChance = 0.6f; // Daha çok dodge
            }
        }

        public override void OnDeath()
        {
            base.OnDeath();
            isInvulnerable = false;
        }
        #endregion

        #region Utility
        /// <summary>
        /// I-frame kontrolü için IDamageable'a ek.
        /// </summary>
        public bool IsInvulnerable => isInvulnerable;
        #endregion
    }
}

