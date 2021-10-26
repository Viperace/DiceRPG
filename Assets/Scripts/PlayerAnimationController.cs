using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour, IHitable
{
    public AudioClip[] getHitClips;
    public AudioClip[] attackHitClips;

    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
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
        hitEventCallback = () => target.PlayGetHit();

    }

    public void PlayGetHit()
    {
        animator.SetTrigger("GetHit");

        AudioClip toPlay = Random.value > 0.5f ? getHitClips[0] : getHitClips[1];
        SoundManager.Instance.Play(toPlay);
    }

    public void PlayDead()
    {
        string key = Random.value > 0.5f ? "Death1" : "Death2";
        animator.SetTrigger(key);
    }

    public void PlayVictory()
    {
        animator.SetTrigger("Victory");
    }

    System.Action hitEventCallback;
    public void MeleeHitEvent()
    {
        if(hitEventCallback != null)
            hitEventCallback();

        //
        AudioClip toPlay = Random.value > 0.5f ? attackHitClips[0] : attackHitClips[1];
        SoundManager.Instance.Play(toPlay);
        Debug.Log("Player blow hit");
    }

    public void RangeHitEvent()
    {
        Debug.Log("PrintFloat is called with a value of ");
    }

}

public interface IHitable
{
    public void PlayGetHit();
}