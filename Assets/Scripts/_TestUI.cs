using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _TestUI : MonoBehaviour
{
    public GameObject craftPanel;
    public void _CycleNextGear()
    {
        if (craftPanel)
        {
            GearDiceUIDisplay ui = craftPanel.GetComponentInChildren<GearDiceUIDisplay>();
            ui.SetNextPlayerGearDice();
        }
    }

    public void _ReduceDurability()
    {
        if (craftPanel)
        {
            foreach (var g in Player.playerStat.gearDices)
            {
                g.durability -= 2;
                g.durability = Mathf.Clamp(g.durability, 0, 100);
            }
        }
    }

}
