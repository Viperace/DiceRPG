using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MyMath;

public class ShopManager : MonoBehaviour
{
    GameParameter gameParameter;
    public GameObject inventoryListPanelGO;
    public GameObject buyListPanelGO;    
    GearDiceUIDisplay sellListPanel;
    GearDiceUIDisplay buyListPanel;

    public GameObject sellTargetPanelGO;
    public GameObject buyTargetPanelGO;
    GearDiceUIDisplay sellTargetPanel;
    GearDiceUIDisplay buyTargetanel;
    void Start()
    {
        gameParameter = ScriptableObject.CreateInstance<GameParameter>();

        sellListPanel = inventoryListPanelGO.GetComponentInChildren<GearDiceUIDisplay>();
        buyListPanel = buyListPanelGO.GetComponentInChildren<GearDiceUIDisplay>();

        sellTargetPanel = sellTargetPanelGO.GetComponent<GearDiceUIDisplay>();
        buyTargetanel = buyTargetPanelGO.GetComponent<GearDiceUIDisplay>();

        PopulateItemsForSale();
    }

    // Decide what to sell, based on current player's journey and game difficulty
    public List<GearDice> ItemsForSale { get; private set; }

    void PopulateItemsForSale()
    {
        ItemsForSale = new List<GearDice>();

        // Randomly select 3 items (non-repeated)
        List<GearDice> gears = GearDiceDatabase.Instance.GearsDictionary.Values.ToList();
        gears.Shuffle();

        for (int i = 0; i < 3; i++)
            ItemsForSale.Add(gears[i]);

        //// Randomize
        //for (int i = 0; i < 3; i++)
        //{
        //    GearDice randomItem = GearDiceDatabase.Instance.GetRandomizeGearData();
        //    ItemsForSale.Add(randomItem);
        //}

    }

    void RemoveItemForSale(string gearName)
    {
        GearDice itemToRemove = null;
        foreach (var item in ItemsForSale)
        {
            if (item.gearName == gearName)
            {
                itemToRemove = item;
                break;
            }
        }

        if(itemToRemove !=null)
            ItemsForSale.Remove(itemToRemove);
    }

        
    public void TurnOnBuyPanel()
    {
        buyListPanelGO.gameObject.SetActive(true);
        buyTargetPanelGO.gameObject.SetActive(true);

        inventoryListPanelGO.gameObject.SetActive(false);
        sellTargetPanelGO.gameObject.SetActive(false);
    }
    public void TurnOnSellPanel()
    {
        buyListPanelGO.gameObject.SetActive(false);
        buyTargetPanelGO.gameObject.SetActive(false);

        inventoryListPanelGO.gameObject.SetActive(true);
        sellTargetPanelGO.gameObject.SetActive(true);
    }

    public void ToggleBuyOrSell()
    {
        if (buyListPanelGO.gameObject.activeInHierarchy)
            TurnOnSellPanel();
        else
            TurnOnBuyPanel();
    }

    public void BuySelectedItem()
    {
        if (buyTargetanel.RepresentedGearDice == null)
            return;

        GearDice itemToBuy = buyTargetanel.RepresentedGearDice.CreateCopy();

        if (Player.playerStat.Gold >= itemToBuy.goldCost)
        {
            Player.playerStat.AddDice(itemToBuy);
            Player.playerStat.Gold -= itemToBuy.goldCost;

            // Remove from shop
            RemoveItemForSale(itemToBuy.gearName);

            // Clear from selection
            buyTargetanel.ClearGearDice();
        }
    }

    public void SellSelectedItem()
    {
        GearDice itemToSell = sellTargetPanel.RepresentedGearDice;
        //int cost = Mathf.RoundToInt(gameParameter.SellGearDiscount * itemToSell.goldCost);
        int sellPrice = gameParameter.GetSellPrice(itemToSell.goldCost);

        if (Player.playerStat.RemoveDice(itemToSell))
        {
            Player.playerStat.Gold += sellPrice;
            sellTargetPanel.ClearGearDice();            
        }
        else
            Debug.Log("Cant sell!");

    }
}
