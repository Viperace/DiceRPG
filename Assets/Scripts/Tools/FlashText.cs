using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class FlashText : MonoBehaviour
{
    TMP_Text text;
    [SerializeField] Color color1;
    [SerializeField] Color color2;
    [SerializeField] Color color3;
    [SerializeField] float duration;
    Sequence seq;

    void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    public void ToggleTween(bool value)
    {
        if (value)
        {
            seq.Kill();
            seq = DOTween.Sequence();
            seq.Append(text.DOColor(color1, duration));
            seq.Append(text.DOColor(color2, duration));
            seq.Append(text.DOColor(color3, duration));
            seq.Append(text.DOColor(color2, duration));
            seq.SetLoops(-1);
        }
        else
            seq.Kill();
    }

    void OnDisable()
    {
        ToggleTween(false);
    }

    void OnEnable()
    {
        ToggleTween(true);    
    }
}
