using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HordeInTown.Managers;
using HordeInTown.Zombies;

namespace HordeInTown.UI
{
    /// <summary>
    /// Manages the main menu UI and transitions
    /// </summary>
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Main Menu UI")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private Button startButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        
        [Header("Loading Screen")]
        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private Slider loadingProgressBar;
        [SerializeField] private TextMeshProUGUI loadingText;
        [SerializeField] private float fakeLoadingDuration = 2f; // Duration of fake loading

        [Header("Tutorial Panel")]
        [SerializeField] private Button tutorialButton;
        
        private void Start()
        {
            // Setup buttons
            if (startButton != null)
            {
                startButton.onClick.AddListener(OnStartButtonClicked);
            }
            
            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            }
            
            if (quitButton != null)
            {
                quitButton.onClick.AddListener(OnQuitButtonClicked);
            }
            
            // Show main menu, hide loading screen
            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(true);
            }
            
            if (loadingPanel != null)
            {
                loadingPanel.SetActive(false);
            }

            if (tutorialButton != null)
            {
                tutorialButton.onClick.AddListener(ShowTutorial);
            }
            
            // Pause the game while main menu is active
            PauseGame();
            
            // Play main menu BGM
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayBGM("mainmenu");
            }
        }
        
        private void ShowTutorial()
        {
            // Play button sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX("button_press");
            }

            if (TutorialPanelManager.Instance != null)
            {
                TutorialPanelManager.Instance.ShowTutorial();
            }
        }
        private void OnStartButtonClicked()
        {
            // Play button sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX("button_press");
            }
            
            // Hide main menu and show loading screen
            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(false);
            }
            
            if (loadingPanel != null)
            {
                loadingPanel.SetActive(true);
            }
            
            // Start fake loading
            StartCoroutine(FakeLoadingAndStartGame());
        }
        
        private void OnSettingsButtonClicked()
        {
            // Play button sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX("button_press");
            }
            
            // Show settings panel
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ToggleSettingsPanel();
            }
            else
            {
                // Fallback: try to find UIManager if instance not set
                UIManager uiManager = FindFirstObjectByType<UIManager>();
                if (uiManager != null)
                {
                    uiManager.ToggleSettingsPanel();
                }
                else
                {
                    Debug.LogWarning("MainMenuManager: UIManager not found! Cannot open settings panel.");
                }
            }
        }
        
        private void OnQuitButtonClicked()
        {
            // Play button sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX("button_press");
            }
            
            // Quit application
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
        private System.Collections.IEnumerator FakeLoadingAndStartGame()
        {
            float elapsedTime = 0f;
            
            // Reset loading bar
            if (loadingProgressBar != null)
            {
                loadingProgressBar.value = 0f;
            }
            
            if (loadingText != null)
            {
                loadingText.text = "Loading...";
            }
            
            // Fake loading progress (use unscaled time so it works while paused)
            while (elapsedTime < fakeLoadingDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float progress = elapsedTime / fakeLoadingDuration;
                
                // Update loading bar
                if (loadingProgressBar != null)
                {
                    loadingProgressBar.value = progress;
                }
                
                // Update loading text
                if (loadingText != null)
                {
                    int percent = Mathf.RoundToInt(progress * 100f);
                    loadingText.text = $"Loading... {percent}%";
                }
                
                yield return null;
            }
            
            // Ensure 100%
            if (loadingProgressBar != null)
            {
                loadingProgressBar.value = 1f;
            }
            
            if (loadingText != null)
            {
                loadingText.text = "Loading... 100%";
            }
            
            // Wait a brief moment at 100% (use unscaled time so it works while paused)
            yield return new WaitForSecondsRealtime(0.2f);
            
            // Hide loading screen
            if (loadingPanel != null)
            {
                loadingPanel.SetActive(false);
            }
            
            // Deactivate main menu panel
            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(false);
            }
            
            // Unpause the game
            ResumeGame();
            
            // Start the game
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartGame();
            }
            
            // Restart zombie spawning
            ZombieSpawner spawner = FindFirstObjectByType<ZombieSpawner>();
            if (spawner != null)
            {
                spawner.StartSpawning();
            }
        }
        
        private void PauseGame()
        {
            Time.timeScale = 0f;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PauseGame();
            }
        }
        
        private void ResumeGame()
        {
            Time.timeScale = 1f;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResumeGame();
            }
        }
        
        /// <summary>
        /// Show main menu panel (called from UIManager when returning to main menu)
        /// </summary>
        public void ShowMainMenu()
        {
            // Show main menu panel
            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(true);
            }
            
            // Hide loading screen
            if (loadingPanel != null)
            {
                loadingPanel.SetActive(false);
            }
            
            // Pause the game
            PauseGame();
        }
        
        /// <summary>
        /// Hide main menu panel (called when opening settings)
        /// </summary>
        public void HideMainMenu()
        {
            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(false);
            }
        }
    }
}

