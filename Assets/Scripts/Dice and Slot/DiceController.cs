using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DiceController : MonoBehaviour
{
    RollDiceViewTransform[] dices;
    Canvas canvas;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        dices = FindObjectsOfType<RollDiceViewTransform>();
    }

    public void ResetDice()
    {
        foreach(RollDiceViewTransform d in dices) { 
            d.transform.SetParent(canvas.transform);
            d.AnimateReset();
        }
    }

}
