using UnityEngine;

public class CatZones : MonoBehaviour
{
    public CatZone[] Zones { get; private set; }
    
    public void Init()
    {
        Zones = GetComponentsInChildren<CatZone>();
        foreach (var catZone in Zones)
        {
            catZone.Init();
        }
    }
}
