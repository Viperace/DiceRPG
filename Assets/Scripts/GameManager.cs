using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpecialComboHandler))]
public class GameManager : MonoBehaviour
{

    static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    MonsterAnimationController enemyAnimationController;
    PlayerAnimationController playerAnimationController;
    SpecialComboHandler specialComboHandler;
    ButtonSelector buttonSelector;
    Player player;

    // Round information
    GamePhase gamePhase = GamePhase.INIT;
    int roundNumber;
    public EnemyStat currentEnemy { get; private set; }

    GameCheat gameCheat;

    // Debug
    public Text _debugText;

    void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    void Start()
    {
        buttonSelector = FindObjectOfType<ButtonSelector>();
        enemyAnimationController = FindObjectOfType<MonsterAnimationController>();
        playerAnimationController = FindObjectOfType<PlayerAnimationController>();
        specialComboHandler = GetComponent<SpecialComboHandler>();
        gameCheat = FindObjectOfType<GameCheat>(true);

        player = Player.Instance;

        StartCoroutine(SetupFight(0.5f));
    }

    
    IEnumerator SetupFight(float delay)
    {
        // Wait till player
        while (Player.journeyLog == null)
            yield return null;

        // Init
        roundNumber = 1;

        // Load/Create enemy
        //currentEnemy = EnemyStat.CreateTestEnemy();
        
        // Enemy loading depends on current journey
        MonsterEnum m = JourneyAlgorithm.RollEnemy(Player.journeyLog);
        currentEnemy = EnemyStat.CreateNewEnemy(m);
        FindObjectOfType<MonsterAvatarController>().SelectAvatar(m);

        // Refresh player 
        Player.playerStat.HP = Player.playerStat.maxHP;

        // Load Hearts View
        HeartTrackerView[] views = FindObjectsOfType<HeartTrackerView>();
        foreach (var item in views)
            item.ResetTarget();

        if(enemyAnimationController)
            enemyAnimationController.SetupCharacter();

        // Setup special combo. Find all dices, and look for their combo
        StartCoroutine(LinkSpecialCombo());

        if (_debugText)
        {
            _debugText.text = "G0 compatible slot" + currentEnemy.gearDices[0].compatibleSlots.Count + " ," ;
            _debugText.text += "G1 compatible slot" + currentEnemy.gearDices[1].compatibleSlots.Count + " ,";
            _debugText.text += "G0 slot is" + currentEnemy.gearDices[0].compatibleSlots[0].ToString();
            _debugText.text += "G1 slot is" + currentEnemy.gearDices[1].compatibleSlots[0].ToString();
        }
    }

    IEnumerator LinkSpecialCombo()
    {
        GearDiceBehavior[] gearDiceBehaviors = FindObjectsOfType<GearDiceBehavior>();
        foreach (var item in gearDiceBehaviors)
        {
            while (item.RepresentedDice == null) // Wait till linked
                yield return null;
            
            specialComboHandler.AddCombo(item.RepresentedDice.combo);
        }
    }

    public void RunNextRound()
    {
        roundNumber++;
        gamePhase = GamePhase.INIT;

        // Unparent dice
        HolderSlot[] slots = FindObjectsOfType<HolderSlot>();
        foreach (var item in slots)
            item.EjectObject();

        StartCoroutine(RunNextRoundWithDelay(0f));

        // Stamina-
        Player.playerStat.stamina--;
    }

    IEnumerator RunNextRoundWithDelay( float delay)
    {
        yield return new WaitForSeconds(delay);

        RollDiceViewTransform[] dices = FindObjectsOfType<RollDiceViewTransform>();
        for (int i = 0; i < dices.Length; i++)
        {
            if(i == 0)
                dices[i].AnimateReset(() => RollPlayerDices()); //Only first one need to callback
            else
                dices[i].AnimateReset();
        }

        // Reset FX
        ParticleFXcontroller[] fxControllers = FindObjectsOfType<ParticleFXcontroller>();
        foreach (var c in fxControllers)
            c.StopAllEffects();
    }

    public void _RollPlayerDice()
    {
        RollPlayerDices();
    }

    public void _RollEnemyDice()
    {
        RollEnemyDiceAndResolve();
    }

    Dictionary<GearDiceBehavior, int> playerDiceOutcome;    
    public void RollPlayerDices()
    {
        //gamePhase = GamePhase.PLAYER_ROLL;

        // Roll actual number
        playerDiceOutcome = new Dictionary<GearDiceBehavior, int>();

        
        // Animate it
        GearDiceBehavior[] gearDiceBehaviors = FindObjectsOfType<GearDiceBehavior>();
        int icount = 0; // Number of represented dice counted
        foreach (GearDiceBehavior gearDiceBehavior in gearDiceBehaviors)
        {            
            if (gearDiceBehavior.RepresentedDice != null)
            {                
                int rollValue = gearDiceBehavior.RepresentedDice.Roll();

                // CHECK FOR GAME CHEAT OVERRIDE
                if (gameCheat.IsEnable)
                    rollValue = gameCheat.GetNextUserSpecifiedDiceNumber();

                playerDiceOutcome.Add(gearDiceBehavior, rollValue);

                if (icount == gearDiceBehaviors.Length - 1) // If last dice, attach the specialComboHandler callback
                {
                    // Check for Specials (straight, double, triple... )
                    //System.Action handleSpecialCombo = () => specialComboHandler.Handle(new int[3] { 3, 4, 5 });
                    System.Action handleSpecialCombo = () => specialComboHandler.Handle(playerDiceOutcome.Values.ToArray());
                    gearDiceBehavior.GetComponent<RollDiceViewTransform>().AnimateFreeRoll(rollValue, () =>
                    {
                        gamePhase = GamePhase.PLAYER_MOVE;
                        handleSpecialCombo();
                    });
                }
                else
                {
                    gearDiceBehavior.GetComponent<RollDiceViewTransform>().
                        AnimateFreeRoll(rollValue, () => gamePhase = GamePhase.PLAYER_MOVE);
                }

                icount++;
            }
        }

        // Play Immediate Effect
        StartCoroutine(ApplyImmediateComboEffect());

        // Next sequence, show enemy button
        buttonSelector.ToggleEnemyButton(true, 5f);
    }

    IEnumerator ApplyImmediateComboEffect()
    {
        // Wait so that it play together with teh special effect
        yield return new WaitForSeconds(specialComboHandler.SpecialEffectPlaytime + 1f);

        List<DiceSpecialCombo> combos = specialComboHandler.GetTriggeredCombos(playerDiceOutcome.Values.ToArray());
        foreach (var combo in combos)
        {
            Dictionary<DiceSlotEnum, int> temp = new Dictionary<DiceSlotEnum, int>();
            DiceComboEffect effect = new DiceComboEffect(combo, Player.playerStat, currentEnemy, ref temp);
            if (effect.IsEffectImmediate()) // Immediate End turn type
                effect.ApplyEffect();
        }
    }

    void ApplyEndTurnComboEffect(Dictionary<DiceSlotEnum, int> playerArrangedOutcome)
    {
        List<DiceSpecialCombo> combos = specialComboHandler.GetTriggeredCombos(playerDiceOutcome.Values.ToArray());
        foreach (var combo in combos)
        {
            DiceComboEffect effect = new DiceComboEffect(combo, Player.playerStat, currentEnemy, ref playerArrangedOutcome);
            if (!effect.IsEffectImmediate()) // End turn type
                effect.ApplyEffect();
        }
    }


    public void ShowEndBattleButton()
    {
        buttonSelector.HideAllButtons(0);
        buttonSelector.ShowEndBattleButton(0.5f);
    }

    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject lostPanel;
    public void EndBattleSequence()
    {
        if (Player.playerStat.HP > 0)
        {
            winPanel.SetActive(true);
            Player.journeyLog.AddNumerOfKill();
        }
        else
        {
            lostPanel.SetActive(true);
        }
            
    }

    /// <summary>
    /// Function to find player's dice and totalled the Attack and Defend point based on the slot they are slotted.
    /// </summary>
    Dictionary<DiceSlotEnum, int> TallyPlayerSlottedDice()
    {
        Dictionary<DiceSlotEnum, int> playerArrangedOutcome = new Dictionary<DiceSlotEnum, int>();
        playerArrangedOutcome.Add(DiceSlotEnum.ATTACK, 0);
        playerArrangedOutcome.Add(DiceSlotEnum.DEFEND, 0);

        foreach (var diceBehavior in playerDiceOutcome.Keys)
        {
            SlotType slot = diceBehavior.GetComponentInParent<SlotType>();
            int rollValue = playerDiceOutcome[diceBehavior];
            if (slot)
            {
                playerArrangedOutcome[slot.slotType] += rollValue;
            }
        }

        // Overlay with Combo
        //List<DiceSpecialCombo> combos =  specialComboHandler. GetTriggeredCombos(playerDiceOutcome.Values.ToArray());        
        //foreach (var combo in combos)
        //{
        //    DiceComboEffect effect = new DiceComboEffect(combo, Player.playerStat, currentEnemy, ref playerArrangedOutcome);
        //    if(!effect.IsEffectImmediate()) // End turn type
        //        effect.ApplyEffect();
        //}
        ApplyEndTurnComboEffect(playerArrangedOutcome);

        return playerArrangedOutcome;
    }

    /// <summary>
    /// For dices that are being slotted, lower their durability by 1
    /// For 0 durability, lock the dice
    /// </summary>
    void LowerPlayerDiceDurability()
    {
        foreach (var diceBehavior in playerDiceOutcome.Keys)
        {
            SlotType slot = diceBehavior.GetComponentInParent<SlotType>();
            if (slot)
            {
                diceBehavior.RepresentedDice.durability--;
                if (diceBehavior.RepresentedDice.durability < 0)
                    diceBehavior.RepresentedDice.durability = 0;
            }
        }

    }

    Dictionary<DiceSlotEnum, int> enemyDiceOutcome;
    void RollEnemyDiceAndResolve()
    {
        gamePhase = GamePhase.ENEMY_ROLL_AND_RESOLVE;

        // Tally player's 
        Dictionary<DiceSlotEnum, int> playerArrangedOutcome = TallyPlayerSlottedDice();

        // Enemy 
        GameObject[] gearDiceBehaviors = GameObject.FindGameObjectsWithTag("EnemyDice");

        // Roll actual number
        enemyDiceOutcome = new Dictionary<DiceSlotEnum, int>();
        for (int i = 0; i < currentEnemy.gearDices.Count; i++)
        {
            GearDice d = currentEnemy.gearDices[i];

            // Roll
            int rollValue = d.Roll();

            // Save
            enemyDiceOutcome.Add(d.compatibleSlots[0], rollValue);

            // Animate it
            foreach (var g in gearDiceBehaviors)
            {
                if(g.gameObject.name == "Enemy_Attack_Dice" & d.compatibleSlots[0] == DiceSlotEnum.ATTACK)
                    g.GetComponent<RollDiceViewTransform>().AnimateTargetPositionRoll(rollValue);
                else if(g.gameObject.name == "Enemy_Defend_Dice" & d.compatibleSlots[0] == DiceSlotEnum.DEFEND)
                    g.GetComponent<RollDiceViewTransform>().AnimateTargetPositionRoll(rollValue);
            }
        }

        if (_debugText)
        {
            for (int i = 0; i < currentEnemy.gearDices.Count; i++)
            {
                _debugText.text = "Attack is " + enemyDiceOutcome[DiceSlotEnum.ATTACK];
                _debugText.text += ". Def is " + enemyDiceOutcome[DiceSlotEnum.DEFEND];
                _debugText.text += "\n";
            }
            
        }
            

        //***** Play resolution ******
        float attackAnimDelay = 4f;
        StartCoroutine(PlayAttackAnim(attackAnimDelay, playerArrangedOutcome));

        // Play reset
        buttonSelector.ToggleNextRoundButton(true, attackAnimDelay + 2f);

        // Set durability lower
        LowerPlayerDiceDurability();
    }

    IEnumerator PlayAttackAnim(float delay, Dictionary<DiceSlotEnum, int> playerArrangedOutcome)
    {
        // Find 
        GameObject enemyAttackDice = GameObject.Find("Enemy_Attack_Dice");
        GameObject enemyDefendDice = GameObject.Find("Enemy_Defend_Dice");

        SlotType[] allSlots = GameObject.Find("PlayerSlots").GetComponentsInChildren<SlotType>();
        List<Transform> attackSlots = new List<Transform>();
        List<Transform> defendSlots = new List<Transform>();
        foreach (var s in allSlots)
        {
            if (s.slotType == DiceSlotEnum.ATTACK)
                attackSlots.Add(s.transform);
            else if (s.slotType == DiceSlotEnum.DEFEND)
                defendSlots.Add(s.transform);
        }

        yield return new WaitForSeconds(delay);

        // ********* Attack 1 - Enemy attack player ***********
        // Count first
        int enemyAttackRoll = enemyDiceOutcome[DiceSlotEnum.ATTACK];
        int playerDefendRoll = playerArrangedOutcome[DiceSlotEnum.DEFEND];
        int playerHPLost = DiceCombatResolution.FindDefenderHPLost(enemyAttackRoll, playerDefendRoll);
        System.Action playerCallback = () => { Player.playerStat.HP -= playerHPLost; };

        string textToShow = "Player HP Lost " + playerHPLost + ". Tally enemy attack/Player Defend:" + enemyAttackRoll + "/" + playerDefendRoll;
        Debug.Log(textToShow);
        if(_debugText) _debugText.text = textToShow;

        // Do animation of enemy Attack Dice attack player's defend dice 
        RollDiceViewTransform enemyAtk = enemyAttackDice.GetComponent<RollDiceViewTransform>();
        RollDiceViewTransform playerDef = defendSlots[0].GetComponentInChildren<RollDiceViewTransform>();
        if(playerDef)
            enemyAtk.AnimateAttackOtherDice(playerDef.gameObject, playerCallback);
        else
            enemyAtk.AnimateAttackOtherDice(defendSlots[0].gameObject, playerCallback);

        // Avatar attack anim
        if (enemyAnimationController) 
            enemyAnimationController.PlayAttackOnTarget(playerAnimationController);

        yield return new WaitForSeconds(2f);

        // ********* Attack 2 - Player attack enemy ***********
        if (Player.playerStat.HP > 0)
        {
            // Count first
            int playerAttackRoll = playerArrangedOutcome[DiceSlotEnum.ATTACK];
            int enemyDefendRoll = enemyDiceOutcome[DiceSlotEnum.DEFEND];
            int enemyHPLost = DiceCombatResolution.FindDefenderHPLost(playerAttackRoll, enemyDefendRoll);
            System.Action enemyCallback = () => { currentEnemy.HP -= enemyHPLost; };

            //textToShow = "Enemy HP Lost " + enemyHPLost + ". Tally Player attack/enemy Defend:" + playerAttackRoll + "/" + enemyDefendRoll;
            textToShow += "Enemy HP Lost " + enemyHPLost + ". Tally Player attack/enemy Defend:" + playerAttackRoll + "/" + enemyDefendRoll;
            Debug.Log(textToShow);
            if (_debugText) _debugText.text = textToShow;

            GroupAttackAnimation(attackSlots, enemyDefendDice, enemyCallback);

            if (playerAnimationController)
                playerAnimationController.PlayAttackOnTarget(enemyAnimationController);
        }
    }

    /// <summary>
    /// Multiple attacker animate attacking teh target. WIth the callback only called once
    /// </summary>
    /// <param name="attackSlots"></param>
    /// <param name="target"></param>
    /// <param name="firstCallback"></param>  Only be applied to first available attackSlot
    void GroupAttackAnimation(List<Transform> attackSlots, GameObject target, System.Action firstCallback)
    {
        //foreach (var a in attackSlots)
        bool _callbackUsed = false;
        for (int i = 0; i < attackSlots.Count; i++)
        {
            Transform a = attackSlots[i];
            RollDiceViewTransform attacker = a.GetComponentInChildren<RollDiceViewTransform>();
            if (attacker) {
                if (_callbackUsed)
                    attacker.AnimateAttackOtherDice(target, null);
                else
                {
                    attacker.AnimateAttackOtherDice(target, firstCallback);
                    _callbackUsed = true;
                }
            }
        }
    }


    bool _hasDoDeadSequence = false;
    void Update()
    {
        if (!_hasDoDeadSequence)
        {
            if (currentEnemy != null && currentEnemy.HP <= 0) // enemy die
            {
                enemyAnimationController.PlayDead();
                playerAnimationController.PlayVictory();
                
                ShowEndBattleButton();
                _hasDoDeadSequence = true;
            }
            else if ( Player.playerStat != null && Player.playerStat.HP <= 0)  // Player die
            {
                playerAnimationController.PlayDead();
                enemyAnimationController.PlayTaunt();

                ShowEndBattleButton();
                _hasDoDeadSequence = true;
            }
            else if (Player.playerStat != null && Player.playerStat.stamina <= 0) // Player stamina finish
            {
                playerAnimationController.PlayDead();

                ShowEndBattleButton();
                _hasDoDeadSequence = true;
            }
        }
    }

    #region Getters
    public int RoundNumber { get { return roundNumber; } }
    #endregion  
}

public enum GamePhase
{
    INIT = 0,
    PLAYER_MOVE = 1,
    ENEMY_ROLL_AND_RESOLVE = 2,
}


public class DiceCombatResolution
{
    int attacker;
    int defender;

    public DiceCombatResolution() {}
    public DiceCombatResolution(int attacker, int defender)
    {
        this.attacker = attacker;
        this.defender = defender;
    }

    public int DefenderHPlost()
    {
        if (attacker > defender)
            return 1;
        else
            return 0;
    }

    public static int FindDefenderHPLost(int attacker, int defender)
    {
        if (attacker > defender)
            return 1;
        else
            return 0;
    }
}