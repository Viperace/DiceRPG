using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SpecialComboHandler : MonoBehaviour
{
    List<DiceSpecialCombo> availableCombos = new List<DiceSpecialCombo>();
    [SerializeField] TMP_Text comboText;
    [SerializeField] TMP_Text comboSubText;
    [SerializeField] float specialEffectPlaytime = 2f;

    DiceComboEffectView viewEffect;
    SlotType[] playerSlots;

    void Start()
    {
        playerSlots = FindObjectsOfType<SlotType>();
        viewEffect = new DiceComboEffectView(playerSlots);
    }

    public SpecialComboHandler() { }
    
    public void AddCombo(DiceSpecialCombo combo)
    {
        if(combo != null)
            availableCombos.Add(combo);
    }

    public List<DiceSpecialCombo> GetTriggeredCombos(int[] diceValues)
    {
        // Check which combo trigger
        List<DiceSpecialCombo> triggeredCombo = new List<DiceSpecialCombo>();
        foreach (var combo in availableCombos)
        {
            // Check validity
            if (combo.IsValid(diceValues) && combo.IsTrigger(diceValues))
            {
                // Record all triggered combo, do effect later
                triggeredCombo.Add(combo);
            }
        }

        return triggeredCombo;
    }

    public void Handle(int[] diceValues)
    {
        // Check which combo trigger
        List<DiceSpecialCombo> triggeredCombo = GetTriggeredCombos(diceValues);

        // Do all combo effect
        StartCoroutine(PlaySpecialEffect(triggeredCombo));
    }

    IEnumerator PlaySpecialEffect(List<DiceSpecialCombo> combos)
    {
        if (combos.Count > 0)
        {
            string comboTextToShow = "";
            string comboSubTextToShow = "";
            foreach (var item in combos) 
            {
                comboTextToShow += item.name + "!\n";
                comboSubTextToShow += item.effectDescription + "\n";
            }
            Debug.Log(comboTextToShow);

            // Write main and sub text
            comboText.gameObject.SetActive(true);
            comboText.text = string.Concat(comboTextToShow);

            comboSubText.gameObject.SetActive(true);
            comboSubText.text = comboSubTextToShow;

            // Show Combo FX
            foreach (var combo in combos)
                viewEffect.ApplyEffect(combo);

            // Display screen shake
            // Ask dicesMonobehavior to shake, flash
            // Wait for X sec <- time for these effects to play out
            yield return new WaitForSeconds(specialEffectPlaytime);
            comboText.gameObject.SetActive(false);

            yield return new WaitForSeconds(0.5f);
            comboSubText.gameObject.SetActive(false);
        }
    }

    public float SpecialEffectPlaytime { get { return specialEffectPlaytime; } }
}
