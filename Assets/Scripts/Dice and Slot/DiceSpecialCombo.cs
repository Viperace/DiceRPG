using System.Collections;
using System.Collections.Generic;

public abstract class DiceSpecialCombo
{
    public string name { get; protected set; }
    public string effectDescription { get; protected set; }

    public int[] numberOfDiceRequired { get; protected set; }
    public DiceSpecialCombo() { }

    public bool IsValid(params int[] rolledValue)
    {
        foreach (var n in numberOfDiceRequired)
        {
            if (rolledValue.Length == n)
                return true;
        }
        return false;
    }

    public bool Equal(DiceSpecialCombo other)
    {
        if (this == null & other == null)
            return true;

        if(this == null)
        {
            if (other == null)
                return true;
            else
                return false;
        }
        else
        {
            if (other == null)
                return false;
            else
                return this.name == other.name;
        }
    }

    public abstract bool IsTrigger(params int[] rolledValue);

    public static DiceSpecialCombo GetComboFromEnum(DiceSpecialComboEnum e, int param1 = 0)
    {
        switch (e)
        {
            case DiceSpecialComboEnum.Straight:
                return new Straight();
            case DiceSpecialComboEnum.SumEqual:
                return new SumEqual(param1);
            case DiceSpecialComboEnum.Double:
                return new DoubleCombo();
            case DiceSpecialComboEnum.Triple:
                return new TripleCombo();
            case DiceSpecialComboEnum.FourOfAKindCombo:
                return new FourOfAKindCombo();
            case DiceSpecialComboEnum.AllSameCombo:
                return new AllSameCombo();
            case DiceSpecialComboEnum.NONE:
                return null;
            default:
                return null;
        }
    }

    public static DiceSpecialCombo GetComboFromString(string combo, int param1 = 0)
    {
        System.Enum.TryParse(combo, true, out DiceSpecialComboEnum trialComboEnum);
        return GetComboFromEnum(trialComboEnum, param1);
    }

    public static DiceSpecialComboEnum GetEnum(DiceSpecialCombo combo)
    {
        if (combo is Straight)
            return DiceSpecialComboEnum.Straight;
        else if(combo is DoubleCombo)
            return DiceSpecialComboEnum.Double;
        else if (combo is TripleCombo)
            return DiceSpecialComboEnum.Triple;
        else if (combo is FourOfAKindCombo)
            return DiceSpecialComboEnum.FourOfAKindCombo;
        else if (combo is AllSameCombo)
            return DiceSpecialComboEnum.AllSameCombo;
        else if (combo is SumEqual)
            return DiceSpecialComboEnum.SumEqual;
        else
            return DiceSpecialComboEnum.NONE;       
    }

    public DiceSpecialComboEnum GetEnum()
    {
        return GetEnum(this);
    }
}


/// <summary>
/// Base for double, triple etc
/// </summary>
public abstract class SameNumbers : DiceSpecialCombo
{
    public int numberSame { get; protected set; }
    public override bool IsTrigger(params int[] rolledValue)
    {
        // Check valid
        if (!IsValid(rolledValue)) return false;

        // Chk straight condition
        int count = 1;
        List<int> sortedValue = new List<int>(rolledValue);
        for (int i = 1; i < sortedValue.Count; i++)
            if (sortedValue[i] == sortedValue[i - 1])
                count++;

        bool _test = count >= this.numberSame;
        if ( _test)
            return true;
        else
            return false;
    }
}


/***********************************
 *
 *Actual implementation 
 * 
 *
 **********************************/

/// <summary>
/// Triple!
/// </summary>
public class DoubleCombo : SameNumbers
{
    public DoubleCombo()
    {
        base.numberSame = 2;
        base.name = "Double";
        base.effectDescription = "HP +1";
        base.numberOfDiceRequired = new int[9] { 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    }
}

public class TripleCombo : SameNumbers
{
    public TripleCombo()
    {
        base.numberSame = 3;
        base.name = "Triple";
        base.effectDescription = "Perfect Counter";
        base.numberOfDiceRequired = new int[8] { 3, 4, 5, 6, 7, 8, 9, 10 };
    }
}


/// <summary>
/// 4,4,4,4
/// or 3,3,3,3...
/// </summary>
public class FourOfAKindCombo : SameNumbers
{
    public FourOfAKindCombo()
    {
        base.numberSame = 4;
        base.name = "Quadruple";
        base.effectDescription = "God's Hammer!";
        base.numberOfDiceRequired = new int[7] { 4, 5, 6, 7, 8, 9, 10 };
    }
}

public class AllSameCombo : SameNumbers
{

    public AllSameCombo()
    {
        base.numberSame = 5;  // FIX ME
        base.name = "Identity";
        base.effectDescription = "Hell's Gate!";
        base.numberOfDiceRequired = new int[6] { 5, 6, 7, 8, 9, 10 };
    }
}

/// <summary>
/// Sum =
/// </summary>
public class SumEqual : DiceSpecialCombo
{
    public int targetSum;

    public SumEqual(int targetSum)
    {
        base.numberOfDiceRequired = new int[7] { 3,4,5,6,7,8,9 };
        base.name = "Lucky Sum";
        base.effectDescription = "Perfect Defend";

        this.targetSum = targetSum;
    }

    public override bool IsTrigger(params int[] rolledValue)
    {
        // Check valid
        if (!IsValid(rolledValue)) return false;

        // Sum first and Chk Condition
        int sum = 0;
        for (int i = 0; i < rolledValue.Length; i++)
            sum += rolledValue[i];

        return (sum == targetSum);
    }
}



/// <summary>
/// Base for 1,2,3,4 
/// 5,6,7,8
/// </summary>
public class Straight : DiceSpecialCombo
{
    public Straight()
    {
        base.name = "Straight";
        base.effectDescription = "Impale";

        base.numberOfDiceRequired = new int[7] { 3, 4, 5, 6, 7, 8, 9 };
    }

    public override bool IsTrigger(params int[] rolledValue)
    {
        // Check valid
        if (!IsValid(rolledValue)) return false;

        // Chk straight condition
        List<int> sortedValue = new List<int>(rolledValue);
        sortedValue.Sort();
        for (int i = 1; i < sortedValue.Count; i++)
        {
            if (sortedValue[i] - sortedValue[i - 1] == 1)
                continue;
            else
                return false;
        }

        return true;
    }
}

public enum DiceSpecialComboEnum
{
    NONE = 0,
    Straight,
    SumEqual,
    Double,
    Triple,
    FourOfAKindCombo,
    AllSameCombo,
}