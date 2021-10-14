using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Decide what to give player, based on current progress, difficulty
///
/// </summary>
public class RewardManager : MonoBehaviour
{
    ChoiceRewardButton[] buttons;

    void Awake()
    {
        buttons = FindObjectsOfType<ChoiceRewardButton>(true);

        // Disable first
        foreach (var item in buttons)
            item.gameObject.SetActive(false);

        StartCoroutine(InitializeRewards());
    }

    IEnumerator InitializeRewards()
    {
        // Wait till player is ready
        while (Player.journeyLog == null)
            yield return null;

        // Check the journey number
        if(Player.journeyLog.Length == 6)
        {
            GoldReward gold = new GoldReward(50);
            RandomGearReward gear = new RandomGearReward(50);
            MaxHPreward hp = new MaxHPreward();
            SetRewardChoices(gold, gear, hp);
        }
        else if(Player.journeyLog.Length == 14)
        {
            GoldReward gold = new GoldReward(60);
            RandomGearReward gear = new RandomGearReward(60);
            MaxStaminaReward hp = new MaxStaminaReward(10);
            SetRewardChoices(gold, gear, hp);
        }
        else if (Player.journeyLog.Length == 21)
        {
            GoldReward gold = new GoldReward(70);
            RandomGearReward gear = new RandomGearReward(70);
            SetRewardChoices(gold, gear);
        }
        else if (Player.journeyLog.Length == 27)
        {
            GoldReward gold = new GoldReward(75);
            RandomGearReward gear = new RandomGearReward(75);
            MaxHPreward hp = new MaxHPreward();
            SetRewardChoices(gold, gear, hp);
        }
        else
        {
            Debug.LogWarning("Giving random reward, not sure what to give");
            GoldReward gold = new GoldReward(100);
            RandomGearReward gear = new RandomGearReward(100);
            MaxHPreward hp = new MaxHPreward();
            SetRewardChoices(gold, gear, hp);
        }
    }

    public void SetRewardChoices(params ChoiceReward[] rewards )
    {
        // Enable and set choice
        for (int i = 0; i < rewards.Length; i++)
        {
            buttons[i].gameObject.SetActive(true);
            buttons[i].SetReward(rewards[i]);
        }
    }

    public void CloseUnchosedButtons()
    {
        foreach (var item in buttons)
            if (!item.IsChosen)
            {
                item.AnimateClose();

                ParticleSystem[] fxs = item.transform.Find("FX").GetComponentsInChildren<ParticleSystem>();
                foreach (var fx in fxs)
                    fx.Stop();
            }
    }
   

    public void ExitScene()
    {
        StartCoroutine(ExitSceneDelay());
    }

    IEnumerator ExitSceneDelay(float duration = 2)
    {
        yield return new WaitForSeconds(duration);

        MySceneManager.Instance.LoadJourneyScene();
    }
}

public interface ChoiceReward 
{
    public string RewardText { get; }
    public void ApplyReward(PlayerStat player);
}

public class GoldReward : ChoiceReward
{
    int amount;
    public GoldReward(int amount)
    {
        this.amount = amount;
    }

    public string RewardText
    { 
        get { return string.Concat("+", amount); }
    }

    public void ApplyReward(PlayerStat player)
    {
        player.Gold += amount;
    }
}

public class MaxHPreward : ChoiceReward
{
    int amount = 1;
    public MaxHPreward()
    {}

    public string RewardText
    {
        get { return string.Concat("+", amount, " Max"); }
    }

    public void ApplyReward(PlayerStat player)
    {
        player.maxHP++;
        player.HP++;
    }
}
public class MaxStaminaReward : ChoiceReward
{
    int amount;
    public MaxStaminaReward(int amount)
    {
        this.amount = amount;
    }
    public string RewardText
    {
        get { return string.Concat("+", amount, " Max"); }
    }

    public void ApplyReward(PlayerStat player)
    {
        player.maxStamina += amount;
        player.stamina += amount;
    }
}

public class RandomGearReward : ChoiceReward
{
    public GearDice selectedGear { get; private set; }
    int gearValue;    // How much this gear worth (in gold)
    int valueRange = 10;
    public RandomGearReward() { }
    public RandomGearReward(int gearValue) 
    {
        this.gearValue = gearValue;

        // Init
        selectedGear = FindSuitableGear();
    }

    public string RewardText
    {
        get 
        {
            if (selectedGear == null)
                return "";
            else
                return selectedGear.gearName;
        }
    }

    GearDice FindSuitableGear()
    {
        List<GearDice> candidates = new List<GearDice>();
        foreach (var item in GearDiceDatabase.Instance.GearsDictionary.Values)
        {
            if (gearValue <= item.goldCost + valueRange & gearValue >= item.goldCost)
                candidates.Add(item);
        }

        if (candidates.Count == 0)
        {
            // Random select anything below this value
            int maxRange = this.gearValue + 2 * valueRange;
            candidates = new List<GearDice>();
            foreach (var item in GearDiceDatabase.Instance.GearsDictionary.Values)
                if (gearValue <= maxRange)
                    candidates.Add(item);

            if (candidates.Count > 0)
            {
                Debug.LogWarning("Cant find suitable gear. Random select one");

                int roll = Random.Range(0, candidates.Count);
                return candidates[roll];
            }
            else
            {
                Debug.LogWarning("Still Cant find suitable gear. Give Knigh shield");
                return GearDiceDatabase.Instance.GearsDictionary["Knight Shield"];
            }
            
        }
        else
        {
            int roll = Random.Range(0, candidates.Count);
            return candidates[roll];
        }
    }

    public void ApplyReward(PlayerStat player)
    {        
        player.AddDice(selectedGear.CreateCopy());
    }
}