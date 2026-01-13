using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class CatInteractionDetector : MonoBehaviour
{
    public delegate void CatPetted(string zone, float speed, Vector2 cursorPosition, bool isGrab);

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
    private float currentPettingSpeed = 0f;


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

    private Vector2 prevCursorPos;

    private void Update()
    {
        var mousePos = Mouse.current.position.ReadValue();
        var curSpeed = (((mousePos - prevCursorPos) / Time.deltaTime).magnitude) / screenDiagonal;
        
        // Check if hovering cat
        var isCatHover = IsCatHover(mousePos, out hitCatLayer);
        
        // Only set speed if hovering cat and holding interaction button, otherwise set to 0
        if (isCatHover && isInteractionButtonHold)
        {
            AudioManager.Instance.SetGlobalParameter(FmodParameter.PETTING_SPEED, curSpeed);
        }
        else
        {
            AudioManager.Instance.SetGlobalParameter(FmodParameter.PETTING_SPEED, 0f);
        }
        
        prevCursorPos = mousePos;
    }

    private void OnMoveCursor(InputAction.CallbackContext context)
    {
        if (UIManager.Instance.IsSkillTreeOpen) 
            return;
        
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
            currentPettingSpeed = (((cursorPosition - previousCursorPosition) / Time.deltaTime).magnitude) / screenDiagonal;
            OnCatPetted?.Invoke(hitCatLayer, currentPettingSpeed, cursorPosition, false);
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
        if (UIManager.Instance.IsSkillTreeOpen) 
            return;
        
        if (context.started)
        {
            isInteractionButtonHold = true;
            if (isCatHover)
            {
                OnCatPetted?.Invoke(hitCatLayer, 0, cursorPosition, true); // speed arbitrary
            }
        }
        else if (context.canceled)
        {
            isInteractionButtonHold = false;
        }
    }
}