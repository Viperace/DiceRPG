using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Fill player slot
/// </summary>
public class CombatSlotsUI : MonoBehaviour
{
    List<Image> slotImages;
    Button addSlotButton;
    [SerializeField] bool isAttack = false;  // attack or defend?

    void Start()
    {
        addSlotButton = GetComponentInChildren<Button>();

        slotImages = new List<Image>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform c = transform.GetChild(i);
            if(! c.GetComponent<Button>()) // Don't want button
                slotImages.Add(c.GetComponent<Image>());
        }
    }

    void PopulateImages()
    {
        int n = isAttack ? Player.playerStat.AttackSlotNumber : Player.playerStat.DefendSlotNumber;
       
        for (int i = 0; i < slotImages.Count; i++)
        {
            if (i < n)
                slotImages[i].gameObject.SetActive(true);
            else
                slotImages[i].gameObject.SetActive(false);
        }
    }

    // If already reach max 5 size or 3 each
    void TurnOffAddSlotButton()
    {
        if (isAttack & Player.playerStat.AttackSlotNumber >= 3)
            addSlotButton.interactable = false;

        if (!isAttack & Player.playerStat.DefendSlotNumber >= 3)
            addSlotButton.interactable = false;

        if (Player.playerStat.AttackSlotNumber + Player.playerStat.DefendSlotNumber >= 5)
            addSlotButton.interactable = false;
    }

    void Update()
    {
        if (Player.playerStat != null)
        {
            PopulateImages();
            TurnOffAddSlotButton();
        }

    }
}
