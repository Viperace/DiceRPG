using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulateShopItemsUI : MonoBehaviour
{
    GearDiceUIDisplay[] uIDisplays;
    ShopManager shopManager;
    void Start()
    {
        uIDisplays = GetComponentsInChildren<GearDiceUIDisplay>();
        shopManager = FindObjectOfType<ShopManager>();
    }

    void Populate()
    {
        if(shopManager.ItemsForSale != null) 
        { 
            for (int i = 0; i < uIDisplays.Length; i++)
            {
                if (i < shopManager.ItemsForSale.Count)
                {
                    uIDisplays[i].gameObject.SetActive(true);
                    uIDisplays[i].SetGearDice(shopManager.ItemsForSale[i]);
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
