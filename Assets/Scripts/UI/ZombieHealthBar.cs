using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HordeInTown.Zombies;

namespace HordeInTown.UI
{
    /// <summary>
    /// Health bar that displays above a zombie's head
    /// </summary>
    public class ZombieHealthBar : MonoBehaviour
    {
        [Header("Health Bar Settings")]
        [SerializeField] private Slider healthBar;
        [SerializeField] private Image healthBarFill;
        [SerializeField] private Canvas healthBarCanvas;
        [SerializeField] private float offsetY = 2.5f; // Height above zombie head
        [SerializeField] private bool alwaysFaceCamera = true;
        
        [Header("Colors")]
        [SerializeField] private Color healthyColor = new Color(0f, 1f, 0f, 1f); // Bright green
        [SerializeField] private Color mediumHealthColor = new Color(1f, 1f, 0f, 1f); // Bright yellow
        [SerializeField] private Color lowHealthColor = new Color(1f, 0f, 0f, 1f); // Bright red
        
        [Header("Optional")]
        [SerializeField] private TextMeshProUGUI healthText; // Optional health number display
        
        private ZombieController zombieController;
        private Camera mainCamera;
        private float maxHealth;    
        
        private void Start()
        {
            // Find zombie controller
            zombieController = GetComponentInParent<ZombieController>();
            if (zombieController == null)
            {
                zombieController = GetComponent<ZombieController>();
            }
            
            if (zombieController == null)
            {
                Debug.LogWarning("ZombieHealthBar: No ZombieController found!");
                enabled = false;
                return;
            }
            
            // Get max health from zombie (actual random health value, not the range)
            maxHealth = zombieController.GetMaxHealth();
            
            // Ensure health bar slider is properly configured
            if (healthBar != null)
            {
                healthBar.minValue = 0f; // Always start from 0
                healthBar.maxValue = maxHealth; // Use zombie's actual max health
            }
            
            // Find main camera
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                mainCamera = FindFirstObjectByType<Camera>();
            }
            
            // Setup canvas if not assigned
            if (healthBarCanvas == null)
            {
                healthBarCanvas = GetComponentInParent<Canvas>();
                if (healthBarCanvas == null)
                {
                    healthBarCanvas = GetComponent<Canvas>();
                }
            }
            
            // Position health bar above zombie
            UpdatePosition();
            
            // Initialize health bar (hidden at full health)
            UpdateHealthBar(maxHealth);
            Hide(); // Start hidden since at full health
        }
        
        private void Update()
        {
            if (zombieController == null) return;
            
            // Update position to stay above zombie head
            UpdatePosition();
            
            // Face camera if enabled
            if (alwaysFaceCamera && mainCamera != null && healthBarCanvas != null)
            {
                healthBarCanvas.transform.LookAt(healthBarCanvas.transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
            }
        }
        
        private void UpdatePosition()
        {
            if (zombieController == null) return;
            
            Transform zombieTransform = zombieController.transform;
            
            // Position health bar above zombie's head
            Vector3 zombiePosition = zombieTransform.position;
            
            // Try to find head bone or use offset
            Vector3 headPosition = zombiePosition + Vector3.up * offsetY;
            
            // Check if zombie has animator with head bone
            var animator = zombieController.GetComponent<Animator>();
            if (animator != null && animator.isHuman)
            {
                try
                {
                    Transform headBone = animator.GetBoneTransform(HumanBodyBones.Head);
                    if (headBone != null)
                    {
                        headPosition = headBone.position + Vector3.up * 0.3f; // Slightly above head
                    }
                }
                catch
                {
                    // Head bone not found, use offset
                }
            }
            
            transform.position = headPosition;
        }
        
        /// <summary>
        /// Update health bar display
        /// </summary>
        public void UpdateHealthBar(float health)
        {
            // Only show health bar if not at full health
            bool isFullHealth = health >= maxHealth;
            if (isFullHealth)
            {
                Hide();
            }
            else
            {
                Show();
            }
            
            // Update slider
            if (healthBar != null)
            {
                healthBar.value = health;
            }
            
            // Update fill color (bright green → yellow → red)
            if (healthBarFill != null)
            {
                float healthPercent = health / maxHealth;
                
                // Green: 80-100%, Yellow: 30-79%, Red: 0-29%
                if (healthPercent >= 0.8f)
                {
                    // Green (80% to 100%)
                    healthBarFill.color = healthyColor;
                }
                else if (healthPercent >= 0.3f)
                {
                    // Yellow (30% to 79%) - ensure it's bright yellow
                    healthBarFill.color = mediumHealthColor;
                }
                else
                {
                    // Red (0% to 29%)
                    healthBarFill.color = lowHealthColor;
                }
            }
            
            // Update health text if available
            if (healthText != null)
            {
                healthText.text = $"{Mathf.CeilToInt(health)}/{Mathf.CeilToInt(maxHealth)}";
            }
        }
        
        /// <summary>
        /// Set max health (called by zombie)
        /// </summary>
        public void SetMaxHealth(float max)
        {
            maxHealth = max;
            if (healthBar != null)
            {
                healthBar.minValue = 0f;
                healthBar.maxValue = max;
            }
        }
        
        /// <summary>
        /// Hide health bar
        /// </summary>
        public void Hide()
        {
            if (healthBarCanvas != null)
            {
                healthBarCanvas.gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// Show health bar
        /// </summary>
        public void Show()
        {
            if (healthBarCanvas != null)
            {
                healthBarCanvas.gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }
    }
}

