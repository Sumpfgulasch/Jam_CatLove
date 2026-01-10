using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Cat : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private CatZones zones;
    public CatZone[] Zones => zones.Zones;
    
    private CatVFX vfx;
    

    public void Init(CatVisualSettings visualSettings)
    {
        zones.Init();
        vfx = new CatVFX(visualSettings);
    }
    
    public CatZone GetRandomZone() => zones.Zones[Random.Range(0, zones.Zones.Length)];


    private void Start()
    {
        
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