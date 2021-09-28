using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearDiceBehavior : MonoBehaviour
{
    int _rank;
    List<GearDice> playerDices;
    public GearDice RepresentedDice { get; set; }

    public GearTextNumber LinkedArsenalDice { get; private set; }

    public Material[] materials;

    void Start()
    {
        StartCoroutine(Initialize());
    }

    IEnumerator Initialize()
    {
        yield return new WaitForSeconds(1);

        while(playerDices == null) 
        {
            if(Player.playerStat == null)
                yield return null;
            else 
            { 
                // Cache player dice list
                playerDices = Player.playerStat.gearDices;
                FindRepresentedDice();
                break;
            }
        }
    }

    void FindRepresentedDice()
    {
        // Find representation
        _rank = FindOwnRank();

        if(_rank < playerDices.Count) 
        { 
            RepresentedDice = playerDices[_rank];
            InitializeColor();
        }
        else
        {
            this.gameObject.SetActive(false);
        }
        
    }

    // Find all player dices and determine its own ranking
    int FindOwnRank()
    {
        GearDiceBehavior[] dices = FindObjectsOfType<GearDiceBehavior>();
        for (int i = 0; i < dices.Length; i++)
            if (dices[i] == this)
            {
                return i;
            }
        Debug.LogError("Cant find rank");
        return -1;
    }

    // Depending on what the representative dice is, assign corresponding color
    void InitializeColor()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        if(RepresentedDice.compatibleSlots.Count > 1)
            meshRenderer.material = materials[2];
        else
        {
            if(RepresentedDice.compatibleSlots[0] == DiceSlotEnum.ATTACK)
                meshRenderer.material = materials[0];
            else
                meshRenderer.material = materials[1];
        }


    }

    public void SetLinkedArsenalDice(GearTextNumber g)
    {
        LinkedArsenalDice = g;
    }

    void Update()
    {
        
    }
}

public class GearDice : ScriptableObject
{
    public string gearName;
    public string prefabName;
    public int minValue;
    public int maxValue;
    public int upgradeOnMinValue { get; private set; }
    public int upgradeOnMaxValue { get; private set; }
        
    public int durability;
    public int maxDurability;
    public int numberOfStar;
    public int maxNumberOfStar;
    public int goldCost;
    public List<DiceSlotEnum> compatibleSlots { get; set; }

    public DiceSpecialCombo combo { get; private set; }

    int _rank;
    
    public GearDice()
    {
        upgradeOnMinValue = 0;
        upgradeOnMaxValue = 0;
        minValue = 1;
        maxValue = 2;
        durability = maxDurability = 10;
        goldCost = 1;

        compatibleSlots = new List<DiceSlotEnum>();
        compatibleSlots.Add(DiceSlotEnum.ATTACK);
    }

    public GearDice(int minValue, int maxValue, 
        int upgradeOnMinValue, int upgradeOnMaxValue,
        int durability, params DiceSlotEnum[] slots)
    {
        this.upgradeOnMinValue = upgradeOnMinValue;
        this.upgradeOnMaxValue = upgradeOnMaxValue;
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.maxDurability = this.durability = durability;
        this.maxNumberOfStar = 3;
        this.numberOfStar = 0;

        compatibleSlots = new List<DiceSlotEnum>(slots);
    }

    public GearDice(int minValue, int maxValue,
        int upgradeOnMinValue, int upgradeOnMaxValue,
        params DiceSlotEnum[] slots)
    {
        this.upgradeOnMinValue = upgradeOnMinValue;
        this.upgradeOnMaxValue = upgradeOnMaxValue;
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.maxDurability = this.durability = 10;
        this.maxNumberOfStar = 3;
        this.numberOfStar = 0;

        compatibleSlots = new List<DiceSlotEnum>(slots);
    }

    public GearDice(GearDiceJson json)
    {
        this.gearName = json.Name;
        this.prefabName = json.Prefab;
        this.minValue = json.Min;
        this.maxValue = json.Max;
        this.upgradeOnMinValue = 0;
        this.upgradeOnMaxValue = 0;
        this.durability = this.maxDurability = json.MaxDurability;
        this.numberOfStar = 0;
        this.maxNumberOfStar = json.MaxStar;
        this.goldCost = json.GoldCost;

        this.compatibleSlots = new List<DiceSlotEnum>();
        if (json.CanAttack) this.compatibleSlots.Add(DiceSlotEnum.ATTACK);
        if (json.CanDefend) this.compatibleSlots.Add(DiceSlotEnum.DEFEND);

        this.combo = DiceSpecialCombo.GetComboFromString(json.Combo, json.ComboParam);        
    }

    public void UpgradeMax(int x) => upgradeOnMaxValue += x;
    public void UpgradeMin(int x)
    {
        upgradeOnMinValue += x;

        // Check if Min + Upgrded min > Max + upgraded max. If so, must raise
        if (minValue + upgradeOnMinValue > maxValue + upgradeOnMaxValue)
            maxValue = minValue + upgradeOnMinValue - upgradeOnMaxValue;
    }

    public void SetCombo(DiceSpecialCombo x)
    {
        this.combo = x;
    }

    public int Roll()
    {
        int roll = Random.Range(minValue + upgradeOnMinValue, maxValue + upgradeOnMaxValue + 1);

        if (durability > 0)
            return roll;
        else
        { //If low dur, need to take min
            int roll2 = minValue + upgradeOnMinValue;
            return roll2;
        }

    }

    public bool CompatibleWith(DiceSlotEnum slot)
    {
        foreach (var item in compatibleSlots)
            if (item == slot)
                return true;
        return false;
    }

    public int effectiveMinValue
    {
        get { return minValue + upgradeOnMinValue; } 
    }
    public int effectiveMaxValue
    {
        get { return maxValue + upgradeOnMaxValue; }
    }

    public GearDice CreateCopy()
    {
        var gCopy = Instantiate(this);
        gCopy.compatibleSlots = new List<DiceSlotEnum>(this.compatibleSlots.ToArray()); // List cant be copy directly !
        gCopy.SetCombo(this.combo);
        return gCopy;
    }
}


public enum DiceSlotEnum
{
    ATTACK,
    DEFEND
}