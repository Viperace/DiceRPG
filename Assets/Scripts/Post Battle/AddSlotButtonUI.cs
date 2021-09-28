using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AddSlotButtonUI : MonoBehaviour
{
    TMP_Text coinText;
    GameParameter gameParameter;
    void Start()
    {
        coinText = GetComponentInChildren<TMP_Text>();
        gameParameter = ScriptableObject.CreateInstance<GameParameter>();

    }

    void Update()
    {
        if (gameParameter && Player.playerStat != null)
        {
            coinText.text = gameParameter.GetAddSlotCost(Player.playerStat.AttackSlotNumber, Player.playerStat.DefendSlotNumber).ToString();
        }
    }
}
