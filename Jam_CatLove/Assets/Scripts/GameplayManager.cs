using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private string[] catZones;
    [SerializeField] private Cat cat;
    [SerializeField] private CatInteractionDetector interactionDetector;

    private string currentZone;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
    
    private void ActivateZone(string zone)
    {
        currentZone = zone;
    }
    
    
}
