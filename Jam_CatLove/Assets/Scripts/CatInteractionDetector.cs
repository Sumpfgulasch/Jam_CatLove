using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class CatInteractionDetector : MonoBehaviour
{
    public event Action<Cat> OnCatClicked;
    public event Action<Cat> OnCatHoverEnter;
    public event Action<Cat> OnCatHoverExit;
    public event Action<Cat> OnCatInteraction;

    private Camera mainCamera;
    private Cat currentHoveredCat = null;

    private void Start()
    {
        mainCamera = Camera.main;
        OnCatClicked += (cat) =>OnCatInteraction?.Invoke(cat);
        OnCatHoverEnter += (cat) =>OnCatInteraction?.Invoke(cat);
    }

    private void Update()
    {
        CheckHover();
        CheckClick();
    }

    private void CheckHover()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Cat hoveredCat = null;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            hoveredCat = hit.collider.GetComponent<Cat>();
        }

        // Hover state changed
        if (hoveredCat != currentHoveredCat)
        {
            // Exit previous cat
            if (currentHoveredCat != null)
            {
                OnCatHoverExit?.Invoke(currentHoveredCat);
            }

            // Enter new cat
            if (hoveredCat != null)
            {
                OnCatHoverEnter?.Invoke(hoveredCat);
            }

            currentHoveredCat = hoveredCat;
        }
    }

    private void CheckClick()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Cat clickedCat = hit.collider.GetComponent<Cat>();
                if (clickedCat != null)
                {
                    OnCatClicked?.Invoke(clickedCat);
                }
            }
        }
    }

    // Clear hovered cat reference when it gets destroyed
    public void ClearHoveredCat(Cat cat)
    {
        if (currentHoveredCat == cat)
        {
            currentHoveredCat = null;
        }
    }
}