using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssignButtonOnNthShopItem : MonoBehaviour
{
    [SerializeField] GameObject targetPanel;
    GearDiceUIDisplay target;
    Button button;
    ShopManager shopManager;
    public int N { get; private set; }
    void Start()
    {
        button = GetComponent<Button>();
        shopManager = FindObjectOfType<ShopManager>();

        N = FindOwnOrder();

        AssignButton();
    }

    int FindOwnOrder()
    {
        // Find which child this object belong to
        AssignButtonOnNthShopItem[] siblings = transform.parent.GetComponentsInChildren<AssignButtonOnNthShopItem>();
        for (int i = 0; i < siblings.Length; i++)
            if (siblings[i] == this)
                return i;
        return -1;
    }

    void AssignButton()
    {
        GameObject go = targetPanel;
        if (go)
            target = go.GetComponentInChildren<GearDiceUIDisplay>();
        
        if (N >= 0 && target)
            button.onClick.AddListener(() => target.SetGearDice(shopManager.ItemsForSale[N]));
    }

}
