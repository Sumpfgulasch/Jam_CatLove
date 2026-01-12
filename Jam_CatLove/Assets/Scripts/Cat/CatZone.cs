using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CatZone : MonoBehaviour
{
    public DecalProjector DecalProjector;
    
    public string Name { get; private set; } // == layer
    public GameObject GameObject => gameObject;
    public float TargetPetting { get; private set; }
    public float TargetSpeed { get; }
    public float CurrentPetting { get; set; } = 0;
    public float CurrentPulseTime { get; set; } = 0;

    public float CurrentTargetPercentage => Mathf.Clamp01(CurrentPetting / TargetPetting);
    public bool IsTargetReached => CurrentTargetPercentage >= 1f;

    public void Init()
    {
        Name = LayerMask.LayerToName(gameObject.layer);
    }

    public void Activate(float targetPetting)
    {
        gameObject.SetActive(true);
        TargetPetting = targetPetting;
    }
    
    public void Reset()
    {
        CurrentPetting = 0;
        CurrentPulseTime = 0;
        TargetPetting = 0;
    }
    
    public void Enable(bool enable)
    {
        GameObject.SetActive(enable);
    }
}
