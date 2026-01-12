using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameplayManager : MonoBehaviour {
    public static GameplayManager Instance;
    [SerializeField] private Cat cat;
    [SerializeField] private CatInteractionDetector interactionDetector;
    [SerializeField] private CatVisualSettings catVisualSettings;
    [SerializeField] private GameplaySettings gameplaySettings;

    public int heartsCount { get; set; } = 0;

    public Cat Cat => cat;

    private float lastHeartSpawn = 0f;
    private float pettingSinceLastHeart;
    private float lastZoneFinishTime;
    private List<CatZone> activeZones = new();
    
    public static bool IsPaused { get; private set; }
    
    public void Pause(bool pause)
    {
        if (pause) {
            Time.timeScale = 0f;
            IsPaused = true;
        }
        else {
            Time.timeScale = 1f;
            IsPaused = false;
        }
    }


    void Start() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        }
        else {
            Instance = this;
        }

        interactionDetector.OnCatPetted += OnCatPetted;

        cat.Init(gameplaySettings, catVisualSettings);
    }

    void Update() {
        cat.Tick(activeZones);
    }

    /// <param name="speed">In screen diagonals per second</param>
    /// <param name="cursorPosition"></param>
    private void OnCatPetted(string hitLayer, float speed, Vector2 cursorPosition) {
        
        var optimalSpeedMultiplier = MathUtility.GetSpeedMultiplier(speed,
            gameplaySettings.optimalPettingSpeed,
            gameplaySettings.pettingSpeedLowerTolerance,
            gameplaySettings.pettingSpeedUpperTolerance,
            gameplaySettings.optimalPettingSpeedMultiplier);
        
        var clampedSpeed = Mathf.Clamp(speed, 0, gameplaySettings.maxPettingSpeed);
        var currentPetting = (clampedSpeed * PlayerSkillTreeState.Instance.PettingMultiplier * optimalSpeedMultiplier * Time.deltaTime);
        
        // cat zone?
        if (PlayerSkillTreeState.Instance.UnlockedZones) {
            var catZone = activeZones.FirstOrDefault(activeZone => activeZone.Name.Equals(hitLayer));

            if (catZone != null) {
                currentPetting *= gameplaySettings.zonePettingBaseMultiplier;
                
                catZone.CurrentPetting += currentPetting;

                // animation
                cat.SetAnimationHappiness(catZone.CurrentTargetPercentage);

                // finish zone?
                if (catZone.IsTargetReached) {
                    FinishZone(catZone, cursorPosition);
                    ActivateRandomZoneDelayed();
                    return;
                }
            }
        }
        
        pettingSinceLastHeart += currentPetting;
        
        // spawn hearts
        if (pettingSinceLastHeart < gameplaySettings.requiredPettingPerHeart ||
            Time.time - lastHeartSpawn < gameplaySettings.heartsMinSpawnInterval) {
            return;
        }

        var hearts = Mathf.RoundToInt(pettingSinceLastHeart / gameplaySettings.requiredPettingPerHeart);
        var position = cursorPosition + new Vector2(0, Screen.height * catVisualSettings.heartSpawnYOffset);

        UIManager.Instance.SpawnCatHearts(position, hearts);

        heartsCount += hearts;
        lastHeartSpawn = Time.time;
        pettingSinceLastHeart = 0;
    }

    private void FinishZone(CatZone zone, Vector2 cursorPosition) {
        var position = cursorPosition + new Vector2(0, Screen.height * catVisualSettings.heartSpawnYOffset);
        heartsCount += gameplaySettings.heartsPerFinishedZone;
        UIManager.Instance.SpawnCatHearts(position, gameplaySettings.heartsPerFinishedZone);
        zone.Reset();
        zone.Enable(false);
        activeZones.Remove(zone);
        lastZoneFinishTime = Time.time;
    }

    public void ActivateRandomZoneDelayed() {
        StartCoroutine(ActivateRandomZoneDelayedRoutine(cat.Zones.GetRandomZone()));
    }

    private IEnumerator ActivateRandomZoneDelayedRoutine(CatZone zone) {
        yield return new WaitForSeconds(gameplaySettings.zoneActivationDelay);
        ActivateZone(zone);
    }

    private void ActivateZone(CatZone zone) {
        activeZones.Add(zone);
        zone.Activate(gameplaySettings.targetPettingPerZone);

        cat.SetAnimationHappiness(0);
    }
}