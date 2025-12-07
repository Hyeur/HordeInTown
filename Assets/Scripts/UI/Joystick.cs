using UnityEngine;
using UnityEngine.EventSystems;

namespace HordeInTown.UI
{
    /// <summary>
    /// Simple virtual joystick for mobile aiming
    /// </summary>
    public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Header("Joystick Settings")]
        [SerializeField] private RectTransform joystickBackground;
        [SerializeField] private RectTransform joystickHandle;
        [SerializeField] private float joystickRange = 50f;
        
        private Vector2 direction = Vector2.zero;
        private bool isPressed = false;
        
        public Vector2 Direction => direction;
        public bool IsPressed => isPressed;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            isPressed = true;
            OnDrag(eventData);
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            isPressed = false;
            direction = Vector2.zero;
            
            if (joystickHandle != null && joystickBackground != null)
            {
                joystickHandle.anchoredPosition = Vector2.zero;
            }
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (joystickBackground == null || joystickHandle == null) return;
            
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                joystickBackground,
                eventData.position,
                eventData.pressEventCamera,
                out localPoint
            );
            
            // Clamp to joystick range
            direction = Vector2.ClampMagnitude(localPoint, joystickRange);
            direction /= joystickRange; // Normalize to -1 to 1
            
            // Move handle
            joystickHandle.anchoredPosition = direction * joystickRange;
        }
    }
}

