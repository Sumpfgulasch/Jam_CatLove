using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CatZones
{
    private CatZone[] allZones;
    private Transform zonesParent;

    private List<CatZone> UnlockedZones { get; set; } = new();
    
    
    public void Init(GameplaySettings gameplaySettings, Transform zonesParent)
    {
        this.zonesParent = zonesParent;
        UnlockedZones = gameplaySettings.startZones.ToList();
        
        InitZones();
    }
    
    private void InitZones()
    {
        allZones = zonesParent.GetComponentsInChildren<CatZone>(true);

        foreach (var catZone in allZones)
        {
            catZone.Init();
            catZone.gameObject.SetActive(false);
        }
    }
    
    public void UnlockZones(List<CatZone> zones)
    {
        foreach (var zone in zones)
        {
            if (!UnlockedZones.Contains(zone))
            {
                UnlockedZones.Add(zone);
            }
        }
    }

    public CatZone GetRandomZone() => UnlockedZones[Random.Range(0, UnlockedZones.Count)];
}
