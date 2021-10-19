using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowBoostedValueUI : MonoBehaviour
{
    DiceBooster diceBooster;
    public TMP_Text attackText;
    public TMP_Text defendText;

    void Start()
    {
        StartCoroutine(InitDiceBooster());

    }

    IEnumerator InitDiceBooster()
    {
        while (GameManager.Instance.GetDiceBooster == null)
            yield return null;

        diceBooster = GameManager.Instance.GetDiceBooster;
    }

    void Update()
    {
        // TODO: Animate it
        if (diceBooster != null)
        {
            if (attackText && diceBooster.boostedAttack == 0)
                attackText.text = "";
            else
                ShowText(attackText, diceBooster.boostedAttack);

            if (defendText && diceBooster.boostedDefend == 0)
                defendText.text = "";
            else
                ShowText(defendText, diceBooster.boostedDefend);
            //defendText.text = string.Concat("+", diceBooster.boostedDefend);
        }
    }

    string _newText = "";
    void ShowText(TMP_Text textUI, int value)
    {
        _newText = string.Concat("+", value);
        if (_newText != textUI.text)
        {
            // Do animation
            textUI.text = _newText;
        }
    }
}
