using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSensitivity = 0.5f;
    [SerializeField] private float rotationSmoothing = 0.15f;
    [SerializeField] private float minVerticalAngle = 0f;
    [SerializeField] private float maxVerticalAngle = 90f;

    [Header("Zoom Settings")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float zoomSensitivity = 0.1f;
    [SerializeField] private float zoomSmoothing = 0.15f;
    [SerializeField] private float minZoom = 20f;
    [SerializeField] private float maxZoom = 80f;

    private InputSystem_Actions inputActions;
    private Vector2 lookDelta;
    private float scrollDelta;

    private Vector3 currentRotation;
    private Vector3 targetRotation;
    private float currentZoom;
    private float targetZoom;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        
        // Subscribe to Look action (mouse delta when right-click is held)
        inputActions.Player.RotateCamera.performed += OnLook;
        inputActions.Player.RotateCamera.canceled += OnLook;
        
        // Subscribe to Zoom action (scroll wheel)
        inputActions.Player.Zoom.performed += OnZoom;
        inputActions.Player.Zoom.canceled += OnZoom;
    }

    private void OnDisable()
    {
        inputActions.Player.RotateCamera.performed -= OnLook;
        inputActions.Player.RotateCamera.canceled -= OnLook;
        inputActions.Player.Zoom.performed -= OnZoom;
        inputActions.Player.Zoom.canceled -= OnZoom;
        
        inputActions.Player.Disable();
    }

    private void Start()
    {
        // Initialize rotation to current transform rotation
        currentRotation = transform.localEulerAngles;
        targetRotation = currentRotation;
        

        currentZoom = minZoom;
        targetZoom = currentZoom;
    }

    private void Update()
    {
        HandleRotation();
        HandleZoom();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        lookDelta = context.ReadValue<Vector2>();
    }

    private void OnZoom(InputAction.CallbackContext context)
    {
        // Mouse scroll returns a Vector2, we only need the Y component (vertical scroll)
        scrollDelta = context.ReadValue<float>();
    }

    private void HandleRotation()
    {
        if (lookDelta != Vector2.zero)
        {
            // Calculate rotation deltas
            float yawDelta = lookDelta.x * rotationSensitivity;
            float pitchDelta = -lookDelta.y * rotationSensitivity;

            // Apply rotation to target
            // Y rotation is global (world space)
            targetRotation.y += yawDelta;
            
            // X rotation is local and clamped
            targetRotation.x += pitchDelta;
            targetRotation.x = Mathf.Clamp(targetRotation.x, minVerticalAngle, maxVerticalAngle);
        }

        // Smoothly interpolate current rotation towards target
        currentRotation.x = Mathf.Lerp(currentRotation.x, targetRotation.x, rotationSmoothing);
        currentRotation.y = Mathf.Lerp(currentRotation.y, targetRotation.y, rotationSmoothing);

        // Apply rotation: local X, global Y
        transform.rotation = Quaternion.Euler(0, currentRotation.y, 0) * Quaternion.Euler(currentRotation.x, 0, 0);
    }

    private void HandleZoom()
    {
        if (targetCamera == null) return;

        if (scrollDelta != 0)
        {
            // Adjust target zoom based on scroll input
            targetZoom += scrollDelta * zoomSensitivity;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            
            // Reset scroll delta after processing
            scrollDelta = 0;
        }

        currentZoom = Mathf.Lerp(currentZoom, targetZoom, zoomSmoothing);
        Vector3 newPosition = targetCamera.transform.localPosition;
        newPosition.z = currentZoom;
        targetCamera.transform.localPosition = newPosition;
    }

    private void OnDestroy()
    {
        inputActions?.Dispose();
    }
}
