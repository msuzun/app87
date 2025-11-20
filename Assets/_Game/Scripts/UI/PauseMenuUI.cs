using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using NeonSyndicate.Core;
#if DOTWEEN_ENABLED
using DG.Tweening;
#endif

namespace NeonSyndicate.UI
{
    /// <summary>
    /// Pause Menu UI - Oyun durakladığında açılan menü.
    /// 
    /// Crazy Flasher tarzı:
    /// - VHS glitch efekti (oyun donarken)
    /// - Scanline efekti
    /// - Siyah-beyaz filtre (opsiyonel)
    /// - Metalik buton sesleri
    /// </summary>
    public class PauseMenuUI : MonoBehaviour
    {
        [Header("Menu Panels")]
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject moveListPanel;

        [Header("Buttons")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button moveListButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        [Header("Effects")]
        [SerializeField] private Image glitchOverlay;
        [SerializeField] private Image scanlineOverlay;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Move List")]
        [SerializeField] private TextMeshProUGUI moveListText;

        private bool isPaused = false;

        private void Awake()
        {
            // Başlangıçta gizli
            if (pausePanel != null) 
                pausePanel.SetActive(false);
            
            if (moveListPanel != null) 
                moveListPanel.SetActive(false);
        }

        private void Start()
        {
            InitializeButtons();
            
            // GameManager event'ine abone ol
            GameManager.OnGamePaused += HandlePauseStateChanged;
        }

        private void OnDestroy()
        {
            GameManager.OnGamePaused -= HandlePauseStateChanged;
        }

        private void Update()
        {
            // ESC tuşu (NEW Input System)
            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
            {
                TogglePause();
            }
        }

        #region Initialization
        private void InitializeButtons()
        {
            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(OnResumeClick);
            }

            if (moveListButton != null)
            {
                moveListButton.onClick.AddListener(OnMoveListClick);
            }

            if (restartButton != null)
            {
                restartButton.onClick.AddListener(OnRestartClick);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(OnMainMenuClick);
            }
        }
        #endregion

        #region Pause Control
        public void TogglePause()
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        private void Pause()
        {
            isPaused = true;
            Time.timeScale = 0f;

            if (pausePanel != null)
            {
                pausePanel.SetActive(true);
            }

            // Glitch efekti
            PlayGlitchEffect();

            // Ses
            SoundManager.Instance?.PlaySFX("Menu_Open");
        }

        private void Resume()
        {
            isPaused = false;
            Time.timeScale = 1f;

            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
            }

            if (moveListPanel != null)
            {
                moveListPanel.SetActive(false);
            }

            // Ses
            SoundManager.Instance?.PlaySFX("Menu_Close");
        }

        private void HandlePauseStateChanged(bool paused)
        {
            isPaused = paused;
            
            if (pausePanel != null)
            {
                pausePanel.SetActive(paused);
            }
        }
        #endregion

        #region Button Callbacks
        private void OnResumeClick()
        {
            SoundManager.Instance?.PlaySFX("Menu_Select");
            Resume();
        }

        private void OnMoveListClick()
        {
            SoundManager.Instance?.PlaySFX("Menu_Click");
            
            if (moveListPanel != null)
            {
                bool isActive = moveListPanel.activeSelf;
                moveListPanel.SetActive(!isActive);

                if (!isActive)
                {
                    DisplayMoveList();
                }
            }
        }

        private void OnRestartClick()
        {
            SoundManager.Instance?.PlaySFX("Menu_Select");
            
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void OnMainMenuClick()
        {
            SoundManager.Instance?.PlaySFX("Menu_Select");
            
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
        #endregion

        #region Move List
        private void DisplayMoveList()
        {
            if (moveListText == null) return;

            string moveList = @"
╔══════════════════════════════════╗
║   AXEL - MOVE LIST               ║
╚══════════════════════════════════╝

BASIC COMBOS
────────────
Z → Z → Z           3-Hit Combo
Z → Z → X           Launcher
X                   Heavy Attack

AIR COMBOS
──────────
(Launcher) → Space → Z    Air Combo
Space → Z → Z             Aerial Assault

SPECIAL
───────
C                   Grab
Shift               Dodge (I-Frame)
Shift (Hold)        Sprint

ADVANCED
────────
Z → Z → Shift       Dash Cancel
X → Wall            Wall Bounce
            ";

            moveListText.text = moveList;
        }
        #endregion

        #region Visual Effects
        private void PlayGlitchEffect()
        {
            #if DOTWEEN_ENABLED
            if (glitchOverlay != null)
            {
                // VHS glitch (flicker)
                glitchOverlay.gameObject.SetActive(true);
                glitchOverlay.DOFade(0.3f, 0.1f).OnComplete(() =>
                {
                    glitchOverlay.DOFade(0f, 0.1f);
                });
            }

            // Scanline efekti
            if (scanlineOverlay != null)
            {
                scanlineOverlay.gameObject.SetActive(true);
            }

            // Panel fade in
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
                canvasGroup.DOFade(1f, 0.3f).SetUpdate(true); // Unscaled time
            }
            #endif
        }
        #endregion
    }
}

