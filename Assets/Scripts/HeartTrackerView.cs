using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HeartTrackerView : MonoBehaviour
{
    [SerializeField] EntityWithHP target;
    [SerializeField] bool isPlayer; // Yes = player, No = enemy

    WiggleTween[] hearts;
    Vector3 origScale;

    void Awake()
    {
        hearts = GetComponentsInChildren<WiggleTween>();
        origScale = hearts[0].transform.localScale;
    }

    void Start()
    {
    }

    public void ResetTarget()
    {
        if (isPlayer)
            //target = FindObjectOfType<Player>().playerStat;
            target = Player.playerStat;
        else
            target = GameManager.Instance.currentEnemy;

        ResetAllHearts();
    }

    public void ResetAllHearts(float heartDelay = 0.25f)
    {
        StartCoroutine(LateResetAllHeart(heartDelay));
    }

    IEnumerator LateResetAllHeart(float heartDelay)
    {
        yield return new WaitForSeconds(0.0f);

        // Hide all first
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].IsShowing = false;
            hearts[i].transform.localScale = Vector3.zero;
        }

        // Show only the needed HP
        if (target != null)
        {
            float delaySum = 0;
            for (int i = 0; i < target.HP; i++)
            {
                hearts[i].IsShowing = true;
                AnimateHeartEntry(hearts[i].gameObject, delaySum);
                delaySum += heartDelay;
            }
        }
    }

    void AnimateHeartEntry(GameObject heartGO, float delayStart)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.PrependInterval(delayStart);
        sequence.Append(heartGO.transform.DOScale(0, 0));
        sequence.Append(heartGO.transform.DOScale(origScale.y, 0.3f));
        sequence.Append(heartGO.transform.DOShakeScale(0.5f, origScale.y * .5f));
    }

    void AnimateLoseOneHeart(float delayStart = 0)
    {
        // Find out which heart to lose
        int indexToLose = target.HP;
        if (indexToLose < 0)
            return;
        else
        {
            WiggleTween h = hearts[indexToLose];

            Sequence sequence = DOTween.Sequence();
            sequence.PrependInterval(delayStart);
            sequence.Append(h.transform.DOShakeScale(0.5f, origScale.y * .5f));
            sequence.Append(h.transform.DOScale(0, 0.25f));

            h.IsShowing = false; // Hide it
        }
    }

    void AnimateLoseAllHeart(float delayStart = 0)
    {
        float inBetweenDelay = 0.5f;
        for (int i = 0; i < hearts.Length; i++)
        {
            WiggleTween h = hearts[i];
            if (h.IsShowing)
            {
                Sequence sequence = DOTween.Sequence();
                sequence.PrependInterval(delayStart + inBetweenDelay * i);
                sequence.Append(h.transform.DOShakeScale(0.5f, origScale.y * .5f));
                sequence.Append(h.transform.DOScale(0, 0.25f));

                h.IsShowing = false; // Hide it
            }
        }
    }

    void AnimateRestoreOneHeart(float delayStart = 0)
    {
        // Find out which heart to lose
        int indexToAdd = target.HP - 1;
        if (indexToAdd < 0)
            return;
        else
        {
            WiggleTween h = hearts[indexToAdd];

            Sequence sequence = DOTween.Sequence();
            sequence.PrependInterval(delayStart);
            sequence.Append(h.transform.DOScale(origScale, 0.25f));
            sequence.Append(h.transform.DOShakeScale(0.25f, origScale.y * .5f));

            h.IsShowing = true; // Show it
        }
    }

    void Update()
    {
        // Gather total HP that is showing
        int numberHeartsShown = 0;
        for (int i = 0; i < hearts.Length; i++)
            if (hearts[i].IsShowing)
                numberHeartsShown++;

        if (target != null)
            if (target.HP <= 0 & numberHeartsShown > 0)
                AnimateLoseAllHeart();
            else if (target.HP < numberHeartsShown)
                AnimateLoseOneHeart();
            else if (target.HP > numberHeartsShown)
                AnimateRestoreOneHeart();
    }

    public void _HideHeart()
    {
        ResetAllHearts();
    }
}
