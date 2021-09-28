using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class heartsUI : MonoBehaviour
{
    TMP_Text text;
    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (Player.playerStat != null)
        {
            text.text = Player.playerStat.maxHP.ToString();
        }
    }
}
