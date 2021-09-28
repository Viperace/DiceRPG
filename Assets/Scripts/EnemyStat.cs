using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStat : EntityWithHP
{
    public int maxHP { get; set; }
    public int HP { get; set; }
    public string name { get; private set; }
    public MonsterEnum monsterEnum { get; private set; }
    public List<GearDice> gearDices { get; private set; }
    public Reward reward { get; private set; }
    public EnemyStat() { }

    public void SetDice(params GearDice[] dices)
    {
        gearDices = new List<GearDice>(dices);
    }

    public void SetReward(Reward r) => reward = r;

    public static EnemyStat CreateTestEnemy()
    {
        EnemyStat enemy = new EnemyStat();
        enemy.HP = enemy.maxHP = 3;

        GearDice attackDice = new GearDice(3, 5, 0, 0, DiceSlotEnum.ATTACK);
        GearDice defendDice = new GearDice(3, 4, 0, 0, DiceSlotEnum.DEFEND);
        enemy.SetDice(attackDice, defendDice);

        return enemy;
    }

    public static EnemyStat CreateNewEnemy(MonsterEnum monsterEnum)
    {
        EnemyStat enemy = new EnemyStat();
        GearDice attackDice = null;
        GearDice defendDice = null;
        enemy.name = monsterEnum.ToString();
        enemy.monsterEnum = monsterEnum;
        switch (monsterEnum)
        {
            case MonsterEnum.Slime:
                enemy.HP = enemy.maxHP = 2;
                attackDice = new GearDice(1, 3, 0, 0, DiceSlotEnum.ATTACK);
                defendDice = new GearDice(1, 2, 0, 0, DiceSlotEnum.DEFEND);
                break;
            case MonsterEnum.TurtleShell:
                enemy.HP = enemy.maxHP = 2;
                attackDice = new GearDice(1, 3, 0, 0, DiceSlotEnum.ATTACK);
                defendDice = new GearDice(2, 3, 0, 0, DiceSlotEnum.DEFEND);
                break;
            case MonsterEnum.Bat:
                enemy.HP = enemy.maxHP = 3;
                attackDice = new GearDice(2, 4, 0, 0, DiceSlotEnum.ATTACK);
                defendDice = new GearDice(2, 4, 0, 0, DiceSlotEnum.DEFEND);
                break;
            case MonsterEnum.MonsterPlant:
                enemy.HP = enemy.maxHP = 4;
                attackDice = new GearDice(2, 4, 0, 0, DiceSlotEnum.ATTACK);
                defendDice = new GearDice(1, 6, 0, 0, DiceSlotEnum.DEFEND);
                break;
            case MonsterEnum.Spider:
                enemy.HP = enemy.maxHP = 4;
                attackDice = new GearDice(2, 6, 0, 0, DiceSlotEnum.ATTACK);
                defendDice = new GearDice(2, 4, 0, 0, DiceSlotEnum.DEFEND);
                break;
            case MonsterEnum.Skeleton:
                enemy.HP = enemy.maxHP = 4;
                attackDice = new GearDice(1, 5, 0, 0, DiceSlotEnum.ATTACK);
                defendDice = new GearDice(3, 6, 0, 0, DiceSlotEnum.DEFEND);
                break;
            case MonsterEnum.Orc:
                enemy.HP = enemy.maxHP = 5;
                attackDice = new GearDice(2, 7, 0, 0, DiceSlotEnum.ATTACK);
                defendDice = new GearDice(1, 7, 0, 0, DiceSlotEnum.DEFEND);
                break;
            case MonsterEnum.Golem:
                enemy.HP = enemy.maxHP = 5;
                attackDice = new GearDice(2, 6, 0, 0, DiceSlotEnum.ATTACK);
                defendDice = new GearDice(2, 7, 0, 0, DiceSlotEnum.DEFEND);
                break;
            case MonsterEnum.Dragon:
                enemy.HP = enemy.maxHP = 6;
                attackDice = new GearDice(1, 8, 0, 0, DiceSlotEnum.ATTACK);
                defendDice = new GearDice(2, 7, 0, 0, DiceSlotEnum.DEFEND);
                break;
            default:
                break;
        }

        enemy.SetDice(attackDice, defendDice);
        enemy.SetReward(EnemyStat.GetReward(monsterEnum));

        return enemy;
    }

    public static Reward GetReward(MonsterEnum monsterEnum)
    {
        Reward reward = new Reward();
        switch (monsterEnum)
        {
            case MonsterEnum.Slime:
                reward.gold = 10;
                break;
            case MonsterEnum.TurtleShell:
                reward.gold = 12;
                break;
            case MonsterEnum.Bat:
                reward.gold = 15;
                break;
            case MonsterEnum.MonsterPlant:
                reward.gold = 18;
                break;
            case MonsterEnum.Spider:
                reward.gold = 20;
                break;
            case MonsterEnum.Skeleton:
                reward.gold = 24;
                break;
            case MonsterEnum.Orc:
                reward.gold = 28;
                break;
            case MonsterEnum.Golem:
                reward.gold = 30;
                break;
            case MonsterEnum.Dragon:
                reward.gold = 35;
                break;
            default:
                break;
        }

        reward.gold = Mathf.RoundToInt( reward.gold * Random.Range(0.85f, 1.15f));
        return reward;
    }
}
