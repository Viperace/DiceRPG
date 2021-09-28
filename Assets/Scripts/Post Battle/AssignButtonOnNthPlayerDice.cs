using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssignButtonOnNthPlayerDice : MonoBehaviour
{
    [SerializeField] GameObject targetPanel;
    GearDiceUIDisplay target;
    Button button;    
    public int N { get; private set; }
    void Start()
    {
        button = GetComponent<Button>();

        N = FindOwnOrder();

        AssignButton();
    }

    void AssignButton()
    {
        //GameObject go = GameObject.Find("CraftPanel");
        GameObject go = targetPanel;
        if (go)
            target = go.GetComponentInChildren<GearDiceUIDisplay>();

        if (N >= 0 && target)
            button.onClick.AddListener(() => target.SetGearDice(N));
    }

    // Update is called once per frame
    int FindOwnOrder()
    {
        PopulateGearsInventoryUI parent = GetComponentInParent<PopulateGearsInventoryUI>();
        GearDiceUIDisplay own = GetComponent<GearDiceUIDisplay>();
        int order = -1;
        if (parent & own)
        {
            GearDiceUIDisplay[] siblings = parent.GetComponentsInChildren<GearDiceUIDisplay>();
            for (int i = 0; i < siblings.Length; i++)
            {
                if (siblings[i] == own)
                    order = i;
            }
        }

        return order;
    }
}
