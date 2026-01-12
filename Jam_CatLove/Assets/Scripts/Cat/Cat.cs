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

    public void Init(GameplaySettings gameplaySettings, CatVisualSettings visualSettings)
    {
        Zones = new CatZones();
        Zones.Init(gameplaySettings, zonesParent);

        vfx = new CatVFX(visualSettings);
    }
    
    public void Tick(List<CatZone> activeZones)
    {
        vfx.Tick(activeZones);
    }

    public void SetAnimationHappiness(float happiness)
    {
        animator.SetFloat("Happiness", happiness);
    }


    public void Vanish()
    {
    }
}