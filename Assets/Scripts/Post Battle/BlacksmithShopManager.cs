using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlacksmithShopManager : MonoBehaviour
{
    GameParameter gameParameter;
    GearDiceUIDisplay craftPanel;
    void Start()
    {
        gameParameter = ScriptableObject.CreateInstance<GameParameter>();

        craftPanel = GameObject.Find("CraftPanel").GetComponentInChildren<GearDiceUIDisplay>();
    }
    public void UpradeMaxOnSelectedGear()
    {
        UpgradeMax(craftPanel.RepresentedGearDice);
    }
    public void UpradeMinOnSelectedGear()
    {
        UpgradeMin(craftPanel.RepresentedGearDice);
    }
    public void RepairSelectedGear()
    {
        int cost = gameParameter.RepairCost;
        if (Player.playerStat.Gold >= cost &
            craftPanel.RepresentedGearDice.durability < craftPanel.RepresentedGearDice.maxDurability)
        {
            craftPanel.RepresentedGearDice.durability = craftPanel.RepresentedGearDice.maxDurability;
            Player.playerStat.Gold -= cost;
        }
    }
    public bool UpgradeMax(GearDice gearDice)
    {
        if(gearDice.numberOfStar < gearDice.maxNumberOfStar)
        {
            int cost = gameParameter.UpgradeUpperBoundCost;
            if (Player.playerStat.Gold >= cost)
            {
                gearDice.UpgradeMax(1);
                Player.playerStat.Gold -= cost;
                gearDice.numberOfStar++;
                return true;
            }
        }
        return false;
    }

    public bool UpgradeMin(GearDice gearDice)
    {
        if (gearDice.numberOfStar < gearDice.maxNumberOfStar)
        {
            int cost = gameParameter.UpgradeLowerBoundCost;
            if (Player.playerStat.Gold >= cost)
            {
                gearDice.UpgradeMin(1);
                Player.playerStat.Gold -= cost;
                gearDice.numberOfStar++;
                return true;
            }
        }
        return false;
    }

    public bool BuySlot(bool isAttack)
    {
        int addSlotCost = gameParameter.GetAddSlotCost(Player.playerStat.AttackSlotNumber, Player.playerStat.DefendSlotNumber);

        if (Player.playerStat.Gold >= addSlotCost & 
            !gameParameter.IsSlotSizeReachLimit(Player.playerStat.AttackSlotNumber, Player.playerStat.DefendSlotNumber))
        {
            Player.playerStat.Gold -= addSlotCost;

            if (isAttack)
                Player.playerStat.AttackSlotNumber++;
            else
                Player.playerStat.DefendSlotNumber++;
            return true;
        }
        else
            return false;
    }

    public void BuyDefendSlot()
    {
        BuySlot(false);
    }

    public void BuyAttackSlot()
    {
        BuySlot(true);
    }
}
