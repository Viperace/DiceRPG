using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TweenScale : MonoBehaviour
{
    [SerializeField] float duration = 1f;
    [SerializeField] bool shuffleDuration = true;
    [SerializeField] Vector3 startLocalScale = new Vector3(1, 0.6f, 1);
    [SerializeField] Vector3 endLocalScale = new Vector3(1, 1.4f, 1);

    Sequence seq;
    IEnumerator tweenCoroutine;
    void Start()
    {
        tweenCoroutine = BeginTween(0.5f);
        StartCoroutine(tweenCoroutine);
    }

    void OnValidate()
    {
        if (tweenCoroutine != null)
        {
            StopCoroutine(tweenCoroutine);
            seq.Kill();
            StartCoroutine(tweenCoroutine);            
        }
    }

    IEnumerator BeginTween(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 
        if (shuffleDuration)
            duration += Random.Range(0.9f, 1.1f);

        
        Sequence seq = DOTween.Sequence();

        seq.Append(this.transform.DOScale(startLocalScale, duration).SetEase(Ease.OutQuad));
        seq.Append(this.transform.DOScale(endLocalScale, duration * 2).SetEase(Ease.InOutQuad));
        seq.Append(this.transform.DOScale(Vector3.one, duration));

        seq.SetLoops(-1);

    }
}
