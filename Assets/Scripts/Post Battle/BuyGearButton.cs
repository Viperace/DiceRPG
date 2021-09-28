using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Always find represented dice, and display the Gold amount
/// </summary>
public class BuyGearButton : MonoBehaviour
{
    TMP_Text goldText;
    GearDiceUIDisplay parentUI;

    void Start()
    {
        goldText = GetComponentInChildren<TMP_Text>();
        parentUI = GetComponentInParent<GearDiceUIDisplay>();
    }

    void Update()
    {
        if(parentUI.RepresentedGearDice != null)
        {
            goldText.text = parentUI.RepresentedGearDice.goldCost.ToString();
        }
        else
        {
            goldText.text = "-";
        }
    }
}
