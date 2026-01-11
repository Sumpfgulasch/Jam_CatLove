using UnityEngine;

public class CatZones : MonoBehaviour
{
    public CatZone[] Zones { get; private set; }
    
    public void Init()
    {
        Zones = GetComponentsInChildren<CatZone>(true);

        foreach (var catZone in Zones)
        {
            catZone.Init();
            catZone.gameObject.SetActive(false);
        }
    }
}
