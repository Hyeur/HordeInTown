using UnityEngine;
using HordeInTown.UI;

namespace HordeInTown.Managers
{
    /// <summary>
    /// Main game manager - handles game state, score, and player health
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [Header("Player Stats")]
        [SerializeField] private float maxPlayerHealth = 100f;
        [SerializeField] private float currentPlayerHealth;
        [SerializeField] private float healPer10Kills = 10f; // Heal 10 HP every 10 kills
        
        [Header("Game Settings")]
        [SerializeField] private bool gameStarted = false;
        [SerializeField] private bool gamePaused = false;
        
        private int currentScore = 0;
        private int zombieKillCount = 0;
        private float survivalTime = 0f;
        
        // Events
        public System.Action<float> OnHealthChanged;
        public System.Action<int> OnScoreChanged;
        public System.Action OnGameOver;
        public System.Action OnGameStart;
        public System.Action OnPlayerHealed;
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            // Set game resolution to 1280x720
            Screen.SetResolution(1280, 720, false);
            
            currentPlayerHealth = maxPlayerHealth;
        }
        
        private void Update()
        {
            if (gameStarted && !gamePaused)
            {
                survivalTime += Time.deltaTime;
            }
        }
        
        public void StartGame()
        {
            gameStarted = true;
            gamePaused = false;
            currentScore = 0;
            survivalTime = 0f;
            zombieKillCount = 0; // Reset zombie kill count
            currentPlayerHealth = maxPlayerHealth;
            
            OnGameStart?.Invoke();
        }
        
        public void PauseGame()
        {
            gamePaused = true;
            Time.timeScale = 0f;
        }
        
        public void ResumeGame()
        {
            gamePaused = false;
            Time.timeScale = 1f;
        }
        
        public void AddScore(int points)
        {
            currentScore += points;
            OnScoreChanged?.Invoke(currentScore);
        }
        
        /// <summary>
        /// Called when a zombie is killed - handles healing every 10 kills
        /// </summary>
        public void OnZombieKilled()
        {
            zombieKillCount++;
            
            // Heal player every 10 kills
            if (zombieKillCount % 10 == 0)
            {
                HealPlayer(healPer10Kills);
            }
        }
        
        private void HealPlayer(float healAmount)
        {
            currentPlayerHealth = Mathf.Min(maxPlayerHealth, currentPlayerHealth + healAmount);
            OnHealthChanged?.Invoke(currentPlayerHealth / maxPlayerHealth);
            OnPlayerHealed?.Invoke();
        }
        
        public void PlayerTakeDamage(float damage)
        {
            currentPlayerHealth -= damage;
            currentPlayerHealth = Mathf.Max(0, currentPlayerHealth);
            
            OnHealthChanged?.Invoke(currentPlayerHealth / maxPlayerHealth);
            
            if (currentPlayerHealth <= 0)
            {
                GameOver();
            }
        }
        
        private void GameOver()
        {
            gameStarted = false;
            gamePaused = false;
            Time.timeScale = 1f;
            
            OnGameOver?.Invoke();
            
            Debug.Log($"Game Over! Final Score: {currentScore}, Survival Time: {survivalTime:F1}s");
        }
        
        /// <summary>
        /// Reset game state (called when returning to main menu)
        /// </summary>
        public void ResetGame()
        {
            gameStarted = false;
            gamePaused = false;
            currentScore = 0;
            survivalTime = 0f;
            zombieKillCount = 0;
            currentPlayerHealth = maxPlayerHealth;
            Time.timeScale = 1f;
            
            // Notify UI of reset
            OnHealthChanged?.Invoke(1f); // Full health
            OnScoreChanged?.Invoke(0); // Zero score
        }
        
        // Getters
        public int GetScore() => currentScore;
        public float GetSurvivalTime() => survivalTime;
        public float GetHealthPercent() => currentPlayerHealth / maxPlayerHealth;
        public bool IsGameStarted() => gameStarted;
        public bool IsGamePaused() => gamePaused;
        public int GetZombieKillCount() => zombieKillCount;
    }
}

