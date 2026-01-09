using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private string[] catZones;
    [SerializeField] private Cat cat;
    [SerializeField] private CatInteractionDetector interactionDetector;
    
    [Header("Hearts Settings")]
    [SerializeField] private float heartsMinSpawnInterval = 0.1f;
    [SerializeField] private float pettingSpeedMultiplier = 0.2f;

    private string currentZone = "CatLowerBack";
    private float currentZoneTargetPetting = 100f;
    private float currentZoneTargetSpeed;
    private float currentZonePetting = 0f;
    private float lastHeartSpawn = 0f;
    
    void Start()
    {
        interactionDetector.OnCatPetted += OnCatPetted;
    }

    void Update()
    {
        
    }
    
    private void OnCatPetted(string zone, float speed, Vector2 cursorPosition)
    {
        if (zone == currentZone)
        {
            // add value
            currentZonePetting += speed * pettingSpeedMultiplier;
            
            // cat animation
            cat.SetAnimationHappiness(Mathf.Clamp01(currentZonePetting / currentZoneTargetPetting));
            
            if (Time.time - lastHeartSpawn < heartsMinSpawnInterval)
            {
                return;
            }
            
            // spawn heart
            var position = cursorPosition + new Vector2(0, Screen.height * 0.005f);
            UIManager.Instance.SpawnCatHeart(position);
            
            // todo: sound
            
            if (currentZonePetting >= currentZoneTargetPetting)
            {
                FinishZone(zone);
                ActivateZone(zone);
            }
            
            lastHeartSpawn = Time.time;
        }
    }
    
    private void FinishZone(string zone)
    {
        // todo: spawn multiple hearts nearby zone, send them to hearts counter bar
        // todo: reset zone shader
        currentZone = null;
        currentZoneTargetPetting = 0;
        currentZonePetting = 0;
    }
    
    private void ActivateZone(string zone)
    {
        currentZone = zone;
        currentZoneTargetPetting = 100f;
        currentZonePetting = 0;
        cat.SetAnimationHappiness(0);
    }
    
    
}