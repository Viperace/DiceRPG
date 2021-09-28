using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinText : MonoBehaviour
{
    TMP_Text text;
    void Awake()
    {
        text = GetComponent<TMP_Text>();
    }
    void OnEnable()
    {
        text.text = string.Concat( "You killed the ", GameManager.Instance.currentEnemy.name);
    }
}
