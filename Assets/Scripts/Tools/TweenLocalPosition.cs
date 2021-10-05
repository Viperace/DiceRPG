using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TweenLocalPosition : MonoBehaviour
{
    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 endPos;
    [SerializeField] float duration = 1f;
    [SerializeField] bool shuffleDuration = true;

    Sequence seq;
    IEnumerator tweenCoroutine;
    void Start()
    {
        tweenCoroutine = BeginTween(0.5f);
        StartCoroutine(tweenCoroutine);
    }

    void OnValidate()
    {
        StopCoroutine(tweenCoroutine);
        StartCoroutine(tweenCoroutine);
        seq.Kill();
    }

    IEnumerator BeginTween(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 
        if (shuffleDuration)
            duration += Random.Range(0.9f, 1.1f);

        // Calc timing needed
        float distToStart = Vector3.Distance(Vector3.zero, startPos);
        float distToEnd = Vector3.Distance(Vector3.zero, endPos);
        float dist = Vector3.Distance(startPos, endPos);
        dist = Mathf.Clamp(dist, 0.001f, float.MaxValue);

        // Actual Pos
        Vector3 origPos = this.transform.localPosition;
        Vector3 actualStartPos = origPos + startPos;
        Vector3 actualEndPos = origPos + endPos;

        Sequence seq = DOTween.Sequence();

        seq.Append(this.transform.DOLocalMove(actualStartPos, duration * distToStart / dist).SetEase(Ease.OutQuad));
        seq.Append(this.transform.DOLocalMove(actualEndPos, duration * 1f ).SetEase(Ease.OutQuad));
        seq.Append(this.transform.DOLocalMove(origPos, duration * distToEnd / dist).SetEase(Ease.OutQuad));
        seq.SetLoops(-1);
    }
}
