using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CatVFX
{
    // pulse settings
    private float pulseStartFrequency;
    private float pulseEndFrequency;
    private float minPulseOpacity;
    private float maxPulseOpacity;
    
    private int pulseShaderOpacityID = Shader.PropertyToID("_Opacity");

    public CatVFX(CatVisualSettings visualSettings)
    {
        pulseStartFrequency = visualSettings.zonePulseStartFrequency;
        pulseEndFrequency = visualSettings.zonePulseEndFrequency;
        minPulseOpacity = visualSettings.zoneMinPulseOpacity;
        maxPulseOpacity = visualSettings.zoneMaxPulseOpacity;
    }
    
    public void Tick(List<CatZone> activeZones)
    {
        // Zone pulse effect
        foreach (var zone in activeZones)
        {
            var pulseSpeed = Mathf.Lerp(pulseStartFrequency, pulseEndFrequency, zone.CurrentTargetPercentage);
            zone.CurrentPulseTime += Time.deltaTime * pulseSpeed;
            
            float opacity = Mathf.Lerp(minPulseOpacity, maxPulseOpacity, (Mathf.Sin(zone.CurrentPulseTime * Mathf.PI * 2f) + 1f) / 2f);
            zone.DecalProjector.material.SetFloat(pulseShaderOpacityID, opacity);
        }
    }
    
    public void StopZonePulse(string zone)
    {
        
    }
    
    // todo
    public void HeartSpawn(Vector2 position)
    {
        
    }
    
    public void SetZonePulseSpeed(string zone, float speed)
    {
        
    }
}
