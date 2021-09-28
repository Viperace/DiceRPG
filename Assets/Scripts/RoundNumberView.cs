using UnityEngine;
using TMPro;
using DG.Tweening;

public class RoundNumberView : MonoBehaviour
{
    TMP_Text text;
    int _currentRound = 0;
    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if(_currentRound != GameManager.Instance.RoundNumber)
        {
            _currentRound = GameManager.Instance.RoundNumber;
            DoTransitionTo(GameManager.Instance.RoundNumber);
        }
    }

    void DoTransitionTo(int x)
    {
        text.transform.DOPunchScale(text.transform.localScale, 0.3f).OnComplete( () => text.text = x.ToString());
    }
}
