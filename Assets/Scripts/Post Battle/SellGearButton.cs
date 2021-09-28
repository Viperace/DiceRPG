using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SellGearButton : MonoBehaviour
{
    GameParameter gameParameter;
    TMP_Text goldText;
    GearDiceUIDisplay parentUI;
    void Start()
    {
        goldText = GetComponentInChildren<TMP_Text>();
        parentUI = GetComponentInParent<GearDiceUIDisplay>();

        gameParameter = ScriptableObject.CreateInstance<GameParameter>();
    }
    void Update()
    {
        if (parentUI.RepresentedGearDice != null)
        {
            goldText.text = gameParameter.GetSellPrice(parentUI.RepresentedGearDice.goldCost).ToString();
            //goldText.text = Mathf.RoundToInt(gameParameter.SellGearDiscount * parentUI.RepresentedGearDice.goldCost).ToString();
        }
        else
        {
            goldText.text = "-";
        }
    }
}
