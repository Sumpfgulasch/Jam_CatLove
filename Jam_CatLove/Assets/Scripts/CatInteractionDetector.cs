using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class CatInteractionDetector : MonoBehaviour
{
    public delegate void CatPetted(string zone, float speed);
    public event CatPetted OnCatPetted;
    
    [SerializeField] private InputActionReference moveCursorAction;
    [SerializeField] private InputActionReference interactAction;

    private string hitLayer;
    
    private InputSystem_Actions inputActions;
    private Vector2 cursorPosition = new ();
    private Camera mainCamera;
    private bool isInteracting = false;
    private Vector2 previousCursorPosition = new ();
    
    
    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }
    
    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.MoveCursor.performed += OnMoveCursor;
        inputActions.Player.Interact.started += OnInteract;
        inputActions.Player.Interact.canceled += OnInteract;
    }
    
    private void Start()
    {
        mainCamera = Camera.main;
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
        cursorPosition = context.ReadValue<Vector2>();
        
        // Check hits
        var hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(cursorPosition));
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        if (hits.Length == 0 || !hits[0].collider.CompareTag("Cat"))
        {
            return;
        }
        
        // Get cat zone
        hitLayer = LayerMask.LayerToName(hits[0].collider.gameObject.layer);

        // Invoke event
        if (isInteracting)
        {
            var speed = ((cursorPosition - previousCursorPosition) / Time.deltaTime).magnitude;
            Debug.Log($"layer: {hitLayer}, speed: {speed}");
            OnCatPetted?.Invoke(hitLayer, speed);
        }
        
        previousCursorPosition = cursorPosition;
    }
    
    private void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isInteracting = true;
        }
        else if (context.canceled)
        {
            isInteracting = false;
        }
    }
    
}