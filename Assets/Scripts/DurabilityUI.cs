using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DurabilityUI : MonoBehaviour
{
    [SerializeField] TMP_Text durabilityText;
    GearTextNumber gearText;
    FlashText flashText;
    void Start()
    {
        gearText = GetComponentInParent<GearTextNumber>();
        flashText = durabilityText.GetComponent<FlashText>();
    }

    string _textToShow = "";
    void Update()
    {
        if (Player.Instance && gearText.LinkedGearDice != null)
        {
            _textToShow = gearText.LinkedGearDice.durability.ToString();
            durabilityText.text = _textToShow;

            // Flash if low
            if (flashText && gearText.LinkedGearDice.durability <= 7)
                flashText.enabled = true;
            else
                flashText.enabled = false;
        }

       
    }
}
