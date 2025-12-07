using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HordeInTown.UI
{
    /// <summary>
    /// Manages tutorial panel with multiple pages and navigation
    /// </summary>
    public class TutorialPanelManager : MonoBehaviour
    {
        [Header("Tutorial Panel")]
        [SerializeField] private GameObject tutorialPanel;
        [SerializeField] private GameObject[] tutorialPages; // Array of 4 fullscreen pages
        [SerializeField] private Button leftArrowButton; // Previous page button
        [SerializeField] private Button rightArrowButton; // Next page button
        [SerializeField] private Button skipButton; // Optional skip/close button
        [SerializeField] private TextMeshProUGUI pageIndicatorText; // Optional page number display (e.g., "1/4")
        
        [Header("Settings")]
        [SerializeField] private bool autoStartOnGameStart = true; // Show tutorial when game starts
        [SerializeField] private bool pauseGameDuringTutorial = true; // Pause game while tutorial is showing
        
        private int currentPageIndex = 0;
        private bool isTutorialActive = false;

        public static TutorialPanelManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }
        private void Start()
        {
            // Setup buttons
            if (leftArrowButton != null)
            {
                leftArrowButton.onClick.AddListener(PreviousPage);
            }
            
            if (rightArrowButton != null)
            {
                rightArrowButton.onClick.AddListener(NextPage);
            }
            
            if (skipButton != null)
            {
                skipButton.onClick.AddListener(CloseTutorial);
            }
            
            // Initialize tutorial panel
            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(false);
            }
            
            // Hide all pages initially
            HideAllPages();
            
            // Auto-start tutorial if enabled
            if (autoStartOnGameStart)
            {
                // Subscribe to game start event
                if (HordeInTown.Managers.GameManager.Instance != null)
                {
                    HordeInTown.Managers.GameManager.Instance.OnGameStart += ShowTutorial;
                }
            }
        }
        
        /// <summary>
        /// Show tutorial panel and start from first page
        /// </summary>
        public void ShowTutorial()
        {
            if (tutorialPanel == null || tutorialPages == null || tutorialPages.Length == 0)
            {
                Debug.LogWarning("TutorialPanelManager: Tutorial panel or pages not assigned!");
                return;
            }
            
            isTutorialActive = true;
            currentPageIndex = 0;
            
            // Show tutorial panel
            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(true);
            }
            
            // Ensure buttons are enabled and active
            if (leftArrowButton != null)
            {
                leftArrowButton.gameObject.SetActive(true);
                leftArrowButton.interactable = true;
            }
            
            if (rightArrowButton != null)
            {
                rightArrowButton.gameObject.SetActive(true);
                rightArrowButton.interactable = true;
            }
            
            if (skipButton != null)
            {
                skipButton.gameObject.SetActive(true);
                skipButton.interactable = true;
            }
            
            // Pause game during tutorial
            if (pauseGameDuringTutorial && HordeInTown.Managers.GameManager.Instance != null)
            {
                HordeInTown.Managers.GameManager.Instance.PauseGame();
            }
            
            // Show first page
            ShowPage(0);
            
            // Play button sound
            if (HordeInTown.Managers.AudioManager.Instance != null)
            {
                HordeInTown.Managers.AudioManager.Instance.PlaySFX("button_press");
            }
        }
        
        /// <summary>
        /// Close tutorial panel
        /// </summary>
        public void CloseTutorial()
        {
            if (!isTutorialActive) return;
            
            isTutorialActive = false;
            
            // Hide tutorial panel
            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(false);
            }
            
            // Hide all pages
            HideAllPages();
            
            // Resume game if it was paused
            if (pauseGameDuringTutorial && HordeInTown.Managers.GameManager.Instance != null)
            {
                // Only resume if game was actually started (not in main menu)
                if (HordeInTown.Managers.GameManager.Instance.IsGameStarted())
                {
                    HordeInTown.Managers.GameManager.Instance.ResumeGame();
                }
            }
            
            // Play button sound
            if (HordeInTown.Managers.AudioManager.Instance != null)
            {
                HordeInTown.Managers.AudioManager.Instance.PlaySFX("button_press");
            }
        }
        
        /// <summary>
        /// Go to next page
        /// </summary>
        public void NextPage()
        {
            if (!isTutorialActive) return;
            
            if (currentPageIndex < tutorialPages.Length - 1)
            {
                currentPageIndex++;
                ShowPage(currentPageIndex);
                
                // Play button sound
                if (HordeInTown.Managers.AudioManager.Instance != null)
                {
                    HordeInTown.Managers.AudioManager.Instance.PlaySFX("button_press");
                }
            }
            else
            {
                // On last page, next button closes tutorial
                CloseTutorial();
            }
        }
        
        /// <summary>
        /// Go to previous page
        /// </summary>
        public void PreviousPage()
        {
            if (!isTutorialActive) return;
            
            if (currentPageIndex > 0)
            {
                currentPageIndex--;
                ShowPage(currentPageIndex);
                
                // Play button sound
                if (HordeInTown.Managers.AudioManager.Instance != null)
                {
                    HordeInTown.Managers.AudioManager.Instance.PlaySFX("button_press");
                }
            }
        }
        
        /// <summary>
        /// Show specific page and hide others
        /// </summary>
        private void ShowPage(int pageIndex)
        {
            if (tutorialPages == null || pageIndex < 0 || pageIndex >= tutorialPages.Length)
            {
                return;
            }
            
            // Hide all pages
            HideAllPages();
            
            // Show current page
            if (tutorialPages[pageIndex] != null)
            {
                tutorialPages[pageIndex].SetActive(true);
            }
            
            // Update navigation buttons
            UpdateNavigationButtons();
            
            // Update page indicator
            UpdatePageIndicator();
        }
        
        /// <summary>
        /// Hide all tutorial pages
        /// </summary>
        private void HideAllPages()
        {
            if (tutorialPages == null) return;
            
            foreach (GameObject page in tutorialPages)
            {
                if (page != null)
                {
                    page.SetActive(false);
                }
            }
        }
        
        /// <summary>
        /// Update navigation button states
        /// </summary>
        private void UpdateNavigationButtons()
        {
            // Left arrow: disabled on first page
            if (leftArrowButton != null)
            {
                bool canGoBack = currentPageIndex > 0;
                leftArrowButton.interactable = canGoBack;
                // Visual feedback: you can also change button color/alpha if needed
            }
            
            // Right arrow: always enabled (on last page it closes tutorial)
            if (rightArrowButton != null)
            {
                rightArrowButton.interactable = true;
            }
            
            // Ensure buttons are visible
            if (leftArrowButton != null)
            {
                leftArrowButton.gameObject.SetActive(true);
            }
            
            if (rightArrowButton != null)
            {
                rightArrowButton.gameObject.SetActive(true);
            }
        }
        
        /// <summary>
        /// Update page indicator text (e.g., "1/4", "2/4")
        /// </summary>
        private void UpdatePageIndicator()
        {
            if (pageIndicatorText != null && tutorialPages != null)
            {
                pageIndicatorText.text = $"{currentPageIndex + 1}/{tutorialPages.Length}";
            }
        }
        
        /// <summary>
        /// Check if tutorial is currently active
        /// </summary>
        public bool IsTutorialActive()
        {
            return isTutorialActive;
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (HordeInTown.Managers.GameManager.Instance != null)
            {
                HordeInTown.Managers.GameManager.Instance.OnGameStart -= ShowTutorial;
            }
        }
    }
}

