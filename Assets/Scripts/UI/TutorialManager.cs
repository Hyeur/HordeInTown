using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HordeInTown.UI
{
    /// <summary>
    /// Manages tutorial steps for new players
    /// </summary>
    public class TutorialManager : MonoBehaviour
    {
        [Header("Tutorial UI")]
        [SerializeField] private GameObject tutorialPanel;
        [SerializeField] private TextMeshProUGUI tutorialText;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button skipButton;
        
        [Header("Tutorial Steps")]
        [SerializeField] private string[] tutorialSteps = new string[]
        {
            "Objective: Defend against zombies using your bow!",
            "Aim: Use the virtual joystick to aim your bow.",
            "Shoot: Tap the attack button to fire arrows (5s cooldown).",
            "Healing: Every 10 zombie kills, you'll heal +10 HP!"
        };
        
        private int currentStep = 0;
        private bool tutorialCompleted = false;
        
        private void Start()
        {
            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(true);
            }
            
            if (nextButton != null)
            {
                nextButton.onClick.AddListener(NextStep);
            }
            
            if (skipButton != null)
            {
                skipButton.onClick.AddListener(SkipTutorial);
            }
            
            ShowCurrentStep();
        }
        
        private void ShowCurrentStep()
        {
            if (currentStep < tutorialSteps.Length && tutorialText != null)
            {
                tutorialText.text = tutorialSteps[currentStep];
            }
            
            // Hide next button on last step
            if (nextButton != null)
            {
                nextButton.gameObject.SetActive(currentStep < tutorialSteps.Length - 1);
            }
        }
        
        private void NextStep()
        {
            currentStep++;
            
            if (currentStep >= tutorialSteps.Length)
            {
                CompleteTutorial();
            }
            else
            {
                ShowCurrentStep();
            }
        }
        
        private void SkipTutorial()
        {
            CompleteTutorial();
        }
        
        private void CompleteTutorial()
        {
            tutorialCompleted = true;
            
            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(false);
            }
            
            // Start the game
            if (HordeInTown.Managers.GameManager.Instance != null)
            {
                HordeInTown.Managers.GameManager.Instance.StartGame();
            }
        }
        
        public bool IsTutorialCompleted()
        {
            return tutorialCompleted;
        }
    }
}

