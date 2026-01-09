using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class CatInteractionDetector : MonoBehaviour
{
    public delegate void CatPetted(string zone, float speed, Vector2 cursorPosition);

    public event CatPetted OnCatPetted;

    [SerializeField] private InputActionReference moveCursorAction;
    [SerializeField] private InputActionReference interactAction;

    private string hitLayer;

    private InputSystem_Actions inputActions;
    private Vector2 cursorPosition = new();
    private bool isInteractionButtonHold = false;
    private Vector2 previousCursorPosition = new();
    private float screenDiagonal;
    private bool isCatHover = false;


    void Awake()
    {
        inputActions = new InputSystem_Actions();
        screenDiagonal = Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height);
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.MoveCursor.performed += OnMoveCursor;
        inputActions.Player.Interact.started += OnInteract;
        inputActions.Player.Interact.canceled += OnInteract;
    }


    private void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.Player.MoveCursor.performed -= OnMoveCursor;
        inputActions.Player.Interact.started -= OnInteract;
        inputActions.Player.Interact.canceled -= OnInteract;
    }

    private void OnMoveCursor(InputAction.CallbackContext context)
    {
        cursorPosition = context.ReadValue<Vector2>(); // in pixels

        // // Check hits
        isCatHover = IsCatHover(cursorPosition, out hitLayer);
        if (!isCatHover)
        {
            return;
        }

        // Invoke event
        if (isInteractionButtonHold)
        {
            var speed = (((cursorPosition - previousCursorPosition) / Time.deltaTime).magnitude) / screenDiagonal;
            OnCatPetted?.Invoke(hitLayer, speed, cursorPosition);
            Debug.Log($"layer: {hitLayer}, speed: {speed}");
        }

        previousCursorPosition = cursorPosition;
    }

    private bool IsCatHover(Vector2 cursorPos, out string hitCatLayer)
    {
        // Check hits
        var hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(cursorPos));
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        if (hits.Length == 0 || !hits[0].collider.CompareTag("Cat"))
        {
            hitCatLayer = null;
            return false;
        }

        // Get zone
        hitCatLayer = LayerMask.LayerToName(hits[0].collider.gameObject.layer);
        return true;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isInteractionButtonHold = true;
            if (isCatHover)
            {
                OnCatPetted?.Invoke(hitLayer, 1, cursorPosition); // 1 arbitrary
            }
        }
        else if (context.canceled)
        {
            isInteractionButtonHold = false;
        }
    }
}