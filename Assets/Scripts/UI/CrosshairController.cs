using UnityEngine;
using UnityEngine.UI;

namespace HordeInTown.UI
{
    /// <summary>
    /// Controls crosshair movement based on joystick input
    /// </summary>
    public class CrosshairController : MonoBehaviour
    {
        [Header("Crosshair Settings")]
        [SerializeField] private RectTransform crosshairRect;
        [SerializeField] private float maxCrosshairDistanceX = 200f; // Max horizontal distance from center in pixels
        [SerializeField] private float maxCrosshairDistanceY = 200f; // Max vertical distance from center in pixels
        [SerializeField] private float crosshairSpeed = 500f; // Movement speed
        
        [Header("Joystick")]
        [SerializeField] private Joystick aimJoystick;
        
        private Vector2 crosshairPosition;
        private Canvas parentCanvas;
        private Camera mainCamera;
        
        private void Start()
        {
            if (crosshairRect == null)
            {
                crosshairRect = GetComponent<RectTransform>();
            }
            
            // Find parent canvas
            parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas == null)
            {
                parentCanvas = FindFirstObjectByType<Canvas>();
            }
            
            // Find main camera
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                mainCamera = FindFirstObjectByType<Camera>();
            }
            
            // Initialize crosshair at screen center
            crosshairPosition = Vector2.zero;
            if (crosshairRect != null)
            {
                crosshairRect.anchoredPosition = Vector2.zero;
            }
        }
        
        private void Update()
        {
            UpdateCrosshairPosition();
        }
        
        private void UpdateCrosshairPosition()
        {
            if (crosshairRect == null || aimJoystick == null) return;
            
            // Get joystick input
            Vector2 joystickInput = aimJoystick.Direction;
            
            // Move crosshair based on joystick input
            if (joystickInput.magnitude > 0.1f)
            {
                // Calculate new position
                crosshairPosition += joystickInput * crosshairSpeed * Time.deltaTime;
                
                // Clamp to rectangular bounds (separate X and Y limits)
                crosshairPosition.x = Mathf.Clamp(crosshairPosition.x, -maxCrosshairDistanceX, maxCrosshairDistanceX);
                crosshairPosition.y = Mathf.Clamp(crosshairPosition.y, -maxCrosshairDistanceY, maxCrosshairDistanceY);
            }
            else
            {
                // Return to center when joystick is released (optional - remove if you want crosshair to stay)
                // crosshairPosition = Vector2.Lerp(crosshairPosition, Vector2.zero, Time.deltaTime * 5f);
            }
            
            // Update crosshair position
            crosshairRect.anchoredPosition = crosshairPosition;
        }
        
        /// <summary>
        /// Get the world position that the crosshair is aiming at
        /// </summary>
        public Vector3 GetWorldAimPoint(Vector3 fromPosition, float maxDistance = 100f, LayerMask layerMask = default)
        {
            if (crosshairRect == null || parentCanvas == null)
            {
                return fromPosition + Vector3.forward * maxDistance;
            }
            
            // Get the camera to use
            Camera cam = mainCamera;
            if (parentCanvas.renderMode == RenderMode.ScreenSpaceCamera && parentCanvas.worldCamera != null)
            {
                cam = parentCanvas.worldCamera;
            }
            
            if (cam == null)
            {
                cam = Camera.main ?? FindFirstObjectByType<Camera>();
                if (cam == null)
                {
                    return fromPosition + Vector3.forward * maxDistance;
                }
            }
            
            // Convert crosshair screen position to world ray
            Vector2 screenPoint = GetCrosshairScreenPosition();
            Ray ray = cam.ScreenPointToRay(screenPoint);
            
            // Raycast to find world position
            if (layerMask.value != 0 && Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask))
            {
                return hit.point;
            }
            else if (layerMask.value == 0 && Physics.Raycast(ray, out RaycastHit hitAll, maxDistance))
            {
                // If no layer mask specified, raycast against everything
                return hitAll.point;
            }
            else
            {
                // Return point at max distance along ray
                return ray.GetPoint(maxDistance);
            }
        }
        
        /// <summary>
        /// Get the screen position of the crosshair
        /// </summary>
        public Vector2 GetCrosshairScreenPosition()
        {
            if (crosshairRect == null || parentCanvas == null)
            {
                return new Vector2(Screen.width / 2f, Screen.height / 2f);
            }
            
            // Convert UI position to screen position
            if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                // For ScreenSpaceOverlay, the position is already in screen coordinates
                return crosshairRect.position;
            }
            else if (parentCanvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                // For ScreenSpaceCamera, convert world position to screen
                if (parentCanvas.worldCamera != null)
                {
                    return RectTransformUtility.WorldToScreenPoint(parentCanvas.worldCamera, crosshairRect.position);
                }
                else if (mainCamera != null)
                {
                    return RectTransformUtility.WorldToScreenPoint(mainCamera, crosshairRect.position);
                }
            }
            else if (parentCanvas.renderMode == RenderMode.WorldSpace)
            {
                // For WorldSpace, use the camera to convert
                Camera cam = parentCanvas.worldCamera ?? mainCamera ?? Camera.main;
                if (cam != null)
                {
                    return RectTransformUtility.WorldToScreenPoint(cam, crosshairRect.position);
                }
            }
            
            // Fallback to screen center
            return new Vector2(Screen.width / 2f, Screen.height / 2f);
        }
        
        /// <summary>
        /// Get the aim direction from a position to the crosshair's world position
        /// </summary>
        public Vector3 GetAimDirection(Vector3 fromPosition, float maxDistance = 100f, LayerMask layerMask = default)
        {
            Vector3 aimPoint = GetWorldAimPoint(fromPosition, maxDistance, layerMask);
            return (aimPoint - fromPosition).normalized;
        }
        
        /// <summary>
        /// Reset crosshair to center
        /// </summary>
        public void ResetToCenter()
        {
            crosshairPosition = Vector2.zero;
            if (crosshairRect != null)
            {
                crosshairRect.anchoredPosition = Vector2.zero;
            }
        }
    }
}

