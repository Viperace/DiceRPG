using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JourneyLog
{
    public int numberOfEncounter { get; private set; }  // Number of level passed. Start with 0
    public List<JourneyEnum> log { get; private set; }
    public int numberOfKill { get; private set; }
    public Dictionary<int, List<JourneyEnum>> fullTree { get; private set; }

    public JourneyLog() 
    {
        log = new List<JourneyEnum>();
        numberOfEncounter = 0;
        numberOfKill = 0;
        fullTree = InitializeFullJourneyTree();
    }

    public int Length
    {
        get
        {
            return log.Count;
        }
    }

    public JourneyEnum LatestEntry
    {
        get { return log[log.Count - 1]; }
    }

    public void AddUserSelectedJourney(JourneyEnum journeyEnum)
    {
        log.Add(journeyEnum);
        numberOfEncounter++;
    }

    public List<JourneyEnum> GenerateRandomNextDestinations()
    {
        List<JourneyEnum> destination = new List<JourneyEnum>();

        // 100% 
        // Scripted event  
        List<int> bossNumbers = new List<int>();
        bossNumbers.Add(3);
        bossNumbers.Add(6);
        bossNumbers.Add(9);
        if (bossNumbers.Contains(numberOfEncounter))
            destination.Add(JourneyEnum.BossEncounter);
        else
            destination.Add(JourneyEnum.CombatEncounter);

        // Roll
        float rollHasShop = Random.value;
        if(rollHasShop > 0.5f)
        {
            int rollShopType = Random.Range(0, 2);
            if (rollShopType == 0)
                destination.Add(JourneyEnum.Tavern);
            else
                destination.Add(JourneyEnum.Blacksmith);
        }
        

        return destination;
    }

    /// <summary>
    /// Function to initiate full 25 level choices
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, List<JourneyEnum>> InitializeFullJourneyTree()
    {
        Dictionary<int, List<JourneyEnum>> fullTree = new Dictionary<int, List<JourneyEnum>>();

        // Make the base
        for (int i = 1; i < 27; i++)
        {
            List<JourneyEnum> choice = new List<JourneyEnum>();
            choice.Add(JourneyEnum.CombatEncounter);
            fullTree.Add(i, choice);
        }

        // Replace certain point of interest
        fullTree[6] = new List<JourneyEnum>(new List<JourneyEnum>(new JourneyEnum[1] { JourneyEnum.ChoiceReward }));
        fullTree[14] = new List<JourneyEnum>(new List<JourneyEnum>(new JourneyEnum[1] { JourneyEnum.ChoiceReward }));
        fullTree[21] = new List<JourneyEnum>(new List<JourneyEnum>(new JourneyEnum[1] { JourneyEnum.ChoiceReward }));

        fullTree[3] = new List<JourneyEnum>(new List<JourneyEnum>(new JourneyEnum[2] { JourneyEnum.Shop, JourneyEnum.Blacksmith }));
        //fullTree[7] = new List<JourneyEnum>(new List<JourneyEnum>(new JourneyEnum[1] {JourneyEnum.Blacksmith }));
        fullTree[7] = new List<JourneyEnum>(new List<JourneyEnum>(new JourneyEnum[3] { JourneyEnum.CombatEncounter, JourneyEnum.Tavern, JourneyEnum.Blacksmith }));
        fullTree[10] = new List<JourneyEnum>(new List<JourneyEnum>(new JourneyEnum[1] { JourneyEnum.Shrine }));
        fullTree[15] = new List<JourneyEnum>(new List<JourneyEnum>(new JourneyEnum[2] {  JourneyEnum.Shop, JourneyEnum.Blacksmith }));
        fullTree[19] = new List<JourneyEnum>(new List<JourneyEnum>(new JourneyEnum[3] { JourneyEnum.Tavern, JourneyEnum.Blacksmith, JourneyEnum.Shrine }));
        fullTree[23] = new List<JourneyEnum>(new List<JourneyEnum>(new JourneyEnum[2] { JourneyEnum.Tavern, JourneyEnum.CombatEncounter}));
        fullTree[25] = new List<JourneyEnum>(new List<JourneyEnum>(new JourneyEnum[2] { JourneyEnum.Shop, JourneyEnum.CombatEncounter }));
        fullTree[26] = new List<JourneyEnum>(new List<JourneyEnum>(new JourneyEnum[2] { JourneyEnum.Tavern, JourneyEnum.Blacksmith }));
        fullTree[27] = new List<JourneyEnum>(new List<JourneyEnum>(new JourneyEnum[1] { JourneyEnum.BossEncounter}));

        return fullTree;
    }

    public List<JourneyEnum> GetNextDestinations()
    {
        return fullTree[log.Count + 1];
    }

    public void AddNumerOfKill() => numberOfKill++;

    public bool IsRewardTime()
    {
        int journeyIndex = this.Length;
        foreach (var item in fullTree[journeyIndex])
            if (item == JourneyEnum.ChoiceReward)
                return true;
        return false;
    }
}

public enum JourneyEnum
{
    CombatEncounter,
    BossEncounter,
    Tavern,
    Shop,
    Shrine,
    Blacksmith,
    ChoiceReward
}