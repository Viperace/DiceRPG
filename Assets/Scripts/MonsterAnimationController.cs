using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimationController : MonoBehaviour, IHitable
{
    Animator animator;
    HitEventReceiver hitEventReceiver; 
    void Start()
    {
    }
    public void SetupCharacter()
    {
        animator = GetComponentInChildren<Animator>();
        hitEventReceiver = GetComponentInChildren<HitEventReceiver>();
    }

    public void PlayAttack()
    {
        string key = Random.value > 0.5f ? "Attack1" : "Attack2";
        animator.SetTrigger(key);
    }

    public void PlayAttackOnTarget(IHitable target)
    {
        // Player do attack on target
        string key = Random.value > 0.5f ? "Attack1" : "Attack2";
        animator.SetTrigger(key);

        // Target play get hit
        hitEventReceiver.SetHitCallback(() => target.PlayGetHit());        
    }

    public void PlayGetHit()
    {
        animator.SetTrigger("GetHit");
    }

    public void PlayDead()
    {
        animator.SetTrigger("Die");
    }
    public void PlayTaunt()
    {
        animator.SetTrigger("Taunt");
    }
}
