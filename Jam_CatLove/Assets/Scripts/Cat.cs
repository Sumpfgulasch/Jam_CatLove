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

    public void SetAnimationState(bool isHovered)
    {
        animator.SetBool("IsHovered", isHovered);
    }
    

    public void Vanish()
    {
        
    }

    
}