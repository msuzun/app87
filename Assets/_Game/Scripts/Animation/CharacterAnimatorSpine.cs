// OPTIONAL: Bu dosya sadece Spine kullanıyorsanız aktif edilmelidir.
// Spine Runtime paketi yüklenmediyse bu script compile hatası verecektir.
// Kullanmıyorsanız bu dosyayı silebilir veya .txt uzantısıyla kaydedebilirsiniz.

#if SPINE_UNITY
using UnityEngine;
using Spine;
using Spine.Unity;

namespace NeonSyndicate.Animation
{
    /// <summary>
    /// Character Animator Wrapper - Spine 2D Version
    /// 
    /// Spine kullanıyorsanız CharacterAnimator.cs yerine bunu kullanın.
    /// API tamamen aynı, sadece backend Spine kullanır.
    /// 
    /// Kurulum:
    /// 1. Spine Runtime paketini yükle
    /// 2. CharacterAnimator.cs'i disable et
    /// 3. Bunu kullan
    /// </summary>
    [RequireComponent(typeof(SkeletonAnimation))]
    public class CharacterAnimatorSpine : MonoBehaviour
    {
        [Header("Blend Settings")]
        [SerializeField] private float defaultMixDuration = 0.15f;
        [SerializeField] private float combatMixDuration = 0.05f;

        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = false;

        // Components
        private SkeletonAnimation skeletonAnimation;
        private AnimationEventReceiver eventReceiver;

        // State tracking
        private string currentAnimationName;

        public AnimationEventReceiver EventReceiver => eventReceiver;

        private void Awake()
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
            eventReceiver = GetComponentInChildren<AnimationEventReceiver>();

            // Spine event'leri dinle
            skeletonAnimation.AnimationState.Event += OnSpineEvent;
            skeletonAnimation.AnimationState.Complete += OnSpineComplete;
        }

        #region Animation Playback
        /// <summary>
        /// Animasyon oynatır (Spine MixDuration ile).
        /// </summary>
        public void PlayAnimation(string animationName, bool isCombatAction = false, float manualMix = -1f)
        {
            if (string.IsNullOrEmpty(animationName)) return;
            if (currentAnimationName == animationName) return;

            float mixDuration = manualMix >= 0 
                ? manualMix 
                : (isCombatAction ? combatMixDuration : defaultMixDuration);

            // Spine animasyon set et
            var trackEntry = skeletonAnimation.AnimationState.SetAnimation(0, animationName, false);
            
            // Mix duration (blend)
            skeletonAnimation.AnimationState.Data.SetMix(currentAnimationName, animationName, mixDuration);

            currentAnimationName = animationName;

            if (showDebugInfo)
            {
                Debug.Log($"[Spine] Playing: {animationName} (mix: {mixDuration:F3}s)");
            }
        }

        /// <summary>
        /// Loop animasyon oynatır.
        /// </summary>
        public void PlayLoopAnimation(string animationName, float mixDuration = -1f)
        {
            float mix = mixDuration >= 0 ? mixDuration : defaultMixDuration;
            
            var trackEntry = skeletonAnimation.AnimationState.SetAnimation(0, animationName, true);
            skeletonAnimation.AnimationState.Data.SetMix(currentAnimationName, animationName, mix);

            currentAnimationName = animationName;
        }
        #endregion

        #region Spine Event Handling
        /// <summary>
        /// Spine event'lerini Unity event'lerine dönüştürür.
        /// </summary>
        private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
        {
            // Spine event name'i Unity event name'e map et
            switch (e.Data.Name)
            {
                case "HitFrame":
                case "EnableHitbox":
                    eventReceiver?.OnHitboxEnable?.Invoke();
                    break;

                case "DisableHitbox":
                    eventReceiver?.OnHitboxDisable?.Invoke();
                    break;

                case "ComboStart":
                    eventReceiver?.OnComboWindowOpen?.Invoke();
                    break;

                case "ComboEnd":
                    eventReceiver?.OnComboWindowClose?.Invoke();
                    break;

                case "Footstep":
                    eventReceiver?.OnFootstep?.Invoke();
                    break;

                case "SpawnVFX":
                    // Spine event'te string data varsa
                    string vfxName = e.String ?? "DefaultVFX";
                    eventReceiver?.OnSpawnVFX?.Invoke(vfxName);
                    break;

                case "CameraShake":
                    // Spine event'te float data varsa
                    float intensity = e.Float > 0 ? e.Float : 0.3f;
                    eventReceiver?.OnCameraShake?.Invoke(intensity);
                    break;

                default:
                    if (showDebugInfo)
                    {
                        Debug.Log($"[Spine Event] Unhandled: {e.Data.Name}");
                    }
                    break;
            }
        }

        /// <summary>
        /// Spine animasyon tamamlandığında.
        /// </summary>
        private void OnSpineComplete(TrackEntry trackEntry)
        {
            eventReceiver?.OnAnimationComplete?.Invoke();

            if (showDebugInfo)
            {
                Debug.Log($"[Spine] Animation Complete: {trackEntry.Animation.Name}");
            }
        }
        #endregion

        #region Speed Control
        public void SetPlaybackSpeed(float speed)
        {
            skeletonAnimation.AnimationState.TimeScale = speed;
        }

        public void ResetPlaybackSpeed()
        {
            skeletonAnimation.AnimationState.TimeScale = 1f;
        }
        #endregion

        #region State Queries
        public bool IsPlayingState(string animationName)
        {
            return currentAnimationName == animationName;
        }

        public string GetCurrentStateName()
        {
            return currentAnimationName;
        }

        public float GetNormalizedTime()
        {
            var trackEntry = skeletonAnimation.AnimationState.GetCurrent(0);
            if (trackEntry == null) return 0f;

            return trackEntry.AnimationTime / trackEntry.AnimationEnd;
        }
        #endregion

        private void OnDestroy()
        {
            // Spine event'lerden unsubscribe
            if (skeletonAnimation != null)
            {
                skeletonAnimation.AnimationState.Event -= OnSpineEvent;
                skeletonAnimation.AnimationState.Complete -= OnSpineComplete;
            }
        }
    }
}
#else
// Spine paketi yüklü değil. Bu script kullanılamaz.
// CharacterAnimator.cs kullanın (Unity Animator için).
namespace NeonSyndicate.Animation
{
    public class CharacterAnimatorSpine : UnityEngine.MonoBehaviour
    {
        private void Awake()
        {
            UnityEngine.Debug.LogWarning("[CharacterAnimatorSpine] Spine Runtime paketi yüklü değil! " +
                                        "CharacterAnimator.cs kullanın veya Spine'ı yükleyin.");
            enabled = false;
        }
    }
}
#endif

