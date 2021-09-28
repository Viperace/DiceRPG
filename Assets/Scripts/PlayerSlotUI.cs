using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlotUI : MonoBehaviour
{
    List<SlotType> attackSlots;
    List<SlotType> defendSlots;
    [SerializeField] float updatePeriod = 0.5f;
    void Awake()
    {
        SlotType[] slots = GetComponentsInChildren<SlotType>();

        // Find out which one is attk vs defend
        attackSlots = new List<SlotType>();
        defendSlots = new List<SlotType>();
        foreach (var item in slots)
        {
            if (item.slotType == DiceSlotEnum.ATTACK)
                attackSlots.Add(item);
            else if(item.slotType == DiceSlotEnum.DEFEND)
                defendSlots.Add(item);
        }

        // Turn off first
        foreach (var item in slots)
            item.gameObject.SetActive(false);
    }

    float _cooldown = 0;
    void Update()
    {
        if (Player.playerStat != null && _cooldown < 0)
        {
            for (int i = 0; i < Player.playerStat.AttackSlotNumber; i++)
                attackSlots[i].gameObject.SetActive(true);

            for (int i = 0; i < Player.playerStat.DefendSlotNumber; i++)
                defendSlots[i].gameObject.SetActive(true);

            _cooldown = updatePeriod;
        }
        _cooldown -= Time.deltaTime;
    }
}
