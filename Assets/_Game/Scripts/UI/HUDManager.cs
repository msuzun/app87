using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if DOTWEEN_ENABLED
using DG.Tweening;
#endif

namespace NeonSyndicate.UI
{
    /// <summary>
    /// HUD Manager - Oyun içi arayüz yöneticisi.
    /// 
    /// Özellikler:
    /// - Reaktif health/rage bars (smooth animation)
    /// - Dynamic portrait (can durumuna göre değişir)
    /// - Combo counter (kinetik animasyon)
    /// - Boss health bar
    /// - Style rank display
    /// 
    /// Crazy Flasher tarzı: Her vuruşta UI da tepki verir!
    /// </summary>
    public class HUDManager : MonoBehaviour
    {
        public static HUDManager Instance { get; private set; }

        [Header("Player Status")]
        [SerializeField] private Image hpBarFill;
        [SerializeField] private Image rageBarFill;
        [SerializeField] private Image staminaBarFill;
        [SerializeField] private Image portraitImage;
        
        [Header("Portrait States")]
        [Tooltip("0: Healthy, 1: Hurt, 2: Critical, 3: Dying")]
        [SerializeField] private Sprite[] portraitStates;

        [Header("Combo Display")]
        [SerializeField] private GameObject comboContainer;
        [SerializeField] private TextMeshProUGUI comboText;
        [SerializeField] private TextMeshProUGUI comboLabel;
        [SerializeField] private Image comboBackground;

        [Header("Style Rank")]
        [SerializeField] private GameObject styleRankContainer;
        [SerializeField] private TextMeshProUGUI styleRankText;
        [SerializeField] private Color[] rankColors; // D, C, B, A, S, SS, SSS

        [Header("Boss HUD")]
        [SerializeField] private GameObject bossHealthContainer;
        [SerializeField] private Image bossHealthFill;
        [SerializeField] private TextMeshProUGUI bossNameText;

        [Header("Settings")]
        [SerializeField] private float comboTimeout = 2f;
        [SerializeField] private float barAnimationDuration = 0.3f;
        [SerializeField] private bool useAdvancedAnimations = true;

        // State
        private int currentCombo = 0;
        private float comboTimer = 0f;
        private string currentStyleRank = "D";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            InitializeHUD();
        }

        private void Update()
        {
            UpdateComboTimer();
        }

        #region Initialization
        private void InitializeHUD()
        {
            // Başlangıç değerleri
            if (comboContainer != null) 
                comboContainer.SetActive(false);
            
            if (styleRankContainer != null) 
                styleRankContainer.SetActive(false);
            
            if (bossHealthContainer != null) 
                bossHealthContainer.SetActive(false);

            // Bars
            UpdateHealth(100, 100);
            UpdateRage(0, 100);
            UpdateStamina(100, 100);
        }
        #endregion

        #region Health System
        /// <summary>
        /// Can barını günceller.
        /// </summary>
        public void UpdateHealth(float current, float max)
        {
            if (hpBarFill == null) return;

            float targetFill = Mathf.Clamp01(current / max);

            #if DOTWEEN_ENABLED
            if (useAdvancedAnimations)
            {
                // DOTween ile smooth animation
                hpBarFill.DOFillAmount(targetFill, barAnimationDuration)
                    .SetEase(Ease.OutCirc);
            }
            else
            #endif
            {
                // Basit lerp
                StartCoroutine(LerpFillAmount(hpBarFill, targetFill, barAnimationDuration));
            }

            // Portrait değişimi
            UpdatePortrait(targetFill);

            // Can kritikse bar yanıp sönsün
            if (targetFill < 0.3f)
            {
                StartHealthWarning();
            }
        }

        private void UpdatePortrait(float healthPercentage)
        {
            if (portraitImage == null || portraitStates == null || portraitStates.Length == 0) 
                return;

            int stateIndex = 0;
            if (healthPercentage > 0.7f) stateIndex = 0;      // Healthy
            else if (healthPercentage > 0.5f) stateIndex = 1; // Hurt
            else if (healthPercentage > 0.3f) stateIndex = 2; // Critical
            else stateIndex = 3;                               // Dying

            if (stateIndex < portraitStates.Length)
            {
                portraitImage.sprite = portraitStates[stateIndex];
            }

            // Can azaldıkça portrait kanlanır (color tint)
            portraitImage.color = Color.Lerp(Color.white, new Color(1f, 0.3f, 0.3f), 1f - healthPercentage);
        }

        private void StartHealthWarning()
        {
            #if DOTWEEN_ENABLED
            if (useAdvancedAnimations)
            {
                // Yanıp sönme
                hpBarFill.DOFade(0.3f, 0.3f).SetLoops(-1, LoopType.Yoyo);
            }
            #endif
        }
        #endregion

        #region Rage System
        /// <summary>
        /// Rage barını günceller.
        /// </summary>
        public void UpdateRage(float current, float max)
        {
            if (rageBarFill == null) return;

            float targetFill = Mathf.Clamp01(current / max);

            #if DOTWEEN_ENABLED
            if (useAdvancedAnimations)
            {
                rageBarFill.DOFillAmount(targetFill, 0.2f);
                
                // Full olduğunda parla
                if (targetFill >= 1f)
                {
                    rageBarFill.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 5);
                }
            }
            else
            #endif
            {
                StartCoroutine(LerpFillAmount(rageBarFill, targetFill, 0.2f));
            }

            // Renk değişimi (sarı → kırmızı → ateş)
            Color rageColor = Color.Lerp(Color.yellow, Color.red, targetFill);
            rageBarFill.color = rageColor;
        }
        #endregion

        #region Stamina System
        /// <summary>
        /// Stamina barını günceller.
        /// </summary>
        public void UpdateStamina(float current, float max)
        {
            if (staminaBarFill == null) return;

            float targetFill = Mathf.Clamp01(current / max);
            
            #if DOTWEEN_ENABLED
            if (useAdvancedAnimations)
            {
                staminaBarFill.DOFillAmount(targetFill, 0.15f);
            }
            else
            #endif
            {
                StartCoroutine(LerpFillAmount(staminaBarFill, targetFill, 0.15f));
            }
        }
        #endregion

        #region Combo System
        /// <summary>
        /// Kombo sayacını artırır (her vuruşta çağrılır).
        /// </summary>
        public void AddCombo()
        {
            // İlk vuruşsa container'ı göster
            if (currentCombo == 0)
            {
                ShowComboContainer();
            }

            currentCombo++;
            comboTimer = comboTimeout;

            // Text güncelle
            comboText.text = currentCombo.ToString();

            // Kinetic animation (Punch effect)
            AnimateComboHit();

            // Renk değişimi (combo arttıkça)
            UpdateComboColor();

            // Milestone messages
            CheckComboMilestones();
        }

        private void ShowComboContainer()
        {
            if (comboContainer == null) return;

            comboContainer.SetActive(true);
            
            #if DOTWEEN_ENABLED
            if (useAdvancedAnimations)
            {
                comboContainer.transform.localScale = Vector3.zero;
                comboContainer.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            }
            #endif
        }

        private void AnimateComboHit()
        {
            #if DOTWEEN_ENABLED
            if (useAdvancedAnimations)
            {
                // Punch scale (vuruş hissi)
                comboText.transform.DOKill();
                comboText.transform.localScale = Vector3.one * 1.5f;
                comboText.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutQuad);

                // Shake (titreme)
                comboText.transform.DOShakeRotation(0.1f, 10f);
            }
            #endif
        }

        private void UpdateComboColor()
        {
            Color targetColor = Color.white;

            if (currentCombo >= 50) targetColor = new Color(1f, 0f, 1f); // Magenta (GODLIKE)
            else if (currentCombo >= 30) targetColor = Color.red;         // Red
            else if (currentCombo >= 20) targetColor = new Color(1f, 0.5f, 0f); // Orange
            else if (currentCombo >= 10) targetColor = Color.yellow;      // Yellow
            else targetColor = Color.white;                               // White

            #if DOTWEEN_ENABLED
            if (useAdvancedAnimations)
            {
                comboText.DOColor(targetColor, 0.2f);
            }
            else
            #endif
            {
                comboText.color = targetColor;
            }
        }

        private void CheckComboMilestones()
        {
            // Milestone messages (Crazy Flasher tarzı!)
            if (currentCombo == 10)
            {
                ShowMilestoneText("KILLING SPREE!");
            }
            else if (currentCombo == 25)
            {
                ShowMilestoneText("DOMINATING!");
            }
            else if (currentCombo == 50)
            {
                ShowMilestoneText("UNSTOPPABLE!");
            }
            else if (currentCombo == 100)
            {
                ShowMilestoneText("GODLIKE!");
            }
        }

        private void ShowMilestoneText(string message)
        {
            // Ekranın ortasında büyük text (ileride implement edilebilir)
            Debug.Log($"[MILESTONE] {message}");
            // TODO: Implement milestone text UI
        }

        private void UpdateComboTimer()
        {
            if (currentCombo > 0)
            {
                comboTimer -= Time.deltaTime;

                if (comboTimer <= 0)
                {
                    ResetCombo();
                }
            }
        }

        /// <summary>
        /// Komboyu sıfırlar.
        /// </summary>
        public void ResetCombo()
        {
            if (currentCombo == 0) return;

            currentCombo = 0;
            comboTimer = 0f;

            // Fade out animation
            if (comboContainer != null)
            {
                #if DOTWEEN_ENABLED
                if (useAdvancedAnimations)
                {
                    comboContainer.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
                    {
                        comboContainer.SetActive(false);
                        comboContainer.transform.localScale = Vector3.one;
                    });
                }
                else
                #endif
                {
                    comboContainer.SetActive(false);
                }
            }
        }
        #endregion

        #region Boss HUD
        /// <summary>
        /// Boss health bar'ı gösterir.
        /// </summary>
        public void ShowBossHealth(string bossName, float maxHealth)
        {
            if (bossHealthContainer == null) return;

            bossHealthContainer.SetActive(true);
            
            if (bossNameText != null)
            {
                bossNameText.text = bossName.ToUpper();
            }

            if (bossHealthFill != null)
            {
                bossHealthFill.fillAmount = 1f;
            }

            #if DOTWEEN_ENABLED
            if (useAdvancedAnimations)
            {
                // Slide in from bottom
                bossHealthContainer.transform.localPosition = new Vector3(0, -100, 0);
                bossHealthContainer.transform.DOLocalMoveY(0, 0.5f).SetEase(Ease.OutBack);
            }
            #endif
        }

        /// <summary>
        /// Boss health'i günceller.
        /// </summary>
        public void UpdateBossHealth(float current, float max)
        {
            if (bossHealthFill == null) return;

            float targetFill = Mathf.Clamp01(current / max);

            #if DOTWEEN_ENABLED
            if (useAdvancedAnimations)
            {
                bossHealthFill.DOFillAmount(targetFill, 0.3f).SetEase(Ease.OutQuad);
            }
            else
            #endif
            {
                StartCoroutine(LerpFillAmount(bossHealthFill, targetFill, 0.3f));
            }
        }

        /// <summary>
        /// Boss öldüğünde bar'ı gizler.
        /// </summary>
        public void HideBossHealth()
        {
            if (bossHealthContainer == null) return;

            #if DOTWEEN_ENABLED
            if (useAdvancedAnimations)
            {
                bossHealthContainer.transform.DOLocalMoveY(-100, 0.5f).SetEase(Ease.InBack)
                    .OnComplete(() => bossHealthContainer.SetActive(false));
            }
            else
            #endif
            {
                bossHealthContainer.SetActive(false);
            }
        }
        #endregion

        #region Style Rank
        /// <summary>
        /// Stil derecesini gösterir (D, C, B, A, S, SS, SSS).
        /// </summary>
        public void UpdateStyleRank(string rank)
        {
            if (styleRankContainer == null || styleRankText == null) return;

            currentStyleRank = rank;
            styleRankText.text = rank;

            // Renk değişimi
            int rankIndex = GetRankIndex(rank);
            if (rankIndex < rankColors.Length)
            {
                styleRankText.color = rankColors[rankIndex];
            }

            // Rank yükseldiğinde animation
            #if DOTWEEN_ENABLED
            if (useAdvancedAnimations)
            {
                styleRankText.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 5);
                styleRankText.transform.DOShakeRotation(0.2f, 15f);
            }
            #endif

            // Container'ı göster
            if (!styleRankContainer.activeSelf)
            {
                styleRankContainer.SetActive(true);
            }
        }

        private int GetRankIndex(string rank)
        {
            return rank switch
            {
                "D" => 0,
                "C" => 1,
                "B" => 2,
                "A" => 3,
                "S" => 4,
                "SS" => 5,
                "SSS" => 6,
                _ => 0
            };
        }
        #endregion

        #region Screen Effects
        /// <summary>
        /// Ekranı sarsma (damage alındığında).
        /// </summary>
        public void ShakeHUD(float intensity = 0.3f, float duration = 0.2f)
        {
            #if DOTWEEN_ENABLED
            if (useAdvancedAnimations)
            {
                transform.DOShakePosition(duration, intensity * 10f, 10);
            }
            #endif
        }

        /// <summary>
        /// Glitch efekti (critical hit'te).
        /// </summary>
        public void GlitchEffect()
        {
            // Basit glitch: Tüm UI'yı bir an kaydır
            #if DOTWEEN_ENABLED
            if (useAdvancedAnimations)
            {
                transform.DOLocalMoveX(Random.Range(-5f, 5f), 0.05f).OnComplete(() =>
                {
                    transform.DOLocalMoveX(0, 0.05f);
                });
            }
            #endif
        }
        #endregion

        #region Utility
        /// <summary>
        /// DOTween olmadan fill amount lerp.
        /// </summary>
        private System.Collections.IEnumerator LerpFillAmount(Image image, float targetFill, float duration)
        {
            float startFill = image.fillAmount;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                image.fillAmount = Mathf.Lerp(startFill, targetFill, t);
                yield return null;
            }

            image.fillAmount = targetFill;
        }
        #endregion
    }
}

