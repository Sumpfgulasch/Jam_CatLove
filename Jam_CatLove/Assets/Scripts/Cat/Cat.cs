using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Cat : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform zonesParent;
    
    public CatZones Zones { get; private set; }
    
    private CatVFX vfx;
    
    private float happiness = 0f;

    public void Init(GameplaySettings gameplaySettings, CatVisualSettings visualSettings)
    {
        Zones = new CatZones();
        Zones.Init(gameplaySettings, zonesParent);

        vfx = new CatVFX(visualSettings);
    }
    
    public void Tick(List<CatZone> activeZones)
    {
        vfx.Tick(activeZones);
        
        var smoothedHappiness = Mathf.MoveTowards(animator.GetFloat("Happiness"), happiness, 0.3f * Time.deltaTime);
        animator.SetFloat("Happiness", smoothedHappiness);
    }

    public void SetAnimationHappiness(float happiness)
    {
        this.happiness = happiness;
    }


    public void Vanish()
    {
    }
}