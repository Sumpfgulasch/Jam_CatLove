using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class CatInteractionDetector : MonoBehaviour
{
    public delegate void CatPetted(string zone, float speed, Vector2 cursorPosition);

    public event CatPetted OnCatPetted;

    [SerializeField] private InputActionReference moveCursorAction;
    [SerializeField] private InputActionReference interactAction;

    private string hitCatLayer;

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
        isCatHover = IsCatHover(cursorPosition, out hitCatLayer);
        if (!isCatHover)
        {
            return;
        }

        // Invoke event
        if (isInteractionButtonHold)
        {
            // speed unit: screen diagonals per second
            var speed = (((cursorPosition - previousCursorPosition) / Time.deltaTime).magnitude) / screenDiagonal;
            OnCatPetted?.Invoke(hitCatLayer, speed, cursorPosition);
            //Debug.Log($"layer: {hitCatLayer}, speed: {speed}");
        }

        previousCursorPosition = cursorPosition;
    }

    private bool IsCatHover(Vector2 cursorPos, out string hitLayer)
    {
        // Check hits
        var hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(cursorPos));
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        if (hits.Length == 0 || !hits[0].collider.CompareTag("Cat"))
        {
            hitLayer = null;
            return false;
        }

        // Get zone
        hitLayer = LayerMask.LayerToName(hits[0].collider.gameObject.layer);
        return true;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isInteractionButtonHold = true;
            if (isCatHover)
            {
                OnCatPetted?.Invoke(hitCatLayer, 0.3f, cursorPosition); // speed arbitrary
            }
        }
        else if (context.canceled)
        {
            isInteractionButtonHold = false;
        }
    }
}