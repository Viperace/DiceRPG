using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class RollDiceView : MonoBehaviour
{
    Text numberText;
    RectTransform rectTransform;
    [SerializeField] float movePosX = 2;
    [SerializeField] float showDuration = 3f;
    [SerializeField] float delayStart = 0f;
    [SerializeField] float flashPeriod = 0.05f;

    Vector3 origPos;
    void Awake()
    {
        DOTween.Init();

        numberText = GetComponentInChildren<Text>();
        rectTransform = GetComponent<RectTransform>();

        origPos = this.transform.position;
    }

    void Update()
    {
        
    }

    IEnumerator ShuffleNumber(int finalNumber, float duration, int min, int max)
    {
        // Start shuffling
        float endTime = Time.time + duration;
        while (Time.time < endTime)
        {
            //numberText.text = Random.Range(min, max + 1).ToString();
            numberText.text = Random.Range(0, 10).ToString();
            yield return new WaitForSeconds(flashPeriod);
        }
        numberText.text = finalNumber.ToString();
    }

    public void AnimateRollingNumbers(int finalNumber)
    {
        Sequence sequence = DOTween.Sequence();

        float target = rectTransform.position.x + movePosX;
        sequence.Append(rectTransform.DOAnchorPosX(target, showDuration).SetEase(Ease.OutCubic));
        //sequence.Join(numberText.DOText(finalNumber.ToString(), showDuration + 4, true, ScrambleMode.Numerals));
    }

    void AnimateMoveNumbers(float delayStart)
    {
        Sequence sequence = DOTween.Sequence();

        float target = rectTransform.position.x + movePosX;
        sequence.PrependInterval(delayStart);
        sequence.Append(rectTransform.DOAnchorPosX(target, showDuration).SetEase(Ease.OutCubic));
    }

    public void _TestNumber()
    {
        int min = 1;
        int max = 15;
        int x = Random.Range(min, max + 1);
        //AnimateRollingNumbers(x);
        AnimateMoveNumbers(delayStart);
        StartCoroutine(ShuffleNumber(x, showDuration + delayStart, min, max ));
    }
}
