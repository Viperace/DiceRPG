using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceComboEffect
{
    DiceSpecialCombo combo;
    PlayerStat player;
    EnemyStat enemy;
    Dictionary<DiceSlotEnum, int> playerArrangedOutcome;
    public DiceComboEffect(DiceSpecialCombo combo, PlayerStat player, EnemyStat enemy,
        ref Dictionary<DiceSlotEnum, int> playerArrangedOutcome)
    {
        this.combo = combo;
        this.player = player;
        this.enemy = enemy;
        this.playerArrangedOutcome = playerArrangedOutcome;
    }


    public bool IsEffectImmediate()
    {
        if (combo is DoubleCombo | combo is FourOfAKindCombo)
            return true;
        else
            return false;
    }

    public void ApplyEffect()
    {
        if (combo is DoubleCombo)
        {
            //if(player.stamina < player.maxStamina)
            //    player.stamina++;
            player.HP += 1;
            player.HP = Mathf.Clamp(player.HP, 0, player.maxHP);
        }
        if (combo is TripleCombo) //Impale + Defend
        {
            playerArrangedOutcome[DiceSlotEnum.ATTACK] += 99;
            playerArrangedOutcome[DiceSlotEnum.DEFEND] += 99;
        }
        else if (combo is FourOfAKindCombo) // Immediate kill
        {
            enemy.HP = 0;
        }
        else if (combo is SumEqual) // Perfect defend
        {
            playerArrangedOutcome[DiceSlotEnum.DEFEND] += 99;
        }
        else if (combo is Straight) // Impale
        {
            playerArrangedOutcome[DiceSlotEnum.ATTACK] += 99;
        }
    }
}


public class DiceComboEffectView
{
    SlotType[] playerSlots;
    
    public DiceComboEffectView(SlotType[] playerSlots)
    {
        this.playerSlots = playerSlots;
    }

    public void ApplyEffect(DiceSpecialCombo combo)
    {
        if (combo is DoubleCombo)
        {
            // Play something about stamina
        }
        else if (combo is TripleCombo) //Penetration + Defend
        {
            PlayAllSlotsGlow();
        }
        else if (combo is FourOfAKindCombo) // Immediate kill
        {
            
        }
        else if (combo is SumEqual)
        {
            PlayDefendSlotsGlow();
        }
        else if (combo is Straight)
        {
            PlayAttackSlotsGlow();
        }
    }

    void PlayAttackSlotsGlow()
    {
        foreach (var s in playerSlots)
            if (s.slotType == DiceSlotEnum.ATTACK)
            {
                ParticleFXcontroller fx = s.GetComponent<ParticleFXcontroller>();
                fx.PlayGlowYellow();
            }
    }

    void PlayDefendSlotsGlow()
    {
        foreach (var s in playerSlots)
            if (s.slotType == DiceSlotEnum.DEFEND)
            {
                ParticleFXcontroller fx = s.GetComponent<ParticleFXcontroller>();
                fx.PlayGlowBlue();
            }
    }

    void PlayAllSlotsGlow()
    {
        PlayAttackSlotsGlow();
        PlayDefendSlotsGlow();
    }
}
