using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;
    [SerializeField] private Cat cat;
    [SerializeField] private CatInteractionDetector interactionDetector;
    [SerializeField] private CatVisualSettings catVisualSettings;
    [SerializeField] private GameplaySettings gameplaySettings;
    
    public float pettingSkillMultiplier { get; set; } = 1f;
    public int heartsCount { get; set; } = 0;

    private float lastHeartSpawn = 0f;
    private float pettingSinceLastHeart;
    private float lastZoneFinishTime;
    private List<CatZone> activeZones = new();
    
    
    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        
        interactionDetector.OnCatPetted += OnCatPetted;

        cat.Init(gameplaySettings, catVisualSettings);

        ActivateRandomZoneDelayed();
    }

    void Update()
    {
        cat.Tick(activeZones);
    }

    /// <param name="speed">In screen diagonals per second</param>
    /// <param name="cursorPosition"></param>
    private void OnCatPetted(string hitLayer, float speed, Vector2 cursorPosition)
    {
        // cat zone hit?
        var catZone = activeZones.FirstOrDefault(activeZone => activeZone.Name.Equals(hitLayer));
        if (catZone == null)
        {
            return;
        }

        // update progress
        var optimalSpeedMultiplier = MathUtility.GetSpeedMultiplier(speed, 
            gameplaySettings.optimalPettingSpeed, 
            gameplaySettings.pettingSpeedLowerTolerance, 
            gameplaySettings.pettingSpeedUpperTolerance, 
            gameplaySettings.optimalPettingSpeedMultiplier);
        var currentPetting = (speed * pettingSkillMultiplier * optimalSpeedMultiplier * Time.deltaTime);
        catZone.CurrentPetting += currentPetting;
        pettingSinceLastHeart += currentPetting;
        
        // animation
        cat.SetAnimationHappiness(catZone.CurrentTargetPercentage);

        // finish zone?
        if (catZone.IsTargetReached)
        {
            FinishZone(catZone, cursorPosition);
            ActivateRandomZoneDelayed();
            return;
        }

        // spawn heart
        if (pettingSinceLastHeart < gameplaySettings.requiredPettingPerHeart || Time.time - lastHeartSpawn < gameplaySettings.heartsMinSpawnInterval)
        {
            return;
        }
        
        var hearts = Mathf.RoundToInt(pettingSinceLastHeart / gameplaySettings.requiredPettingPerHeart);
        var position = cursorPosition + new Vector2(0, Screen.height * catVisualSettings.heartSpawnYOffset);
        
        UIManager.Instance.SpawnCatHearts(position, hearts);
        
        heartsCount += hearts;
        lastHeartSpawn = Time.time;
        pettingSinceLastHeart = 0;
    }

    private void FinishZone(CatZone zone, Vector2 cursorPosition)
    {
        var position = cursorPosition + new Vector2(0, Screen.height * catVisualSettings.heartSpawnYOffset);
        heartsCount += gameplaySettings.heartsPerFinishedZone;
        UIManager.Instance.SpawnCatHearts(position, gameplaySettings.heartsPerFinishedZone);
        zone.Reset();
        zone.Enable(false);
        activeZones.Remove(zone);
        lastZoneFinishTime = Time.time;
    }

    private void ActivateRandomZoneDelayed()
    {
        StartCoroutine(ActivateRandomZoneDelayedRoutine(cat.Zones.GetRandomZone()));
    }
    
    private IEnumerator ActivateRandomZoneDelayedRoutine(CatZone zone)
    {
        yield return new WaitForSeconds(gameplaySettings.zoneActivationDelay);
        ActivateZone(zone);
    }

    private void ActivateZone(CatZone zone)
    {
        activeZones.Add(zone);
        zone.Activate(gameplaySettings.targetPettingPerZone);

        cat.SetAnimationHappiness(0);
    }
}