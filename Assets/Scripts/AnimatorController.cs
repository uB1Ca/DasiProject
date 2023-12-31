using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayIdle()
    {
        animator.Play("Idle");
    }
    
    public void PlayWalk()
    {
        animator.Play("Walk");
    }

    public void SetTrigger(string Attack)
    {
        animator.SetTrigger("Attack");
    }
}
