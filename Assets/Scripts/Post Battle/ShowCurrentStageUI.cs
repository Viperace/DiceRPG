using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ShowCurrentStageUI : MonoBehaviour
{
    TMP_Text text;

    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (Player.journeyLog != null)
        {
            text.text = string.Concat("Stage ", Player.journeyLog.Length + 1);
        }
    }
}
