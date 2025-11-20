using UnityEngine;
using System.Collections;
using NeonSyndicate.Core;

namespace NeonSyndicate.Enemy.AI
{
    /// <summary>
    /// HEAVY TANK - Ağır Zırhlı
    /// Yavaş ama güçlü, super armor, yüksek hasar.
    /// Boss-like mini-tank düşman.
    /// </summary>
    public class HeavyTankAI : EnemyAIBase
    {
        [Header("Tank Specific")]
        [SerializeField] private float chargeSpeed = 6f;          // Charge saldırısı hızı
        [SerializeField] private float chargeRange = 8f;          // Charge mesafesi
        [SerializeField] private float grabRange = 1.5f;          // Grab mesafesi
        [SerializeField] private float groundPoundRange = 3f;     // AOE range
        [SerializeField] private bool hasSuperArmor = true;       // Flinch resistance

        [Header("Berserker Mode")]
        [SerializeField] private bool isBerserker = false;
        [SerializeField] private float berserkerThreshold = 0.3f; // HP %30

        private bool isCharging = false;
        private bool isStunned = false;

        protected override void Start()
        {
            base.Start();
            ChangeState(AIState.Idle); // Intimidate
        }

        protected override void UpdateAI()
        {
            // Berserker mode check
            if (!isBerserker && controller.CurrentHealth < controller.MaxHealth * berserkerThreshold)
            {
                ActivateBerserkerMode();
            }

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

            // HP < 30%? Berserker mode
            if (isBerserker)
            {
                BerserkerBehavior(distanceToTarget);
                return;
            }

            // Has Attack Token?
            if (hasAttackToken && CanAttack())
            {
                // Close range? -> Grab
                if (distanceToTarget <= grabRange)
                {
                    ChangeState(AIState.Attack); // Grab attack
                }
                // Medium range? -> Heavy Swing
                else if (distanceToTarget <= attackRange)
                {
                    ChangeState(AIState.Attack); // Heavy swing
                }
                // Far range? -> Charge
                else if (distanceToTarget <= chargeRange)
                {
                    ChangeState(AIState.CircleStrafe); // Charge state olarak kullanıyoruz
                }
                else
                {
                    // Too far, advance
                    if (currentState != AIState.Chase)
                        ChangeState(AIState.Chase);
                }
            }
            // No Token -> Slow Advance (Intimidate)
            else
            {
                if (distanceToTarget > detectionRange)
                {
                    if (currentState != AIState.Idle)
                        ChangeState(AIState.Idle);
                }
                else
                {
                    if (currentState != AIState.Chase)
                        ChangeState(AIState.Chase); // Slow advance
                }
            }
        }

        private void BerserkerBehavior(float distanceToTarget)
        {
            // Berserker mode: Daha agresif
            if (hasAttackToken && CanAttack())
            {
                // Charge attack or Ground Pound
                if (Random.value < 0.5f)
                {
                    ChangeState(AIState.CircleStrafe); // Charge
                }
                else
                {
                    ChangeState(AIState.Attack); // Ground Pound
                }
            }
            else
            {
                // Hızlı advance
                if (currentState != AIState.Chase)
                    ChangeState(AIState.Chase);
            }
        }
        #endregion

        #region State Machine
        private void UpdateCurrentState()
        {
            switch (currentState)
            {
                case AIState.Idle: // Intimidate
                    UpdateIntimidateState();
                    break;

                case AIState.Chase: // Slow/Fast Advance
                    UpdateAdvanceState();
                    break;

                case AIState.CircleStrafe: // Charge (repurposed)
                    // Coroutine kontrolünde
                    break;

                case AIState.Attack:
                    // Coroutine kontrolünde
                    break;

                case AIState.Stunned:
                    UpdateStunnedState();
                    break;

                case AIState.Hurt:
                    UpdateHurtState();
                    break;
            }
        }

        private void UpdateIntimidateState()
        {
            StopMovement();

            // Intimidate animation (breathing, flexing)
            animator.SetBool("IsIntimidating", true);

            // Oyuncu yakınsa advance
            if (IsTargetInRange(detectionRange))
            {
                ChangeState(AIState.Chase);
            }
        }

        private void UpdateAdvanceState()
        {
            float speed = isBerserker ? controller.MoveSpeed * 1.5f : controller.MoveSpeed;
            MoveTowardsTarget(speed);

            animator.SetBool("IsIntimidating", false);
        }

        private void UpdateStunnedState()
        {
            StopMovement();

            // Stun süresi bitti mi?
            if (stateTimer > 2f)
            {
                isStunned = false;
                ChangeState(AIState.Chase);
            }
        }

        private void UpdateHurtState()
        {
            // Super armor varsa hurt state çabuk bitsin
            float hurtDuration = hasSuperArmor ? 0.2f : 0.4f;

            if (stateTimer > hurtDuration)
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
                case AIState.Attack:
                    // Random: Heavy Swing, Grab, veya Ground Pound
                    float distanceToTarget = GetDistanceToTarget();
                    
                    if (distanceToTarget <= grabRange)
                    {
                        StartBehaviorCoroutine(GrabAttackRoutine());
                    }
                    else if (isBerserker && Random.value < 0.3f)
                    {
                        StartBehaviorCoroutine(GroundPoundRoutine());
                    }
                    else
                    {
                        StartBehaviorCoroutine(HeavySwingRoutine());
                    }
                    break;

                case AIState.CircleStrafe: // Charge repurposed
                    StartBehaviorCoroutine(ChargeAttackRoutine());
                    break;

                case AIState.Stunned:
                    isStunned = true;
                    StopMovement();
                    animator.Play("Stunned");
                    break;

                case AIState.Hurt:
                    if (!hasSuperArmor)
                    {
                        StopMovement();
                        animator.SetTrigger("Hurt");
                    }
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
        /// Pattern 1: Heavy Swing
        /// Yavaş ama güçlü saldırı
        /// </summary>
        private IEnumerator HeavySwingRoutine()
        {
            StopMovement();

            // Wind-up (0.6s) - Telegraph
            animator.Play("Heavy_Windup");
            
            // Visual warning (örn: glow effect)
            yield return new WaitForSeconds(0.6f);

            // Swing (0.4s)
            animator.Play("Heavy_Swing");
            yield return new WaitForSeconds(0.2f);
            
            // Hitbox active (180 degree arc)
            ActivateAttackHitbox();
            SoundManager.Instance?.PlaySFX("Heavy_Swing");
            
            yield return new WaitForSeconds(0.2f);

            // Recovery (0.5s) - Vulnerable window
            yield return new WaitForSeconds(0.5f);

            ChangeState(AIState.Chase);
        }

        /// <summary>
        /// Pattern 2: Charge Attack
        /// Boğa gibi koşma saldırısı
        /// </summary>
        private IEnumerator ChargeAttackRoutine()
        {
            isCharging = true;

            // Roar (0.3s) - Warning
            animator.Play("Charge_Roar");
            yield return new WaitForSeconds(0.3f);

            // Charge direction
            Vector2 chargeDirection = (target.position - transform.position).normalized;
            float chargeTime = 0f;
            float maxChargeTime = 1.5f;

            animator.Play("Charging");
            SoundManager.Instance?.PlaySFX("Charge_Start");

            // Charge phase (1.0s)
            while (chargeTime < maxChargeTime)
            {
                rb.velocity = chargeDirection * chargeSpeed;
                chargeTime += Time.deltaTime;

                // Hit player?
                if (GetDistanceToTarget() < 1f)
                {
                    // Impact!
                    ActivateAttackHitbox();
                    SoundManager.Instance?.PlaySFX("Charge_Impact");
                    break;
                }

                // Hit wall?
                RaycastHit2D hit = Physics2D.Raycast(transform.position, chargeDirection, 0.5f);
                if (hit.collider != null && hit.collider.CompareTag("Wall"))
                {
                    // Crash into wall -> Stunned
                    StopMovement();
                    ChangeState(AIState.Stunned);
                    SoundManager.Instance?.PlaySFX("Wall_Crash");
                    yield break;
                }

                yield return null;
            }

            isCharging = false;
            StopMovement();

            // Recovery
            yield return new WaitForSeconds(0.3f);
            ChangeState(AIState.Chase);
        }

        /// <summary>
        /// Pattern 3: Grab Attack
        /// Yakala ve fırlat
        /// </summary>
        private IEnumerator GrabAttackRoutine()
        {
            StopMovement();

            // Grab attempt animation
            animator.Play("Grab_Attempt");
            yield return new WaitForSeconds(0.2f);

            // Success check
            if (GetDistanceToTarget() <= grabRange)
            {
                // Successful grab
                animator.Play("Grab_Success");
                
                // Lift (0.3s)
                yield return new WaitForSeconds(0.3f);

                // Throw (0.5s)
                animator.Play("Throw");
                yield return new WaitForSeconds(0.2f);
                
                // Apply damage and knockback
                ActivateAttackHitbox();
                SoundManager.Instance?.PlaySFX("Throw_Impact");
                
                yield return new WaitForSeconds(0.3f);
            }
            else
            {
                // Failed grab -> Very vulnerable
                animator.Play("Grab_Fail");
                yield return new WaitForSeconds(1.0f); // Long recovery
            }

            ChangeState(AIState.Chase);
        }

        /// <summary>
        /// Pattern 4: Ground Pound (AOE)
        /// Yere yumruk -> shockwave
        /// </summary>
        private IEnumerator GroundPoundRoutine()
        {
            // Jump (0.3s)
            animator.Play("Jump_Windup");
            yield return new WaitForSeconds(0.3f);

            // Peak (0.2s)
            yield return new WaitForSeconds(0.2f);

            // Slam (0.4s)
            animator.Play("Ground_Slam");
            yield return new WaitForSeconds(0.4f);

            // Shockwave effect
            CreateShockwave();
            SoundManager.Instance?.PlaySFX("Ground_Pound");

            // Camera shake
            // CameraShake.Instance?.Shake(0.5f, 0.3f);

            yield return new WaitForSeconds(0.3f);
            ChangeState(AIState.Chase);
        }

        private void CreateShockwave()
        {
            // AOE damage
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, groundPoundRange);
            
            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    float distance = Vector2.Distance(transform.position, hit.transform.position);
                    float damage = distance < 2f ? 30f : 15f;

                    // Apply damage
                    // hit.GetComponent<IDamageable>()?.TakeDamage(damage, ...);

                    Debug.Log($"Ground Pound hit player! Damage: {damage}");
                }
            }

            // Visual effect (particle system)
            if (ObjectPooler.Instance != null)
            {
                ObjectPooler.Instance.SpawnFromPool("Shockwave_VFX", transform.position, Quaternion.identity);
            }
        }
        #endregion

        #region Berserker Mode
        private void ActivateBerserkerMode()
        {
            isBerserker = true;
            hasSuperArmor = true;

            // Stat boosts
            aggressiveness = 1f;
            attackCooldown = 1.5f; // Faster attacks

            // Visual feedback
            animator.SetBool("IsBerserker", true);
            SoundManager.Instance?.PlaySFX("Berserker_Roar");

            Debug.Log("Heavy Tank entered BERSERKER MODE!");
        }
        #endregion

        #region Combat Callbacks
        public override void OnDamageReceived()
        {
            base.OnDamageReceived();

            // Super armor varsa flinch etme
            if (hasSuperArmor && !isCharging)
            {
                // Hitstop effect ama state değişimi yok
                return;
            }

            // Normal hurt
            if (currentState != AIState.Hurt && currentState != AIState.Stunned)
            {
                ChangeState(AIState.Hurt);
            }
        }

        public override void OnDeath()
        {
            base.OnDeath();
            isCharging = false;
            isBerserker = false;
        }
        #endregion

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            // Charge range
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, chargeRange);

            // Ground pound range
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, groundPoundRange);
        }
    }
}

