using UnityEngine;
using UnityEngine.InputSystem;

public class CursorFollower : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform cursorObject;
    [SerializeField] private Vector2 offset = Vector2.zero;
    [SerializeField] private float smoothFactor = 0.9f;
    [SerializeField] private float rotationSpeed = 5f; // Speed of rotation
    [SerializeField] private float rotationAngle = 15f; // Max rotation angle in degrees

    private Vector2 targetPosition;
    private Vector2 previousPosition;
    private RectTransform canvasTransform;
    private float currentRotation = 0f;
    private Vector2 lastMousePosition;

    private void Start()
    {
        Cursor.visible = false;
        canvasTransform = canvas.GetComponent<RectTransform>();
        previousPosition = cursorObject.anchoredPosition;
    }

    private void Update()
    {
        // Get mouse position
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        // Convert to canvas space
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasTransform,
            mousePosition,
            canvas.worldCamera,
            out Vector2 localMousePosition
        ); 
        
        targetPosition = lastMousePosition + ((localMousePosition + offset) - lastMousePosition) * smoothFactor;

        // Calculate horizontal movement direction
        float horizontalDelta = targetPosition.x - previousPosition.x;

        // Calculate target rotation based on horizontal movement
        float targetRotation = 0f;
        if (Mathf.Abs(horizontalDelta) > 0.1f) // Threshold to avoid jitter
        {
            // Positive delta = moving right, negative = moving left
            targetRotation = Mathf.Clamp(horizontalDelta * -rotationAngle, -rotationAngle, rotationAngle);
        }

        // Smoothly interpolate rotation
        currentRotation = Mathf.Lerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Apply rotation (rotate around Z axis)
        cursorObject.localRotation = Quaternion.Euler(0f, 0f, -currentRotation);

        // Update position
        previousPosition = cursorObject.anchoredPosition;
        cursorObject.anchoredPosition = targetPosition;
        
        lastMousePosition = targetPosition;
    }

    private void OnDestroy()
    {
        Cursor.visible = true;
    }
}