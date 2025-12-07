using UnityEngine;
using HordeInTown.UI;

namespace HordeInTown.Player
{
    /// <summary>
    /// Controls the bow aiming and shooting mechanics
    /// </summary>
    public class BowController : MonoBehaviour
    {
        [Header("Bow Settings")]
        [SerializeField] private Transform bowTransform;
        [SerializeField] private Transform arrowSpawnPoint;
        [SerializeField] private GameObject arrowPrefab;
        [Tooltip("Rotation offset to make bow vertical (Euler angles). Adjust X, Y, Z to fine-tune bow orientation.")]
        [SerializeField] private Vector3 verticalRotationOffset = new Vector3(90f, 0f, 0f);
        
        [Header("Aiming")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private LayerMask aimLayerMask = -1; // Default to "Everything" if not set
        [SerializeField] private float maxAimDistance = 100f;
        [SerializeField] private bool debugAiming = false; // Show debug line for aiming
        [SerializeField] private float debugCircleRadius = 0.5f; // Radius of debug circle at ground intersection
        
        [Header("Shooting")]
        [SerializeField] private float shootCooldown = 5f; // 5 seconds as per spec
        [SerializeField] private float minArrowDamage = 10f;
        [SerializeField] private float maxArrowDamage = 30f;
        [SerializeField] private float arrowSpeed = 15f; // Arrow launch speed (reduced from 20)
        
        [Header("Mobile Controls")]
        [SerializeField] private bool useJoystick = true;
        [SerializeField] private Joystick aimJoystick;
        [SerializeField] private UnityEngine.UI.Button attackButton;
        
        [Header("UI")]
        [SerializeField] private HordeInTown.UI.CrosshairController crosshairController; // Crosshair controller
        [SerializeField] private UnityEngine.UI.Image cooldownIndicator; // Cooldown visual
        
        private float lastShootTime;
        private Vector3 aimDirection;
        private bool canShoot = true;
        
        // Events
        public System.Action<float> OnCooldownChanged; // 0-1 progress
        
        private void Start()
        {
            // Setup attack button
            if (attackButton != null)
            {
                attackButton.onClick.AddListener(OnAttackButtonPressed);
            }
        }
        
        private void Update()
        {
            HandleAiming();
            UpdateCooldown();
        }
        
        private void HandleAiming()
        {
            Vector3 aimPoint;
            
            if (useJoystick && crosshairController != null)
            {
                // Use crosshair controller to get aim direction
                // Crosshair moves with joystick, bow aims at crosshair position
                Vector3 spawnPos = arrowSpawnPoint != null ? arrowSpawnPoint.position : transform.position;
                aimPoint = crosshairController.GetWorldAimPoint(
                    spawnPos,
                    maxAimDistance,
                    aimLayerMask
                );
                aimDirection = (aimPoint - spawnPos).normalized;
                
                // Debug visualization
                if (debugAiming)
                {
                    Debug.DrawLine(spawnPos, aimPoint, Color.red, 0.1f);
                    Debug.DrawRay(spawnPos, aimDirection * 10f, Color.green, 0.1f);
                    DrawDebugCircle(aimPoint, debugCircleRadius, Color.yellow);
                }
            }
            else if (useJoystick && aimJoystick != null)
            {
                // Fallback: Use joystick directly if crosshair controller not assigned
                Vector2 joystickInput = aimJoystick.Direction;
                if (joystickInput.magnitude > 0.1f)
                {
                    aimDirection = new Vector3(joystickInput.x, 0, joystickInput.y).normalized;
                    aimPoint = transform.position + aimDirection * maxAimDistance;
                }
                else
                {
                    // Default forward direction
                    aimDirection = transform.forward;
                    aimPoint = transform.position + aimDirection * maxAimDistance;
                }
            }
            else
            {
                // Use camera raycast for aiming (for testing on PC)
                Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, maxAimDistance, aimLayerMask))
                {
                    aimPoint = hit.point;
                    aimDirection = (aimPoint - arrowSpawnPoint.position).normalized;
                }
                else
                {
                    aimPoint = ray.GetPoint(maxAimDistance);
                    aimDirection = (aimPoint - arrowSpawnPoint.position).normalized;
                }
            }
            
            // Rotate bow to face aim direction (keep bow vertical)
            if (bowTransform != null && aimDirection != Vector3.zero)
            {
                // Calculate horizontal aim direction (ignore Y component for horizontal rotation)
                Vector3 horizontalAim = new Vector3(aimDirection.x, 0, aimDirection.z).normalized;
                
                if (horizontalAim != Vector3.zero)
                {
                    // Rotate bow to face horizontal aim direction, but keep it vertical
                    // First, get the rotation to face the horizontal direction
                    Quaternion horizontalRotation = Quaternion.LookRotation(horizontalAim);
                    
                    // Apply configurable vertical rotation offset (set in Inspector)
                    Quaternion verticalOffset = Quaternion.Euler(verticalRotationOffset);
                    
                    // Combine rotations: first rotate to face direction, then apply vertical offset
                    Quaternion targetRotation = horizontalRotation * verticalOffset;
                    
                    bowTransform.rotation = Quaternion.Slerp(bowTransform.rotation, targetRotation, Time.deltaTime * 10f);
                }
            }
        }
        
        private void OnAttackButtonPressed()
        {
            HordeInTown.Managers.AudioManager.Instance.PlaySFX("bow_loading");
            if (canShoot)
            {
                ShootArrow();
            }
        }
        
        private void UpdateCooldown()
        {
            float timeSinceLastShot = Time.time - lastShootTime;
            float cooldownProgress = Mathf.Clamp01(timeSinceLastShot / shootCooldown);
            canShoot = cooldownProgress >= 1f;
            
            // Update cooldown indicator
            if (cooldownIndicator != null)
            {
                cooldownIndicator.fillAmount = 1f - cooldownProgress;
            }
            
            OnCooldownChanged?.Invoke(cooldownProgress);
        }
        
        private void ShootArrow()
        {
            if (arrowPrefab == null || arrowSpawnPoint == null)
            {
                Debug.LogWarning("Arrow prefab or spawn point not assigned!");
                return;
            }
            
            if (!canShoot) return;
            
            // Instantiate arrow with random damage
            GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.LookRotation(aimDirection));
            
            // Set arrow damage (random between 10-30)
            var arrowScript = arrow.GetComponent<HordeInTown.Combat.Arrow>();
            if (arrowScript != null)
            {
                float randomDamage = Random.Range(minArrowDamage, maxArrowDamage);
                arrowScript.SetDamage(randomDamage);
            }
            
            // Apply force to arrow
            Rigidbody arrowRb = arrow.GetComponent<Rigidbody>();
            if (arrowRb != null)
            {
                // Set velocity directly for more control, or use lower force
                arrowRb.linearVelocity = aimDirection * arrowSpeed;
            }
            
            lastShootTime = Time.time;
            canShoot = false;
            
            // Play shoot sound effect
            if (HordeInTown.Managers.AudioManager.Instance != null)
            {
                HordeInTown.Managers.AudioManager.Instance.PlaySFX("arrow_shot");
            }
        }
        
        /// <summary>
        /// Get current aim direction
        /// </summary>
        public Vector3 GetAimDirection()
        {
            return aimDirection;
        }
        
        /// <summary>
        /// Check if bow can shoot (cooldown ready)
        /// </summary>
        public bool CanShoot()
        {
            return canShoot;
        }
        
        /// <summary>
        /// Get cooldown progress (0-1)
        /// </summary>
        public float GetCooldownProgress()
        {
            float timeSinceLastShot = Time.time - lastShootTime;
            return Mathf.Clamp01(timeSinceLastShot / shootCooldown);
        }
        
        /// <summary>
        /// Draws a debug circle at the specified position (visible in Scene view)
        /// </summary>
        private void DrawDebugCircle(Vector3 center, float radius, Color color)
        {
            int segments = 32;
            float angleStep = 360f / segments;
            
            Vector3 previousPoint = center + new Vector3(radius, 0, 0);
            
            for (int i = 1; i <= segments; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector3 currentPoint = center + new Vector3(
                    Mathf.Cos(angle) * radius,
                    0,
                    Mathf.Sin(angle) * radius
                );
                
                Debug.DrawLine(previousPoint, currentPoint, color, 0.1f);
                previousPoint = currentPoint;
            }
            
            // Draw center point
            Debug.DrawRay(center, Vector3.up * 0.2f, color, 0.1f);
            Debug.DrawRay(center, Vector3.down * 0.2f, color, 0.1f);
        }
    }
}

