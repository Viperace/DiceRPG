using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameParam", menuName = "ScriptableObjects/GameParam", order = 1)]
public class GameParameter : ScriptableObject
{
    public int RepairCost = 10;
    public int UpgradeUpperBoundCost = 10;
    public int UpgradeLowerBoundCost = 8;
    public int InnRestingCost = 20;
    public float SellGearDiscount = 0.5f;

    public int MaxAttackSlot = 3;
    public int MaxDefendSlot = 3;

    //--- Shrine
    public int PrayingCost = 100;

    // Game Engine
    public float AnimationDurationMod = 1f;

    public int GetSellPrice(int goldCost)
    {
        return Mathf.RoundToInt(SellGearDiscount * goldCost);
    }

    public int GetAddSlotCost(int currentAttackSlot, int currentDefendSlot)
    {
        int n = currentAttackSlot + currentDefendSlot;
        if (n <= 2)
            return 20;
        else if (n == 3)
            return 25;
        else
            return 35;
    }
    public bool IsSlotSizeReachLimit(int currentAttackSlot, int currentDefendSlot)
    {
        return currentAttackSlot + currentDefendSlot >= 5;
    }
}
