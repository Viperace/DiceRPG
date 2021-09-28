using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _TestDebugFunction : MonoBehaviour
{
    [SerializeField] InputField inputField;
    public void MinusEnemyHP()
    {
        EnemyStat e = GameManager.Instance.currentEnemy;
        e.HP--;
    }

    public void KillEnemy()
    {
        EnemyStat e = GameManager.Instance.currentEnemy;
        e.HP = 0;
    }

    public void MinusPlayerHP()
    {
        Player.playerStat.HP--;
    }

    public void PlusPlayerHP() 
    {
        Player.playerStat.HP++;
        Player.playerStat.HP = Mathf.Clamp(Player.playerStat.HP, 0, Player.playerStat.maxHP);
    }

    public void MinusPlayerStamina()
    {
        Player.playerStat.stamina--;
    }

    [SerializeField] GameObject winPanel;
    public void ShowWinScreen()
    {
        winPanel.SetActive(true);
    }
    
    public void RevealAllDestinationChoices()
    {
        GameObject x = GameObject.Find("ChoiceButtonGroups");
        RectTransform[] buttons = x.GetComponentsInChildren<RectTransform>(true);
        foreach (var item in buttons)
            item.gameObject.SetActive(true);
    }

    public void AddPlayerSlot()
    {
        Player.playerStat.AttackSlotNumber++;
        Player.playerStat.DefendSlotNumber++;

    }

    public void Add1000Coins()
    {
        Player.playerStat.Gold += 1000;
    }

    public void ShuffleRandomWeaponToPlayer()
    {
        List<GearDice> newGears = new List<GearDice>();
        for (int i = 0; i < 5; i++)
        {
            GearDice g = GearDiceDatabase.Instance.GetRandomizeGearInstance();
            newGears.Add(g);
        }
        Player.playerStat.SetDice(newGears.ToArray());
    }

    public void AttachRandomComboToGears()
    {
        List<DiceSpecialCombo> comboPool = new List<DiceSpecialCombo>();
        comboPool.Add(new DoubleCombo());
        comboPool.Add(new TripleCombo());
        comboPool.Add(new Straight());
        comboPool.Add(new FourOfAKindCombo());
        comboPool.Add(new AllSameCombo());
        comboPool.Add(new SumEqual(6));

        int roll_g = Random.Range(0, Player.playerStat.gearDices.Count);
        int roll_combo = Random.Range(0, comboPool.Count);
        Player.playerStat.gearDices[roll_g].SetCombo(comboPool[roll_combo]);
    }

    
}
