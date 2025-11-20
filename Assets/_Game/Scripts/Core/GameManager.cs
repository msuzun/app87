using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace NeonSyndicate.Core
{
    /// <summary>
    /// Oyunun genel akışını yöneten singleton manager.
    /// Pause, Resume, GameOver gibi temel oyun durumlarını kontrol eder.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        [SerializeField] private bool isPaused = false;
        [SerializeField] private bool isGameOver = false;

        [Header("Score & Economy")]
        [SerializeField] private int currentScore = 0;
        [SerializeField] private int money = 0;
        [SerializeField] private float styleMultiplier = 1f;

        // Events
        public delegate void GameStateChanged(bool paused);
        public static event GameStateChanged OnGamePaused;

        public delegate void ScoreChanged(int newScore);
        public static event ScoreChanged OnScoreUpdated;

        #region Properties
        public bool IsPaused => isPaused;
        public bool IsGameOver => isGameOver;
        public int CurrentScore => currentScore;
        public int Money => money;
        public float StyleMultiplier => styleMultiplier;
        #endregion

        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            // ESC tuşu ile pause (NEW Input System)
            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
            {
                TogglePause();
            }
        }

        #region Game Flow Control
        public void TogglePause()
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0f : 1f;
            OnGamePaused?.Invoke(isPaused);
        }

        public void PauseGame()
        {
            if (!isPaused)
            {
                isPaused = true;
                Time.timeScale = 0f;
                OnGamePaused?.Invoke(true);
            }
        }

        public void ResumeGame()
        {
            if (isPaused)
            {
                isPaused = false;
                Time.timeScale = 1f;
                OnGamePaused?.Invoke(false);
            }
        }

        public void GameOver()
        {
            isGameOver = true;
            Time.timeScale = 0f;
            Debug.Log("GAME OVER - Score: " + currentScore);
            // UI Manager'a event gönder
        }

        public void RestartLevel()
        {
            Time.timeScale = 1f;
            isGameOver = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void LoadNextLevel()
        {
            Time.timeScale = 1f;
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.Log("No more levels! Victory!");
                // Victory screen'e geç
            }
        }
        #endregion

        #region Score & Money System
        public void AddScore(int points, float styleBonus = 1f)
        {
            int finalPoints = Mathf.RoundToInt(points * styleBonus * styleMultiplier);
            currentScore += finalPoints;
            OnScoreUpdated?.Invoke(currentScore);
        }

        public void AddMoney(int amount)
        {
            money += amount;
        }

        public bool SpendMoney(int amount)
        {
            if (money >= amount)
            {
                money -= amount;
                return true;
            }
            return false;
        }

        public void SetStyleMultiplier(float multiplier)
        {
            styleMultiplier = Mathf.Clamp(multiplier, 1f, 3f);
        }
        #endregion
    }
}

