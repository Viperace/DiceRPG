using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    static Player _instance;
    public static Player Instance { get { return _instance; } }
    public static PlayerStat playerStat { get; private set; }
    public static JourneyLog journeyLog { get; private set; }

    void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
        {
            DontDestroyOnLoad(this.gameObject);
            _instance = this;
        }
    }

    void Start()
    {
        StartCoroutine(InitPlayer(0.5f));
    }

    IEnumerator InitPlayer(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (playerStat == null)
            playerStat = PlayerStat.CreateNewPlayer();

        if (journeyLog == null)
            journeyLog = new JourneyLog();
    }

    public void RetirePlayer()
    {
        // TODO: Save player
        Debug.Log("To do save player");

        // Do kill player
        playerStat = PlayerStat.CreateNewPlayer();
        journeyLog = new JourneyLog();

        // Delete map
        PlayerMapHolder map = FindObjectOfType<PlayerMapHolder>(true);
        if(map)
            Destroy(map.gameObject);
    }

    public void RevivePlayer()
    {
        // Refresh energy if too low
        if (Player.playerStat.stamina < 10)
            Player.playerStat.stamina = 10;

        // Reload
        MySceneManager.Instance.LoadBattleScene();
    }
}

public class PlayerStat : EntityWithHP
{
    public int maxHP { get; set; }
    public int HP { get; set; }
    public int maxStamina { get; set; }
    public int stamina { get; set; }
    public List<GearDice> gearDices { get; private set; }
    public int Gold { get; set; }
    public int AttackSlotNumber { get; set; }
    public int DefendSlotNumber { get; set; }

    public PlayerStat() { }

    public void SetDice(params GearDice[] dices)
    {
        gearDices = new List<GearDice>(dices);
    }

    public void AddDice(GearDice dice)
    {
        gearDices.Add(dice);
    }
    public bool RemoveDice(GearDice dice)
    {
        if (gearDices.Contains(dice))
        {
            gearDices.Remove(dice);
            return true;
        }
        else
            return false;
    }

    public static PlayerStat CreateNewPlayer()
    {
        PlayerStat player = new PlayerStat();
        player.HP = player.maxHP = 3;
        player.stamina = player.maxStamina = 25;
        player.Gold = 20;
        player.AttackSlotNumber = 1;
        player.DefendSlotNumber = 1;


        if (false) // TO DELETE:
        {
            GearDice attackDice = new GearDice(2, 4, 0, 0, 3, DiceSlotEnum.ATTACK, DiceSlotEnum.DEFEND);
            //attackDice.SetCombo(new SumEqual(7));
            attackDice.SetCombo(new DoubleCombo());

            GearDice defendDice = new GearDice(2, 4, 0, 0, 1, DiceSlotEnum.ATTACK, DiceSlotEnum.DEFEND);
            defendDice.SetCombo(new Straight());

            GearDice extraDice = new GearDice(1, 2, 0, 0, 2, DiceSlotEnum.DEFEND);
            extraDice.SetCombo(new TripleCombo());

            player.SetDice(attackDice, defendDice, extraDice);
        }

        //GearDice d1 = GearDiceDatabase.Instance.GetRandomizeGearInstance();
        //GearDice d2 = GearDiceDatabase.Instance.GetRandomizeGearInstance();
        //GearDice d3 = GearDiceDatabase.Instance.GetRandomizeGearInstance();
        //GearDice d4 = GearDiceDatabase.Instance.GetRandomizeGearInstance();
        //player.SetDice(d1, d2);

        List<GearDice> starters = GearDiceDatabase.Instance.GetStartingGearsInstance();
        player.SetDice(starters.ToArray());


        return player;
    }
}

public interface EntityWithHP
{
    public int HP { get; }
}
