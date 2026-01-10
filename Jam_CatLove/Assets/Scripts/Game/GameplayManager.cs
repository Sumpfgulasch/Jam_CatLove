using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private Cat cat;
    [SerializeField] private CatInteractionDetector interactionDetector;
    [SerializeField] private CatVisualSettings catVisualSettings;
    
    [Header("Hearts Settings")]
    [SerializeField] private float heartsMinSpawnInterval = 0.1f;
    [SerializeField] private float pettingSpeedMultiplier = 0.2f;
    
    [Header("Zone Settings")]
    [SerializeField] private float zoneActivationDelay = 2f;
    
    private float lastHeartSpawn = 0f;
    private float lastZoneFinishTime;
    private List<CatZone> activeZones = new();
    
    void Start()
    {
        interactionDetector.OnCatPetted += OnCatPetted;
        
        cat.Init(catVisualSettings);
        foreach (var catZone in cat.Zones)
        {
            catZone.Enable(false);
        }

        StartCoroutine(ActivateZoneDelayed(cat.GetRandomZone()));
    }

    void Update()
    {
        cat.Tick(activeZones);
    }
    
    private void OnCatPetted(string hitLayer, float speed, Vector2 cursorPosition)
    {
        var zone = activeZones.FirstOrDefault(activeZone => activeZone.Name.Equals(hitLayer));
        if (zone == null)
        {
            return;
        }
        
        // update progress
        zone.CurrentPetting += speed * pettingSpeedMultiplier;
        cat.SetAnimationHappiness(zone.CurrentTargetPercentage);
            
        // finish zone?
        if (zone.IsTargetReached)
        {
            FinishZone(zone);
            StartCoroutine(ActivateZoneDelayed(cat.GetRandomZone()));
            return;
        }
        
        // spawn heart
        if (Time.time - lastHeartSpawn < heartsMinSpawnInterval)
        {
            return;
        }
        var position = cursorPosition + new Vector2(0, Screen.height * 0.005f);
        UIManager.Instance.SpawnCatHeart(position);
        lastHeartSpawn = Time.time;
        
        // todo: sound
    }
    
    private void FinishZone(CatZone zone)
    {
        zone.Reset();
        zone.Enable(false);
        activeZones.Remove(zone);
        lastZoneFinishTime = Time.time;
    }
    
    private IEnumerator ActivateZoneDelayed(CatZone zone)
    {
        yield return new WaitForSeconds(zoneActivationDelay);
        ActivateZone(zone);
    }
    
    private void ActivateZone(CatZone zone)
    {
        activeZones.Add(zone);
        zone.Activate(100f);
        
        cat.SetAnimationHappiness(0);
    }
    
    
}