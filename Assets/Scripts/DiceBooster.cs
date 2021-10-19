using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceBooster
{
    public int boostedAttack { get; private set; }
    public int boostedDefend { get; private set; }
    public int chargeLeft { get; private set; }

    public DiceBooster() { }

    public DiceBooster(int chargeLeft) 
    {
        this.chargeLeft = chargeLeft;
        boostedAttack = boostedDefend = 0;
    }

    public bool BoostOnce(DiceSlotEnum slotType)
    {
        if (chargeLeft > 0)
        {
            if (slotType == DiceSlotEnum.ATTACK)
                boostedAttack++;
            else if (slotType == DiceSlotEnum.DEFEND)
                boostedDefend++;

            chargeLeft--;
            return true;
        }
        else
            return false;
    }
    
    public void Reset()
    {
        boostedAttack = 0;
        boostedDefend = 0;
    }
}
