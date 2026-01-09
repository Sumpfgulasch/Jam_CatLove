using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;

public class Cat : MonoBehaviour
{
    public Animator animator;

    private void Update()
    {
        
    }
    
    public void SetAnimationHappiness(float happiness)
    {
        animator.SetFloat("Happy", happiness);
    }
    

    public void Vanish()
    {
        
    }
}