using UnityEngine;
using UnityEngine.UI;
#if DOTWEEN_ENABLED
using DG.Tweening;
#endif

namespace NeonSyndicate.UI
{
    /// <summary>
    /// UI Effects - Screen shake, glitch, flash effects.
    /// 
    /// Crazy Flasher tarzı kinetic UI:
    /// - Screen shake (damage alınca)
    /// - Glitch effect (critical hit)
    /// - Flash effect (hit confirm)
    /// - Chromatic aberration (rage mode)
    /// </summary>
    public class UIEffects : MonoBehaviour
    {
        public static UIEffects Instance { get; private set; }

        [Header("Screen Shake")]
        [SerializeField] private RectTransform hudRoot;
        [SerializeField] private float shakeIntensity = 10f;
        [SerializeField] private float shakeDuration = 0.2f;

        [Header("Flash Effect")]
        [SerializeField] private Image flashOverlay;
        [SerializeField] private Color flashColor = Color.white;

        [Header("Glitch Effect")]
        [SerializeField] private Image glitchOverlay;
        [SerializeField] private Sprite[] glitchSprites;

        [Header("Vignette")]
        [SerializeField] private Image vignetteOverlay;

        private Vector3 originalHUDPosition;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (hudRoot != null)
            {
                originalHUDPosition = hudRoot.localPosition;
            }
        }

        #region Screen Shake
        /// <summary>
        /// HUD'ı sarsar (ekran shake effect).
        /// </summary>
        public void ShakeScreen(float intensity = 1f, float duration = -1f)
        {
            if (hudRoot == null) return;

            float finalIntensity = shakeIntensity * intensity;
            float finalDuration = duration > 0 ? duration : shakeDuration;

            #if DOTWEEN_ENABLED
            hudRoot.DOKill();
            hudRoot.DOShakePosition(finalDuration, finalIntensity, 10, 90, false, true)
                .OnComplete(() => hudRoot.localPosition = originalHUDPosition);
            #else
            StartCoroutine(ShakeRoutine(finalIntensity, finalDuration));
            #endif
        }

        private System.Collections.IEnumerator ShakeRoutine(float intensity, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                hudRoot.localPosition = originalHUDPosition + (Vector3)Random.insideUnitCircle * intensity;
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            hudRoot.localPosition = originalHUDPosition;
        }
        #endregion

        #region Flash Effect
        /// <summary>
        /// Ekranı flashlar (hit confirm).
        /// </summary>
        public void FlashScreen(Color? color = null, float duration = 0.1f)
        {
            if (flashOverlay == null) return;

            Color targetColor = color ?? flashColor;

            #if DOTWEEN_ENABLED
            flashOverlay.color = targetColor;
            flashOverlay.DOFade(0f, duration);
            #else
            StartCoroutine(FlashRoutine(targetColor, duration));
            #endif
        }

        private System.Collections.IEnumerator FlashRoutine(Color color, float duration)
        {
            flashOverlay.color = color;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                color.a = Mathf.Lerp(color.a, 0f, elapsed / duration);
                flashOverlay.color = color;
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            color.a = 0f;
            flashOverlay.color = color;
        }
        #endregion

        #region Glitch Effect
        /// <summary>
        /// VHS glitch efekti (critical moment).
        /// </summary>
        public void GlitchEffect(float duration = 0.2f)
        {
            if (glitchOverlay == null) return;

            StartCoroutine(GlitchRoutine(duration));
        }

        private System.Collections.IEnumerator GlitchRoutine(float duration)
        {
            glitchOverlay.gameObject.SetActive(true);
            float elapsed = 0f;
            int flickerCount = 5;

            for (int i = 0; i < flickerCount; i++)
            {
                // Random glitch sprite
                if (glitchSprites != null && glitchSprites.Length > 0)
                {
                    glitchOverlay.sprite = glitchSprites[Random.Range(0, glitchSprites.Length)];
                }

                // Random offset
                glitchOverlay.rectTransform.anchoredPosition = Random.insideUnitCircle * 50f;

                yield return new WaitForSecondsRealtime(duration / flickerCount);
            }

            glitchOverlay.gameObject.SetActive(false);
            glitchOverlay.rectTransform.anchoredPosition = Vector2.zero;
        }
        #endregion

        #region Vignette
        /// <summary>
        /// Vignette yoğunluğunu ayarlar (can azalınca karartma).
        /// </summary>
        public void SetVignetteIntensity(float intensity)
        {
            if (vignetteOverlay == null) return;

            Color vignetteColor = vignetteOverlay.color;
            vignetteColor.a = Mathf.Clamp01(intensity);
            
            #if DOTWEEN_ENABLED
            vignetteOverlay.DOColor(vignetteColor, 0.5f);
            #else
            vignetteOverlay.color = vignetteColor;
            #endif
        }
        #endregion

        #region Rage Mode Effects
        /// <summary>
        /// Rage mode aktif olduğunda UI efektleri.
        /// </summary>
        public void ActivateRageMode()
        {
            // Ekranın kenarlarında kırmızı glow
            SetVignetteIntensity(0.4f);
            vignetteOverlay.color = new Color(1f, 0f, 0f, 0.4f);

            #if DOTWEEN_ENABLED
            // Pulse effect
            vignetteOverlay.DOFade(0.6f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
            #endif
        }

        /// <summary>
        /// Rage mode deaktif.
        /// </summary>
        public void DeactivateRageMode()
        {
            #if DOTWEEN_ENABLED
            vignetteOverlay.DOKill();
            #endif
            
            SetVignetteIntensity(0f);
        }
        #endregion
    }
}

