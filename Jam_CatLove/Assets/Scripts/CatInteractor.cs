using UnityEngine;
using UnityEngine.InputSystem;

public class CatInteractor : MonoBehaviour
{
    [SerializeField] private CatInteractionDetector catInteractionDetector;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    
    
    private void HandleCatInteractionStart(Cat cat)
    {
        cat.SetAnimationState(true);
    }

    private void HandleCatInteractionStop(Cat cat)
    {
        cat.SetAnimationState(false);
    }
    
    private void OnCatInteraction(Cat cat)
    {
        UIManager.Instance.SpawnCatHeart(cat.transform);
    }
}
