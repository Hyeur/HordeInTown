using UnityEngine;

namespace HordeInTown.Managers
{
    /// <summary>
    /// Manages game resolution settings
    /// </summary>
    public class ResolutionManager : MonoBehaviour
    {
        [Header("Resolution Settings")]
        [SerializeField] private int targetWidth = 1280;
        [SerializeField] private int targetHeight = 720;
        [SerializeField] private bool setOnStart = true;
        [SerializeField] private bool fullscreen = false;
        
        private void Start()
        {
            if (setOnStart)
            {
                SetResolution(targetWidth, targetHeight, fullscreen);
            }
        }
        
        /// <summary>
        /// Set the game resolution
        /// </summary>
        public void SetResolution(int width, int height, bool isFullscreen)
        {
            Screen.SetResolution(width, height, isFullscreen);
            Debug.Log($"Resolution set to: {width}x{height}, Fullscreen: {isFullscreen}");
        }
        
        /// <summary>
        /// Set resolution to 1280x720
        /// </summary>
        public void SetResolution1280x720(bool isFullscreen = false)
        {
            SetResolution(1280, 720, isFullscreen);
        }
        
        /// <summary>
        /// Get current resolution
        /// </summary>
        public Vector2Int GetCurrentResolution()
        {
            return new Vector2Int(Screen.width, Screen.height);
        }
    }
}

