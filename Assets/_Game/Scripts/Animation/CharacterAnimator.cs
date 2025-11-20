using UnityEngine;

namespace NeonSyndicate.Animation
{
    /// <summary>
    /// Character Animator Wrapper
    /// Mantık kodları ile Unity Animator arasındaki köprü.
    /// 
    /// Avantajları:
    /// - Teknoloji bağımsız (Spine'a geçmek kolay)
    /// - Blend (geçiş) yönetimi merkezi
    /// - Clean API
    /// - Event receiver'a kolay erişim
    /// 
    /// Kullanım:
    /// characterAnimator.PlayAnimation(AnimData.ATTACK_1, isCombat: true);
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimator : MonoBehaviour
    {
        [Header("Blend Settings")]
        [Tooltip("Normal animasyon geçişleri için blend süresi")]
        [SerializeField] private float defaultBlendDuration = 0.15f;
        
        [Tooltip("Dövüş animasyonları için blend süresi (daha keskin)")]
        [SerializeField] private float combatBlendDuration = 0.05f;
        
        [Tooltip("Instant geçiş için blend süresi")]
        [SerializeField] private float instantBlendDuration = 0f;

        [Header("Speed Control")]
        [Tooltip("Animasyon oynatma hızı (Hit Stop için değiştirilebilir)")]
        [SerializeField] private float playbackSpeed = 1f;

        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = false;
        [SerializeField] private string currentStateName = "";

        // Components
        private Animator animator;
        private AnimationEventReceiver eventReceiver;

        // State tracking
        private string currentState;
        private int currentStateHash;

        /// <summary>
        /// Event Receiver'a dışarıdan erişim.
        /// </summary>
        public AnimationEventReceiver EventReceiver => eventReceiver;

        /// <summary>
        /// Animator'a doğrudan erişim (gerekirse).
        /// </summary>
        public Animator Animator => animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            eventReceiver = GetComponentInChildren<AnimationEventReceiver>();

            if (eventReceiver == null)
            {
                Debug.LogWarning($"[CharacterAnimator] AnimationEventReceiver not found on {gameObject.name}! " +
                                 "Add it to the Animator GameObject.");
            }

            // Başlangıç hızı
            animator.speed = playbackSpeed;
        }

        private void Update()
        {
            if (showDebugInfo)
            {
                UpdateDebugInfo();
            }
        }

        #region Animation Playback
        /// <summary>
        /// Animasyon oynatır (CrossFade ile smooth geçiş).
        /// </summary>
        /// <param name="stateName">Animasyon state ismi (AnimData'dan)</param>
        /// <param name="isCombatAction">Dövüş animasyonu mu? (daha keskin geçiş)</param>
        /// <param name="manualBlend">Manuel blend süresi (-1 = otomatik)</param>
        public void PlayAnimation(string stateName, bool isCombatAction = false, float manualBlend = -1f)
        {
            if (string.IsNullOrEmpty(stateName))
            {
                Debug.LogWarning("[CharacterAnimator] Empty state name!");
                return;
            }

            // Aynı animasyon zaten oynuyorsa tekrar başlatma
            if (currentState == stateName && !isCombatAction)
            {
                return;
            }

            // Blend süresi belirleme
            float blendDuration = manualBlend >= 0 
                ? manualBlend 
                : (isCombatAction ? combatBlendDuration : defaultBlendDuration);

            // CrossFade (smooth geçiş)
            animator.CrossFade(stateName, blendDuration);

            // State tracking
            currentState = stateName;
            currentStateHash = Animator.StringToHash(stateName);

            if (showDebugInfo)
            {
                Debug.Log($"[CharacterAnimator] Playing: {stateName} (blend: {blendDuration:F3}s)");
            }
        }

        /// <summary>
        /// Animasyonu anında oynatır (blend yok).
        /// </summary>
        public void PlayAnimationInstant(string stateName)
        {
            PlayAnimation(stateName, false, instantBlendDuration);
        }

        /// <summary>
        /// State hash ile animasyon oynatır (performans optimizasyonu).
        /// </summary>
        public void PlayAnimation(int stateHash, float blendDuration = -1f)
        {
            if (currentStateHash == stateHash) return;

            float blend = blendDuration >= 0 ? blendDuration : defaultBlendDuration;
            animator.CrossFade(stateHash, blend);
            currentStateHash = stateHash;
        }
        #endregion

        #region Parameters
        /// <summary>
        /// Bool parameter ayarla.
        /// </summary>
        public void SetBool(string paramName, bool value)
        {
            animator.SetBool(paramName, value);
        }

        /// <summary>
        /// Bool parameter ayarla (hash ile - daha hızlı).
        /// </summary>
        public void SetBool(int paramHash, bool value)
        {
            animator.SetBool(paramHash, value);
        }

        /// <summary>
        /// Float parameter ayarla.
        /// </summary>
        public void SetFloat(string paramName, float value)
        {
            animator.SetFloat(paramName, value);
        }

        /// <summary>
        /// Float parameter ayarla (hash ile).
        /// </summary>
        public void SetFloat(int paramHash, float value)
        {
            animator.SetFloat(paramHash, value);
        }

        /// <summary>
        /// Int parameter ayarla.
        /// </summary>
        public void SetInt(string paramName, int value)
        {
            animator.SetInteger(paramName, value);
        }

        /// <summary>
        /// Trigger aktif et.
        /// </summary>
        public void SetTrigger(string paramName)
        {
            animator.SetTrigger(paramName);
        }

        /// <summary>
        /// Trigger aktif et (hash ile).
        /// </summary>
        public void SetTrigger(int paramHash)
        {
            animator.SetTrigger(paramHash);
        }

        /// <summary>
        /// Trigger sıfırla.
        /// </summary>
        public void ResetTrigger(string paramName)
        {
            animator.ResetTrigger(paramName);
        }
        #endregion

        #region Common Operations
        /// <summary>
        /// Hareket hızını ayarla (Blend Tree için).
        /// </summary>
        public void SetMovementSpeed(float speed)
        {
            animator.SetFloat(AnimData.Hash.Speed, speed);
        }

        /// <summary>
        /// Walking flag'ini ayarla.
        /// </summary>
        public void SetWalking(bool isWalking)
        {
            animator.SetBool(AnimData.Hash.IsWalking, isWalking);
        }

        /// <summary>
        /// Running flag'ini ayarla.
        /// </summary>
        public void SetRunning(bool isRunning)
        {
            animator.SetBool(AnimData.Hash.IsRunning, isRunning);
        }

        /// <summary>
        /// Grounded flag'ini ayarla.
        /// </summary>
        public void SetGrounded(bool isGrounded)
        {
            animator.SetBool(AnimData.Hash.IsGrounded, isGrounded);
        }
        #endregion

        #region Speed Control
        /// <summary>
        /// Animasyon oynatma hızını değiştirir.
        /// Hit Stop / Freeze Frame için kullanılır.
        /// </summary>
        /// <param name="speed">Hız (0 = dondur, 1 = normal, 2 = 2x hızlı)</param>
        public void SetPlaybackSpeed(float speed)
        {
            playbackSpeed = Mathf.Max(0f, speed);
            animator.speed = playbackSpeed;
        }

        /// <summary>
        /// Normal hıza geri dön.
        /// </summary>
        public void ResetPlaybackSpeed()
        {
            SetPlaybackSpeed(1f);
        }

        /// <summary>
        /// Animasyonu durdur (freeze).
        /// </summary>
        public void Pause()
        {
            SetPlaybackSpeed(0f);
        }

        /// <summary>
        /// Animasyonu devam ettir.
        /// </summary>
        public void Resume()
        {
            SetPlaybackSpeed(1f);
        }
        #endregion

        #region State Queries
        /// <summary>
        /// Belirli bir state oynuyor mu?
        /// </summary>
        public bool IsPlayingState(string stateName)
        {
            return currentState == stateName;
        }

        /// <summary>
        /// Mevcut state'in ismi.
        /// </summary>
        public string GetCurrentStateName()
        {
            return currentState;
        }

        /// <summary>
        /// Mevcut animasyonun normalize edilmiş zamanı (0-1).
        /// </summary>
        public float GetNormalizedTime()
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.normalizedTime;
        }

        /// <summary>
        /// Animasyon tamamlandı mı? (normalizedTime >= 1)
        /// </summary>
        public bool IsAnimationComplete()
        {
            return GetNormalizedTime() >= 1f;
        }
        #endregion

        #region Debug
        private void UpdateDebugInfo()
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            currentStateName = $"{currentState} ({stateInfo.normalizedTime:F2})";
        }

        private void OnGUI()
        {
            if (!showDebugInfo) return;

            GUILayout.BeginArea(new Rect(10, 300, 300, 150));
            GUILayout.Label("=== CHARACTER ANIMATOR ===");
            GUILayout.Label($"Current State: {currentStateName}");
            GUILayout.Label($"Playback Speed: {playbackSpeed:F2}x");
            GUILayout.Label($"Normalized Time: {GetNormalizedTime():F2}");
            GUILayout.Label($"Event Receiver: {(eventReceiver != null ? "✓" : "✗")}");
            GUILayout.EndArea();
        }
        #endregion

        #region Cleanup
        private void OnDestroy()
        {
            // Event receiver'ı temizle
            eventReceiver?.ClearAllEvents();
        }
        #endregion
    }
}

