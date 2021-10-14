using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LosePanelUI : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text journeyText;
    [SerializeField] GameObject continueButton;

    void OnEnable()
    {
        if (Player.playerStat != null)
        {
            if (Player.journeyLog.Length <= 6)
                continueButton.gameObject.SetActive(true);
            else
                continueButton.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Player.playerStat != null)
        {
            journeyText.text = Player.journeyLog.Length.ToString();
            scoreText.text = (Player.journeyLog.Length * 50 + 
                Player.playerStat.gearDices.Count * 5 + 
                Player.playerStat.Gold).ToString();
        }

    }
}
