using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CatZones
{
    private CatZone[] allZones;
    private Transform zonesParent;

    private List<CatZone> UnlockedZones { get; set; } = new();
    private GameplaySettings gameplaySettings;
    
    
    public void Init(GameplaySettings gameplaySettings, Transform zonesParent)
    {
        this.gameplaySettings = gameplaySettings;
        this.zonesParent = zonesParent;
        
        InitZones();
    }
    
    private void InitZones()
    {
        allZones = zonesParent.GetComponentsInChildren<CatZone>(true);
        
        UnlockedZones = gameplaySettings.startZones.Select(startZoneName => allZones.FirstOrDefault(zoneName => zoneName.GameObject.name.Equals(startZoneName))).ToList();

        foreach (var catZone in allZones)
        {
            catZone.Init();
            catZone.gameObject.SetActive(false);
        }
    }
    
    public void UnlockZones(List<string> zones)
    {
        foreach (var zone in zones)
        {
            var catZone = allZones.FirstOrDefault(z => z.GameObject.name.Equals(zone));
            if (catZone == null)
            {
                Debug.LogError("Zone not found: " + zone);
                continue;
            }
            
            if (!UnlockedZones.Contains(catZone))
            {
                UnlockedZones.Add(catZone);
            }
        }
    }
    
    public void LockZones(List<string> zones)
    {
        foreach (var zone in zones)
        {
            var catZone = allZones.FirstOrDefault(z => z.GameObject.name.Equals(zone));
            if (catZone == null)
            {
                Debug.LogError("Zone not found: " + zone);
                continue;
            }
            
            UnlockedZones.Remove(catZone);
        }
    }

    public CatZone GetRandomZone() => UnlockedZones[Random.Range(0, UnlockedZones.Count)];

    // public void Reset()
    // {
    //     UnlockedZones = gameplaySettings.startZones.ToList();
    // }
}
