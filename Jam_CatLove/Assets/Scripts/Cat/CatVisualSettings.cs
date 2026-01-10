using UnityEngine;

[CreateAssetMenu(fileName = "CatVisualSettings", menuName = "Scriptable Objects/CatVisualSettings")]
public class CatVisualSettings : ScriptableObject
{
    [Header("Pulse Settings")]
    public float zonePulseStartFrequency = 2f; // Pulses per second
    public float zonePulseEndFrequency = 10f; // Pulses per second
    public float zoneMinPulseOpacity = 0.1f;
    public float zoneMaxPulseOpacity = 0.4f;
}
