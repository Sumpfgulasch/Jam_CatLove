using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private string[] catZones;
    [SerializeField] private Cat cat;
    [SerializeField] private CatInteractionDetector interactionDetector;
    
    [Header("Hearts Settings")]
    [SerializeField] private float heartsMinSpawnInterval = 0.1f;
    [SerializeField] private float pettingSpeedMultiplier = 0.2f;
    
    [Header("Zone Settings")]
    [SerializeField] private float zoneActivationDelay = 2f;

    private string currentZone = "CatLowerBack";
    private float currentZoneTargetPetting = 100f;
    private float currentZoneTargetSpeed;
    private float currentZonePetting = 0f;
    private float lastHeartSpawn = 0f;
    private float lastZoneFinishTime;
    
    void Start()
    {
        interactionDetector.OnCatPetted += OnCatPetted;
    }

    void Update()
    {
        
    }
    
    private void OnCatPetted(string zone, float speed, Vector2 cursorPosition)
    {
        if (zone != currentZone)
        {
            return;
        }

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
        lastHeartSpawn = Time.time;
            
        // todo: sound
            
        if (currentZonePetting >= currentZoneTargetPetting)
        {
            FinishZone();
            var randomZone = catZones[Random.Range(0, catZones.Length)];
            StartCoroutine(ActivateZoneDelayed(randomZone));
        }
    }
    
    private void FinishZone()
    {
        // todo: spawn multiple hearts nearby zone, send them to hearts counter bar
        // todo: reset zone shader
        currentZone = null;
        currentZoneTargetPetting = 0;
        currentZonePetting = 0;
        lastZoneFinishTime = Time.time;
    }
    
    private IEnumerator ActivateZoneDelayed(string zone)
    {
        yield return new WaitForSeconds(zoneActivationDelay);
        ActivateZone(zone);
    }
    
    private void ActivateZone(string zone)
    {
        currentZone = zone;
        currentZoneTargetPetting = 100f;
        currentZonePetting = 0;
        cat.SetAnimationHappiness(0);
    }
    
    
}