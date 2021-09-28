using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShrineEffect
{
    public string godName;
    public string prefabName;

    public DiceSpecialCombo combo;

    public static List<ShrineEffect> AllEffects
    {
        get
        {
            List<ShrineEffect> allEffects = new List<ShrineEffect>();
            allEffects.Add(new Eir());
            allEffects.Add(new Freyr());
            allEffects.Add(new Heimdall());
            allEffects.Add(new Forseti());
            allEffects.Add(new Hel());
            return allEffects;
        }
    }

    public static List<string> AllEffectNames
    {
        get
        {
            List<string> allEffects = new List<string>();
            allEffects.Add(new Eir().godName);
            allEffects.Add(new Freyr().godName);
            allEffects.Add(new Heimdall().godName);
            allEffects.Add(new Forseti().godName);
            allEffects.Add(new Hel().godName);
            return allEffects;
        }
    }
}

public class Eir : ShrineEffect
{
    public Eir()
    {
        godName = "Eir";
        prefabName = "SM_Prop_Statue_Angel_01";
        combo = new DoubleCombo();
    }
}

public class Freyr : ShrineEffect
{
    public Freyr()
    {
        godName = "Freyr";
        prefabName = "SM_Prop_Statue_Hero_01";
        combo = new TripleCombo();
    }
}

public class Heimdall : ShrineEffect
{
    public Heimdall()
    {
        godName = "Heimdall";
        prefabName = "SM_Prop_Statue_Knight_01";
        combo = new SumEqual(SetASuitableNumber());
    }
    public Heimdall(int sumNumber)
    {
        godName = "Heimdall";
        prefabName = "SM_Prop_Statue_Knight_01";
        combo = new SumEqual(sumNumber);
    }

    /// <summary>
    /// Based on number of dices player has, and their min/max, give an average
    /// </summary>
    /// <returns></returns>
    int SetASuitableNumber()
    {
        int min = 0;
        int max = 0;
        foreach (var g in Player.playerStat.gearDices)
        {
            min += g.minValue;
            max += g.maxValue;
        }

        // Take average and give a range
        int variance = Player.playerStat.gearDices.Count;
        int ave = Mathf.RoundToInt(0.5f * (min + max));
        int output = Random.Range(ave - 1, ave + variance);
        return output;
    }
}

public class Forseti : ShrineEffect
{
    public Forseti()
    {
        godName = "Forseti";
        prefabName = "SM_Prop_Statue_King_01";
        combo = new Straight();
    }
}

public class Hel : ShrineEffect
{
    public Hel()
    {
        godName = "Hel";
        prefabName = "SM_Env_Statue_01";
        combo = new FourOfAKindCombo();
    }
}

