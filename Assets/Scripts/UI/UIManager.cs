using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HordeInTown.Managers;
using HordeInTown.Player;
using HordeInTown.Zombies;

// Extension for Image to update fill amount by slider value
public static class UIExtensions
{
    public static void SyncFillToSlider(Image image, Slider slider)
    {
        if (image != null && slider != null)
        {
            image.fillAmount = (slider.maxValue > slider.minValue) 
                ? (slider.value - slider.minValue) / (slider.maxValue - slider.minValue)
                : 0f;
        }
    }
}

namespace HordeInTown.UI
{
    /// <summary>
    /// Manages all UI elements: health bar, score, game over screen, etc.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Health UI")]
        [SerializeField] private Slider healthBar;
        [SerializeField] private Image healthBarFill;
        [SerializeField] private Color healthyColor = Color.green;
        [SerializeField] private Color lowHealthColor = Color.red;
        
        [Header("Score UI")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI survivalTimeText;
        
        [Header("Aiming UI")]
        [SerializeField] private RectTransform crosshair; // Crosshair at screen center
        [SerializeField] private Image cooldownIndicator; // Cooldown visual (fill image)
        
        [Header("Game Over UI")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private TextMeshProUGUI finalTimeText;
        [SerializeField] private TextMeshProUGUI finalWaveClearedText; // Display waves cleared
        [SerializeField] private TextMeshProUGUI finalZombieKillsText; // Display zombie kills
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;
        
        [Header("In-Game Wave Display")]
        [SerializeField] private TextMeshProUGUI wavesClearedText; // Display current waves cleared during gameplay
        
        private int finalWaveCleared = 0; // Store final waves cleared
        private int finalZombieKills = 0; // Store final zombie kills
        
        [Header("Pause/Settings UI")]
        [SerializeField] private GameObject pauseSettingsPanel; // Combined pause and settings panel
        [SerializeField] private Button pauseButton; // Button to pause/open settings
        [SerializeField] private Button resumeButton; // Button to resume/close settings
        [SerializeField] private Button mainMenuButton; // Button to return to main menu
        [SerializeField] private Button settingsButton; // Button in main menu to open settings
        [SerializeField] private Button musicToggleButton; // Can also use MusicToggleButton
        [SerializeField] private Button sfxToggleButton; // Can also use SFXToggleButton
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private UnityEngine.UI.Image musicToggleONIcon;
        [SerializeField] private UnityEngine.UI.Image musicToggleOFFIcon;
        [SerializeField] private UnityEngine.UI.Image sfxToggleONIcon;
        [SerializeField] private UnityEngine.UI.Image sfxToggleOFFIcon;
        
        public static UIManager Instance { get; private set; }
        
        private void Awake()
        {
            // Set instance early in Awake
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }
        
        private void Start()
        {
            
            // Subscribe to GameManager events
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnHealthChanged += UpdateHealthBar;
                GameManager.Instance.OnScoreChanged += UpdateScore;
                GameManager.Instance.OnGameOver += ShowGameOverScreen;
                GameManager.Instance.OnGameStart += HideGameOverScreen;
            }
            
            // Setup buttons
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(RestartGame);
            }
            
            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(ResumeGame);
            }
            
            if (pauseButton != null)
            {
                pauseButton.onClick.AddListener(PauseGame);
            }
            
            if (menuButton != null)
            {
                menuButton.onClick.AddListener(ReturnToMainMenu);
            }
            
            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(ReturnToMainMenu);
            }
            
            // Setup settings UI
            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            }
            
            if (musicToggleButton != null)
            {
                musicToggleButton.onClick.AddListener(ToggleMusic);
            }
            
            if (sfxToggleButton != null)
            {
                sfxToggleButton.onClick.AddListener(ToggleSFX);
            }
            
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
                // Initialize slider value
                if (AudioManager.Instance != null)
                {
                    musicVolumeSlider.value = AudioManager.Instance.GetBGMVolume();
                }
            }
            
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
                // Initialize slider value
                if (AudioManager.Instance != null)
                {
                    sfxVolumeSlider.value = AudioManager.Instance.GetSFXVolume();
                }
            }
            
            // Initialize UI
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
            
            if (pauseSettingsPanel != null)
            {
                pauseSettingsPanel.SetActive(false);
            }

            if (healthBar != null && healthBarFill != null)
            {
                UIExtensions.SyncFillToSlider(healthBarFill, healthBar);
            }
            
            // Update toggle icons
            UpdateToggleIcons();
        }
        
        private void Update()
        {
            // Update survival time display
            if (survivalTimeText != null && GameManager.Instance != null)
            {
                float time = GameManager.Instance.GetSurvivalTime();
                survivalTimeText.text = $"{time:F1}";
            }
            
            // Update waves cleared display
            UpdateWavesCleared();
            
            // Update cooldown indicator
            UpdateCooldownIndicator();
        }
        
        private void UpdateWavesCleared()
        {
            if (wavesClearedText != null)
            {
                ZombieSpawner spawner = FindFirstObjectByType<ZombieSpawner>();
                if (spawner != null)
                {
                    int waves = spawner.GetWavesCleared();
                    wavesClearedText.text = $"{waves}";
                }
            }
        }
        
        private void UpdateCooldownIndicator()
        {
            if (cooldownIndicator != null)
            {
                // Find BowController to get cooldown progress
                var bowController = FindFirstObjectByType<BowController>();
                if (bowController != null)
                {
                    float cooldownProgress = bowController.GetCooldownProgress();
                    cooldownIndicator.fillAmount = 1f - cooldownProgress; // Fill from 0 to 1 as cooldown completes
                }
            }
        }
        
        private void UpdateHealthBar(float healthPercent)
        {
            if (healthBar != null)
            {
                healthBar.value = healthPercent;
                UIExtensions.SyncFillToSlider(healthBarFill, healthBar);
            }
            
            // Change color based on health
            if (healthBarFill != null)
            {
                healthBarFill.color = Color.LerpUnclamped(lowHealthColor, healthyColor, healthPercent);
            }
        }
        
        private void UpdateScore(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"{score}";
            }
        }
        
        private void ShowGameOverScreen()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
            
            // Get final waves cleared
            ZombieSpawner spawner = FindFirstObjectByType<ZombieSpawner>();
            if (spawner != null)
            {
                finalWaveCleared = spawner.GetWavesCleared();
            }
            
            // Get final zombie kills
            if (GameManager.Instance != null)
            {
                finalZombieKills = GameManager.Instance.GetZombieKillCount();
            }
            
            if (finalScoreText != null && GameManager.Instance != null)
            {
                finalScoreText.text = $"{GameManager.Instance.GetScore()}";
            }
            
            if (finalTimeText != null && GameManager.Instance != null)
            {
                float time = GameManager.Instance.GetSurvivalTime();
                finalTimeText.text = $"{time:F1}s";
            }
            
            if (finalWaveClearedText != null)
            {
                finalWaveClearedText.text = $"{finalWaveCleared}";
            }
            
            if (finalZombieKillsText != null)
            {
                finalZombieKillsText.text = $"{finalZombieKills}";
            }
        }
        
        private void HideGameOverScreen()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
        }
        
        private void PauseGame()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PauseGame();
            }
            
            // Show pause/settings panel
            if (pauseSettingsPanel != null)
            {
                pauseSettingsPanel.SetActive(true);
            }
        }
        
        private void ResumeGame()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResumeGame();
            }
            
            // Hide pause/settings panel
            if (pauseSettingsPanel != null)
            {
                pauseSettingsPanel.SetActive(false);
            }
        }
        
        private void RestartGame()
        {
            // Play button sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX("button_press");
            }
            
            // Stop spawning and destroy all zombies
            ZombieSpawner spawner = FindFirstObjectByType<ZombieSpawner>();
            if (spawner != null)
            {
                spawner.StopSpawning();
                spawner.DestroyAllZombies();
            }
            
            // Reset game state
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResetGame();
            }
            
            // Hide game over panel
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
            
            // Hide pause/settings panel
            if (pauseSettingsPanel != null)
            {
                pauseSettingsPanel.SetActive(false);
            }
            
            // Resume the game
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResumeGame();
            }
            
            // Start the game
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartGame();
            }
            
            // Restart zombie spawning
            if (spawner != null)
            {
                spawner.StartSpawning();
            }
        }
        
        private void ReturnToMainMenu()
        {
            // Play button sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX("button_press");
            }
            
            // Stop spawning and destroy all zombies
            ZombieSpawner spawner = FindFirstObjectByType<ZombieSpawner>();
            if (spawner != null)
            {
                spawner.StopSpawning();
                spawner.DestroyAllZombies();
            }
            
            // Reset game state
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResetGame();
            }
            
            // Hide game UI panels
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
            
            if (pauseSettingsPanel != null)
            {
                pauseSettingsPanel.SetActive(false);
            }
            
            // Find MainMenuManager and show main menu panel
            MainMenuManager mainMenuManager = FindFirstObjectByType<MainMenuManager>();
            if (mainMenuManager != null)
            {
                mainMenuManager.ShowMainMenu();
            }
            
            // // Play main menu BGM
            // if (AudioManager.Instance != null)
            // {
            //     AudioManager.Instance.PlayBGM("mainmenu");
            // }
        }
        
        private void OnSettingsButtonClicked()
        {
            // Play button sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX("button_press");
            }
            
            if (pauseSettingsPanel != null)
            {
                bool isActive = pauseSettingsPanel.activeSelf;
                pauseSettingsPanel.SetActive(!isActive);
                
                if (!isActive) // Opening settings
                {
                    // Pause game when opening settings
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.PauseGame();
                    }
                    
                    // Hide main menu panel when opening settings
                    MainMenuManager mainMenuManager = FindFirstObjectByType<MainMenuManager>();
                    if (mainMenuManager != null)
                    {
                        mainMenuManager.HideMainMenu();
                    }
                }
                else // Closing settings (back button behavior)
                {
                    // Check if we're in main menu (game paused and not started)
                    bool isInMainMenu = false;
                    if (GameManager.Instance != null)
                    {
                        isInMainMenu = GameManager.Instance.IsGamePaused() && !GameManager.Instance.IsGameStarted();
                    }
                    
                    // If in main menu, show main menu panel and keep paused
                    if (isInMainMenu)
                    {
                        MainMenuManager mainMenuManager = FindFirstObjectByType<MainMenuManager>();
                        if (mainMenuManager != null)
                        {
                            mainMenuManager.ShowMainMenu();
                        }
                    }
                    else
                    {
                        // If during gameplay, resume game
                        if (GameManager.Instance != null)
                        {
                            GameManager.Instance.ResumeGame();
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning("UIManager: Pause/Settings panel is not assigned in the Inspector!");
            }
        }
        
        public void ToggleSettingsPanel()
        {
            // Public method for external calls (e.g., from MainMenuManager)
            OnSettingsButtonClicked();
        }
        
        private void ToggleMusic()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.ToggleBGM();
                UpdateToggleIcons();
                
                // Play button sound
                AudioManager.Instance.PlaySFX("button_press");
            }
        }
        
        private void ToggleSFX()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.ToggleSFX();
                UpdateToggleIcons();
                
                // Play button sound (but it won't play if SFX is muted)
                // So we temporarily unmute, play, then restore state
                bool wasMuted = AudioManager.Instance.IsSFXMuted();
                if (wasMuted)
                {
                    // Temporarily unmute to play the toggle sound
                    AudioManager.Instance.ToggleSFX();
                    AudioManager.Instance.PlaySFX("button_press");
                    AudioManager.Instance.ToggleSFX(); // Restore muted state
                }
                else
                {
                    AudioManager.Instance.PlaySFX("button_press");
                }
            }
        }
        
        private void OnMusicVolumeChanged(float value)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetBGMVolume(value);
            }
        }
        
        private void OnSFXVolumeChanged(float value)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetSFXVolume(value);
            }
        }
        
        private void UpdateToggleIcons()
        {
            if (AudioManager.Instance == null) return;
            
            // Update music toggle icon (you can customize this based on your UI design)
            if (musicToggleONIcon != null)
            {
                // You can change color, sprite, or visibility based on mute state
                musicToggleONIcon.gameObject.SetActive(AudioManager.Instance.IsBGMMuted() ? true : false);
                musicToggleOFFIcon.gameObject.SetActive(AudioManager.Instance.IsBGMMuted() ? false : true);
            }
            
            // Update SFX toggle icon
            if (sfxToggleONIcon != null)
            {
                sfxToggleONIcon.gameObject.SetActive(AudioManager.Instance.IsSFXMuted() ? true : false);
                sfxToggleOFFIcon.gameObject.SetActive(AudioManager.Instance.IsSFXMuted() ? false : true);
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnHealthChanged -= UpdateHealthBar;
                GameManager.Instance.OnScoreChanged -= UpdateScore;
                GameManager.Instance.OnGameOver -= ShowGameOverScreen;
                GameManager.Instance.OnGameStart -= HideGameOverScreen;
            }
        }
    }
}

