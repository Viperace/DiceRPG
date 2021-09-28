using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GearDiceUIDisplay : MonoBehaviour
{
    public GearDice RepresentedGearDice { get; private set; }

    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text statText;
    [SerializeField] TMP_Text durabilityText;
    [SerializeField] Transform attackOrDefendIcon;
    [SerializeField] Transform starHolder;
    [SerializeField] Image specialIcon;

    // Settings
    [SerializeField] Sprite starOnSprite;
    [SerializeField] Sprite starOffSprite;
    [SerializeField] Color starOnColor;
    [SerializeField] Color starOffColor;
    [SerializeField] ComboIconData comboIconData;

    Image[] attackDefendIcons;
    Image[] stars;

    MeshRenderer[] allMeshes;

    void Start()
    {
        StartCoroutine(_TestLinkPlayerDices(0));

        attackDefendIcons = attackOrDefendIcon.GetComponentsInChildren<Image>(true);
        stars = starHolder.GetComponentsInChildren<Image>(true);

        Transform prefabHolder = transform.Find("3dHolder");
        allMeshes = prefabHolder.GetComponentsInChildren<MeshRenderer>(true);
    }

    public void SetGearDice(GearDice d)
    {
        RepresentedGearDice = d;
    }

    public void SetGearDice(int playerDiceOrder)
    {
        if (playerDiceOrder >= 0 & playerDiceOrder < Player.playerStat.gearDices.Count)
            RepresentedGearDice = Player.playerStat.gearDices[playerDiceOrder];
        else
            RepresentedGearDice = null;
    }

    public void ClearGearDice()
    {
        RepresentedGearDice = null;

        //=== Close UI
        // Stars
        foreach (var s in stars)
            s.gameObject.SetActive(false);

        // Texts
        nameText.text = "* Select Item First*";
        statText.text = "-";
        durabilityText.text = "-";

        // Prefabs
        foreach (var item in allMeshes)
            item.gameObject.SetActive(false);

        // AD icon
        attackDefendIcons[0].gameObject.SetActive(false);
        attackDefendIcons[1].gameObject.SetActive(false);

    }

    //If current Represented Dice = null, set 1st Player dice, else
    //  Set the Represented Dice as next player dice. 
    public void SetNextPlayerGearDice()
    {
        if (RepresentedGearDice == null)
        {
            RepresentedGearDice = Player.playerStat.gearDices[0];
            return;
        }

        int n = Player.playerStat.gearDices.Count;
        for (int i = 0; i < n - 1; i++)
        {
            if(RepresentedGearDice == Player.playerStat.gearDices[i])
            {
                RepresentedGearDice = Player.playerStat.gearDices[i + 1];
                return;
            }
        }

        RepresentedGearDice = Player.playerStat.gearDices[0]; // Take the first
    }

    IEnumerator _TestLinkPlayerDices(int num)
    {
        while (RepresentedGearDice == null)
        {
            if(Player.playerStat != null)
                RepresentedGearDice = Player.playerStat.gearDices[num];
            yield return null;
        }
    }

    void PopulateStars()
    {
        // Set everything off first
        foreach (var s in stars)
            s.gameObject.SetActive(false);

        for (int i = 0; i < RepresentedGearDice.maxNumberOfStar; i++)
        {
            stars[i].gameObject.SetActive(true);
            if (i < RepresentedGearDice.numberOfStar)
            {
                stars[i].sprite = starOnSprite;
                stars[i].color = starOnColor;
            }
            else 
            { 
                stars[i].sprite = starOffSprite;
                stars[i].color = starOffColor;
            }
        }
    }

    void PopulateTexts()
    {
        nameText.text = RepresentedGearDice.gearName;
        statText.text = string.Concat(
            RepresentedGearDice.minValue + RepresentedGearDice.upgradeOnMinValue, 
            " - ", RepresentedGearDice.maxValue + RepresentedGearDice.upgradeOnMaxValue);
        durabilityText.text = string.Concat(RepresentedGearDice.durability, " / ", RepresentedGearDice.maxDurability);
    }

    void PopulatePrefab()
    {
        foreach (var item in allMeshes)
        {
            if (RepresentedGearDice.prefabName == item.gameObject.name)
                item.gameObject.SetActive(true);
            else
                item.gameObject.SetActive(false);
        }
    }

    void PopulateAttackDefendIcon()
    {
        // Turn off all first
        attackDefendIcons[0].gameObject.SetActive(false);
        attackDefendIcons[1].gameObject.SetActive(false);

        // Turn on
        foreach (var item in RepresentedGearDice.compatibleSlots)
        {
            switch (item)
            {
                case DiceSlotEnum.ATTACK:
                    attackDefendIcons[0].gameObject.SetActive(true);
                    break;
                case DiceSlotEnum.DEFEND:
                    attackDefendIcons[1].gameObject.SetActive(true);
                    break;
                default:
                    break;
            }
        } 
    }
    void PopulateComboIcon()
    {
        if (comboIconData == null)
            return;

        specialIcon.transform.parent.gameObject.SetActive(true);
        specialIcon.gameObject.SetActive(true);

        if (RepresentedGearDice.combo is DoubleCombo)
            specialIcon.sprite = comboIconData.doubleIcon;
        else if (RepresentedGearDice.combo is TripleCombo)
            specialIcon.sprite = comboIconData.tripleIcon;
        else if (RepresentedGearDice.combo is Straight)
            specialIcon.sprite = comboIconData.straightIcon;
        else if (RepresentedGearDice.combo is FourOfAKindCombo)
            specialIcon.sprite = comboIconData.fourAKindIcon;
        else if (RepresentedGearDice.combo is AllSameCombo)
            specialIcon.sprite = comboIconData.allSameIcon;
        else if (RepresentedGearDice.combo is SumEqual)
            specialIcon.sprite = comboIconData.sumEqualIcon;
        else
            specialIcon.transform.parent.gameObject.SetActive(false);
    }

    void Update()
    {
        if(RepresentedGearDice != null)
        {
            PopulateStars();

            PopulateTexts();

            PopulateAttackDefendIcon();

            PopulatePrefab();

            PopulateComboIcon();
        }
    }
}
