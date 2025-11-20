using UnityEngine;
using System.Collections;
using NeonSyndicate.Core;
using NeonSyndicate.Combat;

namespace NeonSyndicate.Enemy.AI
{
    /// <summary>
    /// Tüm düşman AI'ları için base class.
    /// Her AI tipi bundan türer ve kendi davranışını implement eder.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public abstract class EnemyAIBase : MonoBehaviour
    {
        [Header("Base AI Settings")]
        [SerializeField] protected float detectionRange = 8f;
        [SerializeField] protected float attackRange = 1.5f;
        [SerializeField] protected float attackCooldown = 2f;
        
        [Header("AI Personality")]
        [SerializeField] protected float aggressiveness = 0.5f;  // 0-1
        [SerializeField] protected float caution = 0.3f;          // 0-1
        [SerializeField] protected float intelligence = 0.7f;     // 0-1

        [Header("Current State")]
        [SerializeField] protected AIState currentState = AIState.Idle;
        [SerializeField] protected bool hasAttackToken = false;

        protected Transform target;
        protected float lastAttackTime;
        protected float stateTimer;
        protected Coroutine currentBehaviorCoroutine;

        // Components
        protected EnemyController controller;
        protected Rigidbody2D rb;
        protected Animator animator;
        protected SpriteRenderer spriteRenderer;
        protected Hitbox attackHitbox;

        public enum AIState
        {
            Idle,
            Patrol,
            Chase,
            CircleStrafe,
            Attack,
            BackOff,
            Hurt,
            Stunned,
            Death
        }

        protected virtual void Awake()
        {
            controller = GetComponent<EnemyController>();
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            attackHitbox = GetComponentInChildren<Hitbox>();
        }

        protected virtual void Start()
        {
            // Oyuncuyu hedef al
            target = GameObject.FindGameObjectWithTag("Player")?.transform;

            // Token Manager'a kayıt ol
            AITokenManager.Instance?.RegisterEnemy(this);
        }

        protected virtual void Update()
        {
            if (controller.IsDead) return;

            stateTimer += Time.deltaTime;

            // Alt sınıflar kendi update logic'ini implement eder
            UpdateAI();
        }

        /// <summary>
        /// Her AI tipinin kendi update logic'i (override edilmeli).
        /// </summary>
        protected abstract void UpdateAI();

        #region State Management
        protected virtual void ChangeState(AIState newState)
        {
            // Exit current state
            OnStateExit(currentState);

            currentState = newState;
            stateTimer = 0f;

            // Enter new state
            OnStateEnter(newState);
        }

        protected virtual void OnStateEnter(AIState state)
        {
            // Alt sınıflar override edebilir
        }

        protected virtual void OnStateExit(AIState state)
        {
            // Coroutine cleanup
            StopCurrentBehavior();
        }
        #endregion

        #region Movement
        protected void MoveTowardsTarget(float speed)
        {
            if (target == null) return;

            Vector2 direction = (target.position - transform.position).normalized;
            rb.velocity = direction * speed;

            // Sprite flip
            if (direction.x != 0)
            {
                spriteRenderer.flipX = direction.x < 0;
            }

            animator.SetBool("IsWalking", true);
        }

        protected void MoveAwayFromTarget(float speed)
        {
            if (target == null) return;

            Vector2 direction = (transform.position - target.position).normalized;
            rb.velocity = direction * speed;

            // Sprite flip
            if (direction.x != 0)
            {
                spriteRenderer.flipX = direction.x < 0;
            }

            animator.SetBool("IsWalking", true);
        }

        protected void CircleStrafeTarget(float speed)
        {
            if (target == null) return;

            Vector2 directionToTarget = (target.position - transform.position).normalized;
            Vector2 perpendicular = new Vector2(-directionToTarget.y, directionToTarget.x);

            float strafeDirection = Mathf.Sin(Time.time * 2f);
            rb.velocity = perpendicular * strafeDirection * speed;

            animator.SetBool("IsWalking", true);
        }

        protected void StopMovement()
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("IsWalking", false);
        }
        #endregion

        #region Attack System
        protected bool CanAttack()
        {
            return hasAttackToken && 
                   Time.time - lastAttackTime >= attackCooldown &&
                   GetDistanceToTarget() <= attackRange;
        }

        protected virtual void ExecuteAttack()
        {
            lastAttackTime = Time.time;
            animator.SetTrigger("Attack");
            SoundManager.Instance?.PlaySFX("Enemy_Attack");
        }

        /// <summary>
        /// Animasyon eventi ile çağrılır.
        /// </summary>
        public void ActivateAttackHitbox()
        {
            if (attackHitbox != null)
            {
                attackHitbox.Activate(transform, controller.AttackDamage);
            }
        }
        #endregion

        #region Utility
        protected float GetDistanceToTarget()
        {
            if (target == null) return Mathf.Infinity;
            return Vector2.Distance(transform.position, target.position);
        }

        protected bool IsTargetInRange(float range)
        {
            return GetDistanceToTarget() <= range;
        }

        protected void StartBehaviorCoroutine(IEnumerator coroutine)
        {
            StopCurrentBehavior();
            currentBehaviorCoroutine = StartCoroutine(coroutine);
        }

        protected void StopCurrentBehavior()
        {
            if (currentBehaviorCoroutine != null)
            {
                StopCoroutine(currentBehaviorCoroutine);
                currentBehaviorCoroutine = null;
            }
        }
        #endregion

        #region Token System
        public void GrantAttackToken()
        {
            hasAttackToken = true;
        }

        public void RevokeAttackToken()
        {
            hasAttackToken = false;
        }

        public bool HasAttackToken => hasAttackToken;
        public AIState CurrentState => currentState;
        #endregion

        #region Combat Callbacks
        public virtual void OnDamageReceived()
        {
            // Alt sınıflar override edebilir
        }

        public virtual void OnDeath()
        {
            AITokenManager.Instance?.UnregisterEnemy(this);
            enabled = false;
        }
        #endregion

        protected virtual void OnDestroy()
        {
            AITokenManager.Instance?.UnregisterEnemy(this);
        }

        protected virtual void OnDrawGizmosSelected()
        {
            // Detection range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            // Attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}

