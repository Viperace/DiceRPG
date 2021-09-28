using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulateGearsInventoryUI : MonoBehaviour
{
    GearDiceUIDisplay[] uIDisplays;
    Dictionary<GearDiceUIDisplay, int> orderCorrespondToPlayerDice;
    void Start()
    {
        uIDisplays = GetComponentsInChildren<GearDiceUIDisplay>();

        orderCorrespondToPlayerDice = new Dictionary<GearDiceUIDisplay, int>();
        for (int i = 0; i < uIDisplays.Length; i++)
            orderCorrespondToPlayerDice.Add(uIDisplays[i], i);
    }

    void Populate()
    {
        if(Player.playerStat != null)
        {
            for (int i = 0; i < uIDisplays.Length; i++)
            {
                if (i < Player.playerStat.gearDices.Count)
                {
                    uIDisplays[i].gameObject.SetActive(true);
                    uIDisplays[i].SetGearDice(Player.playerStat.gearDices[i]);
                }
                else
                    uIDisplays[i].gameObject.SetActive(false);
            }

        }
    }

    void Update()
    {
        Populate();
    }
}
