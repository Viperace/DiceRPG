using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class TweenOnValueChange : MonoBehaviour
{
    [SerializeField] TMP_Text[] targetText;
    string[] currentText;

    void Start()
    {
        currentText = new string[targetText.Length];

        for (int i = 0; i < targetText.Length; i++)
            currentText[i] = targetText[i].text;
    }

    void DoAnimate()
    {
        float strength = 0.6f*this.transform.localScale.y;
        this.transform.DOShakeScale(0.3f, strength).SetEase(Ease.InBounce);
    }

    void Update()
    {
        if (IsAnyChange())
        {
            DoAnimate();

            // Reset
            for (int i = 0; i < targetText.Length; i++)
                currentText[i] = targetText[i].text;
        }
    }
    bool IsAnyChange()
    {
        for (int i = 0; i < targetText.Length; i++)
            if (currentText[i] != targetText[i].text)
                return true;
        return false;
    }
}
