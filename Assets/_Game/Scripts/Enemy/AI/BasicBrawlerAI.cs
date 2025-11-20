using UnityEngine;
using System.Collections;
using NeonSyndicate.Core;

namespace NeonSyndicate.Enemy.AI
{
    /// <summary>
    /// BASIC BRAWLER - Sokak Kavgacısı
    /// Temel düşman, kalabalık halinde tehlikeli.
    /// Basit saldırı patternleri, öngörülebilir davranış.
    /// </summary>
    public class BasicBrawlerAI : EnemyAIBase
    {
        [Header("Brawler Specific")]
        [SerializeField] private float patrolRadius = 5f;
        [SerializeField] private float backOffDistance = 2f;
        [SerializeField] private float comboChance = 0.7f; // Front combo şansı

        private Vector3 patrolOrigin;
        private Vector3 patrolTarget;

        protected override void Start()
        {
            base.Start();
            patrolOrigin = transform.position;
            ChangeState(AIState.Idle);
        }

        protected override void UpdateAI()
        {
            // Davranış Ağacı (Behavior Tree) Logic
            BehaviorTreeUpdate();

            // State Machine Update
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

            // Has Attack Token?
            if (hasAttackToken)
            {
                // In Attack Range?
                if (distanceToTarget <= attackRange && CanAttack())
                {
                    ChangeState(AIState.Attack);
                }
                // Not in range -> Chase
                else if (distanceToTarget > attackRange)
                {
                    if (currentState != AIState.Chase)
                        ChangeState(AIState.Chase);
                }
            }
            // No Token
            else
            {
                // Too close? Back off
                if (distanceToTarget < backOffDistance)
                {
                    if (currentState != AIState.BackOff)
                        ChangeState(AIState.BackOff);
                }
                // Normal distance -> Circle strafe
                else if (distanceToTarget <= detectionRange)
                {
                    if (currentState != AIState.CircleStrafe)
                        ChangeState(AIState.CircleStrafe);
                }
                // Too far -> Patrol or Idle
                else
                {
                    if (currentState == AIState.Chase || currentState == AIState.CircleStrafe)
                        ChangeState(AIState.Idle);
                }
            }
        }
        #endregion

        #region State Machine
        private void UpdateCurrentState()
        {
            switch (currentState)
            {
                case AIState.Idle:
                    UpdateIdleState();
                    break;

                case AIState.Patrol:
                    UpdatePatrolState();
                    break;

                case AIState.Chase:
                    UpdateChaseState();
                    break;

                case AIState.CircleStrafe:
                    UpdateCircleStrafeState();
                    break;

                case AIState.Attack:
                    // Attack coroutine kontrolünde
                    break;

                case AIState.BackOff:
                    UpdateBackOffState();
                    break;

                case AIState.Hurt:
                    UpdateHurtState();
                    break;
            }
        }

        private void UpdateIdleState()
        {
            StopMovement();

            // Random süre sonra patrol'e geç
            if (stateTimer > Random.Range(0.5f, 1.5f))
            {
                ChangeState(AIState.Patrol);
            }

            // Oyuncu tespit edilirse chase
            if (IsTargetInRange(detectionRange))
            {
                ChangeState(AIState.Chase);
            }
        }

        private void UpdatePatrolState()
        {
            // Patrol hedefine git
            if (Vector3.Distance(transform.position, patrolTarget) < 0.5f)
            {
                ChangeState(AIState.Idle);
                return;
            }

            Vector2 direction = (patrolTarget - transform.position).normalized;
            rb.velocity = direction * (controller.MoveSpeed * 0.5f);

            animator.SetBool("IsWalking", true);

            // Oyuncu tespit edilirse chase
            if (IsTargetInRange(detectionRange))
            {
                ChangeState(AIState.Chase);
            }
        }

        private void UpdateChaseState()
        {
            MoveTowardsTarget(controller.MoveSpeed);

            // Attack range'e girince ve token varsa saldır
            if (CanAttack())
            {
                ChangeState(AIState.Attack);
            }
        }

        private void UpdateCircleStrafeState()
        {
            CircleStrafeTarget(controller.MoveSpeed * 0.6f);
        }

        private void UpdateBackOffState()
        {
            MoveAwayFromTarget(controller.MoveSpeed * 0.7f);

            // Güvenli mesafeye ulaştı mı?
            if (GetDistanceToTarget() >= backOffDistance + 1f)
            {
                ChangeState(AIState.CircleStrafe);
            }
        }

        private void UpdateHurtState()
        {
            // Hurt süresi bitti mi?
            if (stateTimer > 0.4f)
            {
                ChangeState(AIState.Chase);
            }
        }
        #endregion

        #region State Enter/Exit
        protected override void OnStateEnter(AIState state)
        {
            base.OnStateEnter(state);

            switch (state)
            {
                case AIState.Patrol:
                    // Random patrol hedefi belirle
                    Vector2 randomDir = Random.insideUnitCircle * patrolRadius;
                    patrolTarget = patrolOrigin + new Vector3(randomDir.x, randomDir.y, 0);
                    break;

                case AIState.Attack:
                    StartBehaviorCoroutine(AttackRoutine());
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
        private IEnumerator AttackRoutine()
        {
            StopMovement();

            // Random: Front Combo veya Back Attack
            bool frontCombo = Random.value < comboChance;

            if (frontCombo)
            {
                yield return FrontComboPattern();
            }
            else
            {
                yield return BackAttackPattern();
            }

            // Attack bitti, Chase'e dön
            ChangeState(AIState.Chase);
        }

        /// <summary>
        /// Pattern 1: Front Combo (Jab -> Cross)
        /// </summary>
        private IEnumerator FrontComboPattern()
        {
            // Step forward
            yield return new WaitForSeconds(0.1f);

            // Jab (Light punch)
            animator.Play("Attack_Light");
            yield return new WaitForSeconds(0.1f);
            ActivateAttackHitbox();
            yield return new WaitForSeconds(0.2f);

            // Cross (Heavy punch)
            animator.Play("Attack_Heavy");
            yield return new WaitForSeconds(0.15f);
            ActivateAttackHitbox();
            yield return new WaitForSeconds(0.3f);

            // Recovery
            yield return new WaitForSeconds(0.3f);
        }

        /// <summary>
        /// Pattern 2: Back Attack (Grab attempt)
        /// </summary>
        private IEnumerator BackAttackPattern()
        {
            // Oyuncunun arkasına geçme denemesi (basit teleport)
            Vector3 behindPlayer = target.position - target.forward * 1.5f;
            transform.position = behindPlayer;

            yield return new WaitForSeconds(0.2f);

            // Grab animation
            animator.Play("Grab_Attempt");
            yield return new WaitForSeconds(0.3f);

            // Başarı kontrolü (basit distance check)
            if (GetDistanceToTarget() < 1f)
            {
                // Başarılı grab
                Debug.Log("Grab Success!");
                // Oyuncuya hasar ver
                ExecuteAttack();
            }
            else
            {
                // Başarısız, stun
                yield return new WaitForSeconds(0.5f);
            }
        }

        /// <summary>
        /// Pattern 3: Desperate Attack (HP < 30%)
        /// </summary>
        private IEnumerator DesperateAttackPattern()
        {
            // Sürekli saldırı modu
            for (int i = 0; i < 3; i++)
            {
                animator.Play("Attack_Light");
                yield return new WaitForSeconds(0.2f);
                ActivateAttackHitbox();
                yield return new WaitForSeconds(0.3f);
            }
        }
        #endregion

        #region Combat Callbacks
        public override void OnDamageReceived()
        {
            base.OnDamageReceived();

            // %20 şans ile hurt state'e geç
            if (Random.value < 0.2f && currentState != AIState.Hurt)
            {
                ChangeState(AIState.Hurt);
            }

            // Düşük can ise desperate mode
            if (controller.CurrentHealth < controller.MaxHealth * 0.3f)
            {
                aggressiveness = 1f; // Daha agresif
                attackCooldown = 1f; // Daha hızlı saldırı
            }
        }
        #endregion
    }
}

