using UnityEngine;

[CreateAssetMenu(fileName = "GameplaySettings", menuName = "Scriptable Objects/GameplaySettings")]
public class GameplaySettings : ScriptableObject
{
    [Header("Cat Petting")]
    [Tooltip("Without any multiplier, this means - while petting - pointer distance in screen diagonals per second")]
    public float requiredPettingPerHeart = 0.01f;
    public float targetPettingPerZone = 300f;

    public float optimalPettingSpeed = 5f;
    public float optimalPettingSpeedMultiplier = 1.3f;
    public float pettingSpeedLowerTolerance = 0.25f;
    public float pettingSpeedUpperTolerance = 0.25f;

    [Header("Hearts")] 
    public float heartsMinSpawnInterval = 0.1f;
    
    [Header("Zone")]
    public float zoneActivationDelay = 1f;
}