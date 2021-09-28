using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

public class ButtonSelector : MonoBehaviour
{
    [SerializeField] Button RollPlayerButton;
    [SerializeField] Button RollEnemyButton;
    [SerializeField] Button RunNextRoundButton;
    [SerializeField] Button EndBattleButton;

    //[InlineButton("ToggleEnemyButton")]
    public bool enemyOn;

    //[InlineButton("TogglePlayerButton")]
    public bool playerOn;

    void Start()
    {
        //RollPlayerButton.onClick.AddListener(() => GameManager.Instance.RollPlayerDice());
        //RollEnemyButton.onClick.AddListener(() => GameManager.Instance._RollEnemyDice());
        //RunNextRoundButton.onClick.AddListener(() => GameManager.Instance.RunNextRound());
        //EndBattleButton.onClick.AddListener(() => GameManager.Instance.EndBattleSequence());
    }

    
    void HideButton(Button btn)
    {
        btn.interactable = false;
        btn.transform.DOScale(0, 0.1f);
    }

    void ShowButton(Button btn)
    {
        btn.transform.DOScale(1, 0.7f).SetEase(Ease.InBounce).OnComplete(() => btn.interactable = true);
    }
    public void HideAllButtons(float delay)
    {
        RollPlayerButton.interactable = false;
        RollEnemyButton.interactable = false;
        RunNextRoundButton.interactable = false;
        EndBattleButton.interactable = false;
        StartCoroutine(HideAllButtonsDelay(delay));
    }

    IEnumerator HideAllButtonsDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideButton(RollEnemyButton);
        HideButton(RollPlayerButton);
        HideButton(RunNextRoundButton);
        HideButton(EndBattleButton);
    }

    public void TogglePlayerButton(bool value, float delay)
    {
        StartCoroutine(TogglePlayerButtonWithDelay(value, delay));
    }
    IEnumerator TogglePlayerButtonWithDelay(bool value, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (value)
        {
            ShowButton(RollPlayerButton);
            HideButton(RollEnemyButton);
            HideButton(RunNextRoundButton);
        }
        else
        {
            HideButton(RollPlayerButton);
        }
    }


    public void ToggleEnemyButton(bool value, float delay)
    {
        StartCoroutine(ToggleEnemyButtonWithDelay(value, delay));
    }

    IEnumerator ToggleEnemyButtonWithDelay(bool value, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (value)
        {
            ShowButton(RollEnemyButton);
            HideButton(RollPlayerButton);
            HideButton(RunNextRoundButton);
        }
        else
        {
            HideButton(RollEnemyButton);
        }
    }

    public void ToggleNextRoundButton(bool value, float delay)
    {
        StartCoroutine(ToggleNextRoundButtonWithDelay(value, delay));
    }

    IEnumerator ToggleNextRoundButtonWithDelay(bool value, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (value)
        {
            ShowButton(RunNextRoundButton);
            HideButton(RollPlayerButton);
            HideButton(RollEnemyButton);
        }
        else
        {
            HideButton(RunNextRoundButton);
        }
    }

    public void ShowEndBattleButton(float delay)
    {
        StartCoroutine(ShowEndBattleButtonWithDelay(delay));
    }
    IEnumerator ShowEndBattleButtonWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowButton(EndBattleButton);
    }
}
