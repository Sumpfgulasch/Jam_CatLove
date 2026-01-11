using UnityEngine;

[CreateAssetMenu(fileName = "CatVisualSettings", menuName = "Scriptable Objects/CatVisualSettings")]
public class CatVisualSettings : ScriptableObject
{
    [Header("Zone Pulse Settings")]
    public float zonePulseStartFrequency = 2f; // Pulses per second
    public float zonePulseEndFrequency = 10f; // Pulses per second
    public float zonePulseStartMinOpacity = 0.1f;
    public float zonePulseStartMaxOpacity = 0.4f;
    public float zonePulseEndMinOpacity = 0.1f;
    public float zonePulseEndMaxOpacity = 0.7f;
    
    [Header("Hearts")]
    [Tooltip("In screen height")] public float heartSpawnYOffset = 0.05f;
}
