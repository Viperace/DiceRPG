using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GearDiceDatabase : MonoBehaviour
{
    public Dictionary<string, GearDice> GearsDictionary { get; set; }
    public TextAsset myJsonFile;
    public static GearDiceDatabase _instance;
    public static GearDiceDatabase Instance { get { return _instance; } }

    void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
        DontDestroyOnLoad(this);

        GearsDictionary = LoadFromJSON();
    }

    Dictionary<string, GearDice> LoadFromJSON()
    {
        GearDiceJsons itemArray = JsonUtility.FromJson<GearDiceJsons>(myJsonFile.text);

        Dictionary<string, GearDice> myDict = new Dictionary<string, GearDice>();
        foreach (GearDiceJson jsonItem in itemArray.FullGearsTable)
        {
            GearDice g = new GearDice(jsonItem);
            myDict.Add(g.gearName, g);
        }
        return myDict;
    }

    public GearDice GetRandomizeGearData()
    {
        int n = GearsDictionary.Count;
        int roll = Random.Range(0, n);
        int x = 0;
        foreach (var item in GearsDictionary.Values)
        {
            if (x == roll)
                return item;
            x++;
        }
        return null;
    }

    public GearDice GetRandomizeGearInstance()
    {
        GearDice g = GetRandomizeGearData();
        return g.CreateCopy();
    }

    public List<GearDice> GetStartingGearsInstance()
    {
        string[] startingGearStrings = new string[] { "Club", "Plank Shield" };
        List<GearDice> starters = new List<GearDice>();
        foreach (var item in startingGearStrings)
        {
            GearDice g = GearsDictionary[item];
            starters.Add(g.CreateCopy());
        }

        return starters;
    }
}



[System.Serializable]
public class GearDiceJson
{
    public string Name;
    public string Prefab;
    public int Min;
    public int Max;
    public int MaxDurability;
    public int MaxStar;
    public bool CanAttack;
    public bool CanDefend;
    public string Combo;
    public int ComboParam;
    public int GoldCost;
}


[System.Serializable]
public class GearDiceJsons
{
    public GearDiceJson[] FullGearsTable;
}


