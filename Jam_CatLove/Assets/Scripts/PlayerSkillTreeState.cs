using UnityEngine;

public class PlayerSkillTreeState : MonoBehaviour
{
    public static PlayerSkillTreeState Instance;
    
    public bool UnlockedZones { get; set; }
    public float PettingMultiplier { get; set; } = 1f;
    
    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        
        Instance = this;
    }
}
