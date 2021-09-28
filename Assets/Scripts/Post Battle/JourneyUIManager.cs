using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JourneyUIManager : MonoBehaviour
{
    [SerializeField] Transform buttonsGroup;
    [SerializeField] Button blacksmithButton;
    [SerializeField] Button battleButton;
    [SerializeField] Button innButton;
    [SerializeField] Button shopButton;
    [SerializeField] Button shrineButton;
    [SerializeField] Button rewardButton;

    Button[] _buttons;
    void Start()
    {
        _buttons = buttonsGroup.GetComponentsInChildren<Button>(true);

        StartCoroutine(SetDestinationButtons());
    }

    IEnumerator SetDestinationButtons()
    {
        // Turn off all button first
        foreach (var item in _buttons)
            item.gameObject.SetActive(false);

        // Wait till Player is loaded
        while (Player.journeyLog == null)
            yield return null;
       
        // Attach button for each destination. Deactivate the extra buttons
        if (Player.journeyLog != null)
        {
            //List<JourneyEnum> destinations = Player.journeyLog.GenerateRandomNextDestinations();
            List<JourneyEnum> destinations = Player.journeyLog.GetNextDestinations();

            for (int i = 0; i < destinations.Count; i++)
            {
                switch (destinations[i])
                {
                    case JourneyEnum.CombatEncounter:
                        battleButton.gameObject.SetActive(true);
                        break;
                    case JourneyEnum.BossEncounter:
                        battleButton.gameObject.SetActive(true);
                        break;
                    case JourneyEnum.Tavern:
                        innButton.gameObject.SetActive(true);
                        break;
                    case JourneyEnum.Shop:
                        shopButton.gameObject.SetActive(true);
                        break;
                    case JourneyEnum.Blacksmith:
                        blacksmithButton.gameObject.SetActive(true);
                        break;
                    case JourneyEnum.Shrine:
                        shrineButton.gameObject.SetActive(true);
                        break;
                    case JourneyEnum.ChoiceReward:
                        rewardButton.gameObject.SetActive(true);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void AddFightJourneyRecord() => Player.journeyLog.AddUserSelectedJourney(JourneyEnum.CombatEncounter);
    public void AddBlacksmithJourneyRecord() => Player.journeyLog.AddUserSelectedJourney(JourneyEnum.Blacksmith);
    public void AddTavernJourneyRecord() => Player.journeyLog.AddUserSelectedJourney(JourneyEnum.Tavern);
    public void AddShopJourneyRecord() => Player.journeyLog.AddUserSelectedJourney(JourneyEnum.Shop);
    public void AddShrineJourneyRecord() => Player.journeyLog.AddUserSelectedJourney(JourneyEnum.Shrine);
    public void AddRewardJourneyRecord() => Player.journeyLog.AddUserSelectedJourney(JourneyEnum.ChoiceReward);


}
