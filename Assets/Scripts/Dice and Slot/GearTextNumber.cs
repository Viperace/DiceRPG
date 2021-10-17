using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GearTextNumber : MonoBehaviour
{
    public static GearTextNumber gearTextNumberStatic;

    TMP_Text text;
    public GearDice LinkedGearDice { get; set; }
    [SerializeField] bool isPlayer;
    [SerializeField] DiceSlotEnum slot;

    static int currentlyLinkedPlayerDices = 0;
    
    void Start()
    {
        text = GetComponentInChildren<TMP_Text>();

        if(this.gameObject.activeInHierarchy)
            LinkNextGearDice();
    }

    // Keep searching for GameManager till we can link the enemy
    IEnumerator LinkEnemyDiceRoutine()
    {
        LinkedGearDice = null;
        EnemyStat stat;
        while (LinkedGearDice == null)
        {
            stat = GameManager.Instance.currentEnemy;
            if(stat!=null)
                foreach (GearDice d in stat.gearDices)
                    if (d.compatibleSlots[0] == slot)
                        LinkedGearDice = d;
            yield return null;
        }
    }

    // Keep searching for GameManager till we can link 
    IEnumerator LinkPlayerDiceRoutine()
    {
        LinkedGearDice = null;
        while (LinkedGearDice == null)
        {
            PlayerStat playerStat = Player.playerStat;
            if (playerStat != null && currentlyLinkedPlayerDices < playerStat.gearDices.Count)
            {                
                LinkedGearDice = playerStat.gearDices[currentlyLinkedPlayerDices];
                currentlyLinkedPlayerDices++;
                Debug.Log(this.gameObject.name + " Link to " + LinkedGearDice.gearName);


            }
            yield return null;
        }        
    }

    // Search all dice and find the suitable one
    void LinkNextGearDice()
    {        
        if (isPlayer)
        {
            //PlayerStat playerStat = Player.playerStat;
            //if (currentlyLinkedPlayerDices < playerStat.gearDices.Count)
            //{
            //    this.LinkedGearDice = playerStat.gearDices[currentlyLinkedPlayerDices];
            //    currentlyLinkedPlayerDices++;
            //}
            StartCoroutine(LinkPlayerDiceRoutine());
        }
        else
        {
            StartCoroutine(LinkEnemyDiceRoutine());
        }
    }

    void Update()
    {
        if (LinkedGearDice != null)
        {
            if(LinkedGearDice.effectiveMinValue == LinkedGearDice.effectiveMaxValue)
                text.text = string.Concat(LinkedGearDice.effectiveMinValue);
            else
                text.text = string.Concat(LinkedGearDice.effectiveMinValue, " - ", LinkedGearDice.effectiveMaxValue);
        }
    }

    void OnDestroy()
    {
        currentlyLinkedPlayerDices = 0;
    }
}
