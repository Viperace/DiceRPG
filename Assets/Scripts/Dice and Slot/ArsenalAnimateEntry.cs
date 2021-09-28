using System.Collections;
using UnityEngine;
using DG.Tweening;

public class ArsenalAnimateEntry : MonoBehaviour
{
    Player player;
    GearTextNumber[] arsenalDiceUI;
    void Start()
    {
        arsenalDiceUI = GetComponentsInChildren<GearTextNumber>(true);
        player = FindObjectOfType<Player>();

        StartCoroutine(AnimateEntry(0.1f));
    }

    IEnumerator AnimateEntry(float delay)
    {
        while(Player.playerStat == null)
        {
            yield return null;
        }

        // Turn off everything first (inactive and scale off)
        int numToShow = Player.playerStat.gearDices.Count;
        for (int i = 0; i < arsenalDiceUI.Length; i++)
        {
            if(i < numToShow) { 
                arsenalDiceUI[i].gameObject.SetActive(true);
                arsenalDiceUI[i].transform.DOScale(Vector3.zero, 0);
            }
            else
                arsenalDiceUI[i].gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(delay);

        for (int i = 0; i < arsenalDiceUI.Length; i++)
        {
            if (arsenalDiceUI[i].gameObject.activeInHierarchy) 
            { 
                Transform dice = arsenalDiceUI[i].transform;
                AnimateShake(dice);
            }
        }
    }

    void AnimateShake(Transform x, bool randomizeDelay = true)
    {
        Sequence seq = DOTween.Sequence();

        if(randomizeDelay)
            seq.PrependInterval(Random.Range(0, 0.3f));
        seq.Append(x.DOScale(new Vector3(0.8f, 1.2f, 1f), 0.5f));
        seq.Append(x.DOScale(new Vector3(1.2f, 0.8f, 1f), 0.5f));
        seq.Append(x.DOScale(new Vector3(1, 1, 1f), 0.2f));
        seq.SetEase(Ease.InBounce);
    }
}
