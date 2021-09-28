using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnSwordOrShieldUI : MonoBehaviour
{
    GearTextNumber gearTextNumber;
    WiggleTween[] childItems;

    void Start()
    {
        gearTextNumber = GetComponent<GearTextNumber>();
        childItems = GetComponentsInChildren<WiggleTween>(true);

        //Invoke("ShowShieldOrSwordOrBoth", 1f);
        StartCoroutine(ShowShieldOrSwordOrBoth(1f));
    }

    IEnumerator ShowShieldOrSwordOrBoth(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (gearTextNumber && gearTextNumber.LinkedGearDice != null)
        {
            List<DiceSlotEnum> dices = gearTextNumber.LinkedGearDice.compatibleSlots;

            if (dices.Count > 1)
                TurnOnGameObject("SwordAndShield");
            else if (dices[0] == DiceSlotEnum.ATTACK)
                TurnOnGameObject("Sword");
            else if (dices[0] == DiceSlotEnum.DEFEND)
                TurnOnGameObject("Shield");
        }
    }

    void TurnOnGameObject(string name)
    {
        foreach (var item in childItems)
        {
            if (item.name == name)
                item.gameObject.SetActive(true);
            else
                item.gameObject.SetActive(false);
        }
    }
}
