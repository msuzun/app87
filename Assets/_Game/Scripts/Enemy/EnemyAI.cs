using UnityEngine;
using NeonSyndicate.Core;
using NeonSyndicate.Combat;

namespace NeonSyndicate.Enemy
{
    /// <summary>
    /// Düşman yapay zekası. Token sistemi ile çalışır.
    /// Saldırı, kaçınma ve takip davranışlarını yönetir.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyAI : MonoBehaviour
    {
        [Header("AI Settings")]
        [SerializeField] private float detectionRange = 8f;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float attackCooldown = 2f;

        [Header("Behavior")]
        [SerializeField] private AIState currentState = AIState.Idle;
        [SerializeField] private bool hasAttackToken = false;

        [Header("Target")]
        private Transform target;
        private float lastAttackTime;
        private float stateTimer;

        // Components
        private EnemyController controller;
        private Rigidbody2D rb;
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private Hitbox attackHitbox;

        public enum AIState
        {
            Idle,       // Bekliyor
            Patrol,     // Devriye geziyor
            Chase,      // Oyuncuyu kovalıyor
            Attack,     // Saldırıyor
            Retreat,    // Geri çekiliyor (Gunner)
            Stunned     // Sersemlemiş
        }

        private void Awake()
        {
            controller = GetComponent<EnemyController>();
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            attackHitbox = GetComponentInChildren<Hitbox>();
        }

        private void Start()
        {
            // Oyuncuyu hedef al
            target = GameObject.FindGameObjectWithTag("Player")?.transform;

            // Token Manager'a kayıt ol
            AITokenManager.Instance?.RegisterEnemy(this);
        }

        private void Update()
        {
            if (controller.IsDead) return;

            stateTimer += Time.deltaTime;

            switch (currentState)
            {
                case AIState.Idle:
                    UpdateIdle();
                    break;
                case AIState.Chase:
                    UpdateChase();
                    break;
                case AIState.Attack:
                    UpdateAttack();
                    break;
                case AIState.Retreat:
                    UpdateRetreat();
                    break;
                case AIState.Stunned:
                    UpdateStunned();
                    break;
            }
        }

        #region State Updates
        private void UpdateIdle()
        {
            if (target == null) return;

            float distanceToTarget = Vector2.Distance(transform.position, target.position);

            // Menzil içindeyse kovalamaya başla
            if (distanceToTarget <= detectionRange)
            {
                ChangeState(AIState.Chase);
            }
        }

        private void UpdateChase()
        {
            if (target == null)
            {
                ChangeState(AIState.Idle);
                return;
            }

            float distanceToTarget = Vector2.Distance(transform.position, target.position);

            // Saldırı menzilindeyse ve token varsa saldır
            if (distanceToTarget <= attackRange && hasAttackToken)
            {
                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    ChangeState(AIState.Attack);
                }
            }
            // Çok uzaklaştıysa Idle'a dön
            else if (distanceToTarget > detectionRange * 1.5f)
            {
                ChangeState(AIState.Idle);
            }
            // Kovalamaya devam et
            else
            {
                MoveTowardsTarget();
            }
        }

        private void UpdateAttack()
        {
            // Saldırı animasyonu oynuyor (0.8 saniye)
            if (stateTimer >= 0.8f)
            {
                lastAttackTime = Time.time;
                ChangeState(AIState.Chase);
            }
        }

        private void UpdateRetreat()
        {
            // Gunner tipi için: Oyuncudan uzaklaş
            if (target != null)
            {
                Vector2 retreatDirection = (transform.position - target.position).normalized;
                rb.velocity = retreatDirection * controller.MoveSpeed;

                float distanceToTarget = Vector2.Distance(transform.position, target.position);
                if (distanceToTarget > attackRange * 2f)
                {
                    ChangeState(AIState.Chase);
                }
            }
        }

        private void UpdateStunned()
        {
            // Sersemleme süresi (1 saniye)
            if (stateTimer >= 1f)
            {
                ChangeState(AIState.Chase);
            }
        }
        #endregion

        #region Movement
        private void MoveTowardsTarget()
        {
            if (target == null) return;

            // Token yoksa saldırgan olma, sadece çevrele (Circle Strafe)
            if (!hasAttackToken)
            {
                CircleStrafeTarget();
                return;
            }

            // Hedefe doğru hareket et
            Vector2 direction = (target.position - transform.position).normalized;
            rb.velocity = direction * controller.MoveSpeed;

            // Sprite flip
            if (direction.x != 0)
            {
                spriteRenderer.flipX = direction.x < 0;
            }

            // Animasyon
            animator.SetBool("IsWalking", true);
        }

        private void CircleStrafeTarget()
        {
            // Hedefin etrafında dolanma hareketi
            Vector2 directionToTarget = (target.position - transform.position).normalized;
            Vector2 perpendicular = new Vector2(-directionToTarget.y, directionToTarget.x);

            // Rastgele sağa veya sola dolanma
            float strafeDirection = Mathf.Sin(Time.time * 2f);
            rb.velocity = perpendicular * strafeDirection * controller.MoveSpeed * 0.5f;

            animator.SetBool("IsWalking", true);
        }
        #endregion

        #region State Management
        private void ChangeState(AIState newState)
        {
            currentState = newState;
            stateTimer = 0f;

            switch (newState)
            {
                case AIState.Attack:
                    rb.velocity = Vector2.zero;
                    animator.SetTrigger("Attack");
                    break;

                case AIState.Stunned:
                    rb.velocity = Vector2.zero;
                    animator.SetTrigger("Stunned");
                    break;
            }
        }
        #endregion

        #region Token System
        /// <summary>
        /// AI Token Manager'dan saldırı tokeni alır.
        /// </summary>
        public void GrantAttackToken()
        {
            hasAttackToken = true;
        }

        /// <summary>
        /// Saldırı tokenini geri verir.
        /// </summary>
        public void RevokeAttackToken()
        {
            hasAttackToken = false;
        }

        public bool HasAttackToken => hasAttackToken;
        public AIState CurrentState => currentState;
        #endregion

        #region Combat Callbacks
        public void OnDamageReceived()
        {
            // Hasar alınca kısa süreli stunned olabilir (opsiyonel)
            if (Random.value < 0.2f) // %20 şans
            {
                ChangeState(AIState.Stunned);
            }
        }

        public void OnDeath()
        {
            // Token Manager'dan çık
            AITokenManager.Instance?.UnregisterEnemy(this);

            // AI'yı durdur
            enabled = false;
        }

        /// <summary>
        /// Animasyon eventi ile çağrılır. Hitbox'ı aktifleştirir.
        /// </summary>
        public void ActivateAttackHitbox()
        {
            if (attackHitbox != null)
            {
                attackHitbox.Activate(transform, controller.AttackDamage);
            }
        }
        #endregion

        private void OnDrawGizmosSelected()
        {
            // Detection range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            // Attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

        private void OnDestroy()
        {
            // Token Manager'dan kayıt sil
            AITokenManager.Instance?.UnregisterEnemy(this);
        }
    }
}

