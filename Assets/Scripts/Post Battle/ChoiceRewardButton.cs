using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class ChoiceRewardButton : MonoBehaviour
{
    [SerializeField] GameObject gearHolder;
    [SerializeField] GameObject coinGO;
    [SerializeField] GameObject staminaGO;
    [SerializeField] GameObject hpGO;
    [SerializeField] TMP_Text descriptionText;

    public ChoiceReward choiceReward;
    public bool IsChosen { get; private set; }
    Button button;
    RewardManager rewardManager;

    void Awake()
    {
        ClearUI();

        button = this.GetComponent<Button>();
        rewardManager = FindObjectOfType<RewardManager>();
    }

    void ClearUI()
    {
        gearHolder.gameObject.SetActive(false);
        coinGO.gameObject.SetActive(false);
        staminaGO.gameObject.SetActive(false);
        hpGO.gameObject.SetActive(false);
        descriptionText.text = "";
    }

    void OnEnable()
    {
        StartCoroutine(AttachListener());
        IsChosen = false;
    }

    void OnDisable() => ClearUI();

    // Listener to load UI when reward is assigned.
    IEnumerator AttachListener()
    {
        // Wait
        while (choiceReward == null)
            yield return null;

        // Assign
        LoadUI();

        // Attach button
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            this.IsChosen = true;
            this.AnimateEnlarge();
            choiceReward.ApplyReward(Player.playerStat);
            rewardManager.CloseUnchosedButtons();
            rewardManager.ExitScene();
        });
    }

    public void SetReward(ChoiceReward choiceReward) => this.choiceReward = choiceReward;

    public void AnimateClose()
    {
        button.interactable = false;
        this.transform.DOScale(0, 1f);
    }

    public void AnimateEnlarge()
    {
        button.interactable = false;
        float currentScale = this.transform.localScale.y;
        this.transform.DOScale(currentScale*2, 0.5f).SetEase(Ease.InBounce);
    }

    /// Turn on relevant UI element based on the reward
    void LoadUI()
    {
        descriptionText.text = choiceReward.RewardText;

        if(choiceReward is GoldReward)
        {
            coinGO.gameObject.SetActive(true);
        }
        else if(choiceReward is MaxStaminaReward)
        {
            staminaGO.gameObject.SetActive(true);
        }
        else if(choiceReward is MaxHPreward)
        {
            hpGO.gameObject.SetActive(true);
        }
        else if(choiceReward is RandomGearReward)
        {
            gearHolder.gameObject.SetActive(true);

            RandomGearReward gear = (RandomGearReward)choiceReward;
            for (int i = 0; i < gearHolder.transform.childCount; i++)
            {
                if (gearHolder.transform.GetChild(i).name == gear.selectedGear.prefabName)
                    gearHolder.transform.GetChild(i).gameObject.SetActive(true);
                else
                    gearHolder.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        else
            Debug.LogError("Dont have such choice reward: " + choiceReward.ToString());
    }

}
