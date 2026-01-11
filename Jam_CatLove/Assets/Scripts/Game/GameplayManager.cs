using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private Cat cat;
    [SerializeField] private CatInteractionDetector interactionDetector;
    [SerializeField] private CatVisualSettings catVisualSettings;
    [SerializeField] private GameplaySettings gameplaySettings;

    private float lastHeartSpawn = 0f;
    private float pettingSinceLastHeart;
    private float lastZoneFinishTime;
    private List<CatZone> activeZones = new();

    void Start()
    {
        interactionDetector.OnCatPetted += OnCatPetted;

        cat.Init(catVisualSettings);
        foreach (var catZone in cat.Zones)
        {
            catZone.Enable(false);
        }

        StartCoroutine(ActivateZoneDelayed(cat.GetRandomZone()));
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
        var optimalSpeedMultiplier = MathUtility.GetSpeedMultiplier(speed, gameplaySettings.optimalPettingSpeed,
            gameplaySettings.pettingSpeedLowerTolerance, gameplaySettings.pettingSpeedUpperTolerance, gameplaySettings.optimalPettingSpeedMultiplier);
        pettingSinceLastHeart += (speed * optimalSpeedMultiplier * Time.deltaTime);
        //pettingSinceLastHeart += (gameplaySettings.defaultPettingCount * optimalSpeedMultiplier * Time.deltaTime);
        catZone.CurrentPetting += pettingSinceLastHeart;
        cat.SetAnimationHappiness(catZone.CurrentTargetPercentage);
        
        Debug.Log($"currentPetting: {catZone.CurrentPetting}, speed: {speed}, speed multiplier: {optimalSpeedMultiplier}");

        // finish zone?
        if (catZone.IsTargetReached)
        {
            FinishZone(catZone, cursorPosition);
            StartCoroutine(ActivateZoneDelayed(cat.GetRandomZone()));
            return;
        }

        // spawn heart
        if (pettingSinceLastHeart < gameplaySettings.requiredPettingPerHeart || Time.time - lastHeartSpawn < gameplaySettings.heartsMinSpawnInterval)
        {
            return;
        }
        
        var heartsCount = Mathf.RoundToInt(pettingSinceLastHeart / gameplaySettings.requiredPettingPerHeart);

        var position = cursorPosition + new Vector2(0, Screen.height * catVisualSettings.heartSpawnYOffset);
        UIManager.Instance.SpawnCatHearts(position, heartsCount);
        lastHeartSpawn = Time.time;
        pettingSinceLastHeart = 0;

        // todo: sound
    }

    private void FinishZone(CatZone zone, Vector2 cursorPosition)
    {
        var position = cursorPosition + new Vector2(0, Screen.height * catVisualSettings.heartSpawnYOffset);
        UIManager.Instance.SpawnCatHearts(position, 30);
        zone.Reset();
        zone.Enable(false);
        activeZones.Remove(zone);
        lastZoneFinishTime = Time.time;
    }

    private IEnumerator ActivateZoneDelayed(CatZone zone)
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