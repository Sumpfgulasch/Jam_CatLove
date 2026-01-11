using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CatVFX
{
    // pulse settings
    private float pulseStartFrequency;
    private float pulseEndFrequency;
    private float pulseStartMinOpacity;
    private float pulseStartMaxOpacity;
    private float pulseEndMinOpacity;
    private float pulseEndMaxOpacity;
    
    private int pulseShaderOpacityID = Shader.PropertyToID("_Opacity");

    public CatVFX(CatVisualSettings visualSettings)
    {
        pulseStartFrequency = visualSettings.zonePulseStartFrequency;
        pulseEndFrequency = visualSettings.zonePulseEndFrequency;
        pulseStartMinOpacity = visualSettings.zonePulseStartMinOpacity;
        pulseStartMaxOpacity = visualSettings.zonePulseStartMaxOpacity;
        pulseEndMinOpacity = visualSettings.zonePulseEndMinOpacity;
        pulseEndMaxOpacity = visualSettings.zonePulseEndMaxOpacity;
    }
    
    public void Tick(List<CatZone> activeZones)
    {
        // Zone pulse effect
        foreach (var zone in activeZones)
        {
            var pulseSpeed = Mathf.Lerp(pulseStartFrequency, pulseEndFrequency, zone.CurrentTargetPercentage);
            zone.CurrentPulseTime += Time.deltaTime * pulseSpeed;
            
            var minOpacity = Mathf.Lerp(pulseStartMinOpacity, pulseEndMinOpacity, zone.CurrentTargetPercentage);
            var maxOpacity = Mathf.Lerp(pulseStartMaxOpacity, pulseEndMaxOpacity, zone.CurrentTargetPercentage);
            var opacity = Mathf.Lerp(minOpacity, maxOpacity, (Mathf.Sin(zone.CurrentPulseTime * Mathf.PI * 2f) + 1f) / 2f);
            zone.DecalProjector.material.SetFloat(pulseShaderOpacityID, opacity);
        }
    }
}
