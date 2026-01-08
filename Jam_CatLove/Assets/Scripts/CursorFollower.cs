using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorFollower : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform cursorObject;
    [SerializeField] private Vector2 offset = Vector2.zero;
    [SerializeField] private float smoothFactor = 0.9f;
    [SerializeField] private float rotationSpeed = 5f; // Speed of rotation
    [SerializeField] private float rotationAngle = 15f; // Max rotation angle in degrees
    
    [Header("Cursor Sprites")]
    [SerializeField] private Sprite cursorOpenSprite; // Cursor sprite when mouse button is not pressed
    [SerializeField] private Sprite cursorClosedSprite; // Cursor sprite when mouse button is pressed

    private Vector2 targetPosition;
    private Vector2 previousPosition;
    private RectTransform canvasTransform;
    private float currentRotation = 0f;
    private Vector2 lastMousePosition;
    private Image cursorImage;

    private void Start()
    {
        Cursor.visible = false;
        canvasTransform = canvas.GetComponent<RectTransform>();
        previousPosition = cursorObject.anchoredPosition;
        
        // Get the Image component from the cursor object
        cursorImage = cursorObject.GetComponent<Image>();
        
        // Set initial cursor sprite to open
        if (cursorImage != null && cursorOpenSprite != null)
        {
            cursorImage.sprite = cursorOpenSprite;
        }
    }

    private void Update()
    {
        // Check for mouse button press/release and switch cursor sprite
        if (Mouse.current != null && cursorImage != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame && cursorClosedSprite != null)
            {
                cursorImage.sprite = cursorClosedSprite;
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame && cursorOpenSprite != null)
            {
                cursorImage.sprite = cursorOpenSprite;
            }
        }
        
        Vector2 mousePosition = Mouse.current.position.ReadValue();
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