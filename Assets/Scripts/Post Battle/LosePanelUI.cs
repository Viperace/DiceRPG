using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LosePanelUI : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text journeyText;
    void Start()
    {
        
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
