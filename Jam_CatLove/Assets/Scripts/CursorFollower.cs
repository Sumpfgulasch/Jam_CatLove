using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorFollower : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform cursorObject;
    [SerializeField] private Vector2 offset = Vector2.zero;
    [SerializeField] private float movementSmoothing = 0.15f;
    [SerializeField] private float rotationAccelerationgDamping = 0.3f;
    [SerializeField] private float rotationDecelerationgDamping = 0.1f;
    [SerializeField] private float maxRotationSpeed = 5f; // Speed of rotation
    [SerializeField] private float maxRotationAngle = 15f; // Max rotation angle in degrees

    [Header("Cursor Sprites")] [SerializeField]
    private Sprite cursorOpenSprite; // Cursor sprite when mouse button is not pressed

    [SerializeField] private Sprite cursorClosedSprite; // Cursor sprite when mouse button is pressed

    private Vector2 targetPosition;
    private Vector2 previousPosition;
    private RectTransform canvasTransform;
    private float currentRotation = 0f;
    private Image cursorImage;
    private float currentHorizontalSpeed;
    private float lastHorizontalSpeed;

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
        // Cursor sprites
        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                cursorImage.sprite = cursorClosedSprite;
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                cursorImage.sprite = cursorOpenSprite;
            }
        }

        // Position
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasTransform,
            mousePosition,
            canvas.worldCamera,
            out Vector2 localMousePosition
        );
        Vector2 targetMousePosition = localMousePosition + offset;
        targetPosition = Vector2.Lerp(previousPosition, targetMousePosition, movementSmoothing);
        cursorObject.anchoredPosition = targetPosition;
        var currentSpeed = (targetPosition - previousPosition).magnitude;

        // Rotation
        var horizontalDelta = targetPosition.x - previousPosition.x;
        var targetRotation = 0f;
        if (Mathf.Abs(horizontalDelta) > 0.1f)
        {
            targetRotation = Mathf.Clamp(horizontalDelta * -maxRotationAngle, -maxRotationAngle, maxRotationAngle);
        }
        currentRotation = Mathf.Lerp(currentRotation, targetRotation,
            horizontalDelta > 0 ? rotationAccelerationgDamping : rotationDecelerationgDamping);
        cursorObject.localRotation = Quaternion.Euler(0f, 0f, -currentRotation);

        // Misc
        lastHorizontalSpeed = currentSpeed;
        previousPosition = cursorObject.anchoredPosition;
    }

    private void OnDestroy()
    {
        Cursor.visible = true;
    }
}