using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using NeonSyndicate.Core;
#if DOTWEEN_ENABLED
using DG.Tweening;
#endif

namespace NeonSyndicate.UI
{
    /// <summary>
    /// Main Menu UI - Ana menü kontrolcüsü.
    /// 
    /// Crazy Flasher tarzı:
    /// - Yaşayan arka plan (animated scene)
    /// - Reaktif butonlar (hover effects)
    /// - Neon glow efektleri
    /// - Metalik ses feedback
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Menu Buttons")]
        [SerializeField] private Button storyModeButton;
        [SerializeField] private Button survivalModeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;

        [Header("Title")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Image titleGlow;

        [Header("Background Character")]
        [SerializeField] private GameObject axelCharacter;
        [SerializeField] private Animator axelAnimator;

        [Header("Settings")]
        [SerializeField] private bool animateOnStart = true;

        private void Start()
        {
            InitializeMenu();
            
            if (animateOnStart)
            {
                PlayIntroAnimation();
            }
        }

        #region Initialization
        private void InitializeMenu()
        {
            // Button listeners
            if (storyModeButton != null)
            {
                storyModeButton.onClick.AddListener(OnStoryModeClick);
                AddHoverEffects(storyModeButton);
            }

            if (survivalModeButton != null)
            {
                survivalModeButton.onClick.AddListener(OnSurvivalModeClick);
                AddHoverEffects(survivalModeButton);
                
                // Kilitli (placeholder)
                survivalModeButton.interactable = false;
            }

            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(OnSettingsClick);
                AddHoverEffects(settingsButton);
            }

            if (exitButton != null)
            {
                exitButton.onClick.AddListener(OnExitClick);
                AddHoverEffects(exitButton);
            }

            // Müzik
            SoundManager.Instance?.PlayMusic("MainMenu_Theme");
        }

        private void AddHoverEffects(Button button)
        {
            // EventTrigger ile hover effects ekle
            var trigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();

            // Pointer Enter
            var enterEntry = new UnityEngine.EventSystems.EventTrigger.Entry
            {
                eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter
            };
            enterEntry.callback.AddListener((data) => OnButtonHover(button));
            trigger.triggers.Add(enterEntry);

            // Pointer Exit
            var exitEntry = new UnityEngine.EventSystems.EventTrigger.Entry
            {
                eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit
            };
            exitEntry.callback.AddListener((data) => OnButtonExit(button));
            trigger.triggers.Add(exitEntry);
        }
        #endregion

        #region Button Callbacks
        private void OnStoryModeClick()
        {
            SoundManager.Instance?.PlaySFX("Menu_Select");
            
            #if DOTWEEN_ENABLED
            // Fade out animasyonu
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.DOFade(0, 0.5f).OnComplete(() =>
                {
                    SceneManager.LoadScene("Level_01_Slums");
                });
            }
            else
            #endif
            {
                SceneManager.LoadScene("Level_01_Slums");
            }
        }

        private void OnSurvivalModeClick()
        {
            SoundManager.Instance?.PlaySFX("Menu_Select");
            Debug.Log("Survival Mode - Coming Soon!");
        }

        private void OnSettingsClick()
        {
            SoundManager.Instance?.PlaySFX("Menu_Click");
            // Settings panel aç
            Debug.Log("Settings Menu - TODO");
        }

        private void OnExitClick()
        {
            SoundManager.Instance?.PlaySFX("Menu_Click");
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
        #endregion

        #region Hover Effects
        private void OnButtonHover(Button button)
        {
            // Ses efekti (metalik sıyrılma)
            SoundManager.Instance?.PlaySFX("Menu_Hover");

            #if DOTWEEN_ENABLED
            // Scale up
            button.transform.DOScale(Vector3.one * 1.1f, 0.2f).SetEase(Ease.OutQuad);
            
            // Color shift (beyaz → kırmızı)
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.DOColor(Color.red, 0.2f);
            }

            // Axel karakteri yüzünü döner
            if (axelAnimator != null)
            {
                axelAnimator.SetTrigger("LookAt");
            }
            #endif
        }

        private void OnButtonExit(Button button)
        {
            #if DOTWEEN_ENABLED
            // Scale back
            button.transform.DOScale(Vector3.one, 0.2f);
            
            // Color back
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.DOColor(Color.white, 0.2f);
            }
            #endif
        }
        #endregion

        #region Intro Animation
        private void PlayIntroAnimation()
        {
            #if DOTWEEN_ENABLED
            // Title fade in + glow pulse
            if (titleText != null)
            {
                titleText.alpha = 0;
                titleText.DOFade(1f, 1.5f).SetEase(Ease.OutQuad);
            }

            if (titleGlow != null)
            {
                titleGlow.DOFade(0.3f, 1.5f).SetLoops(-1, LoopType.Yoyo);
            }

            // Buttons slide in
            var buttons = new[] { storyModeButton, survivalModeButton, settingsButton, exitButton };
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i] != null)
                {
                    buttons[i].transform.localPosition = new Vector3(200, buttons[i].transform.localPosition.y, 0);
                    buttons[i].transform.DOLocalMoveX(0, 0.5f + i * 0.1f).SetEase(Ease.OutBack);
                }
            }
            #endif
        }
        #endregion
    }
}

