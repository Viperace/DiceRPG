using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Will only turn on relevant icon, if the player has the combo dice
/// </summary>
public class SpecialComboIconManager : MonoBehaviour
{
    [SerializeField] GameObject doubleIcon;
    [SerializeField] GameObject tripleIcon;
    [SerializeField] GameObject fourAKindIcon;
    [SerializeField] GameObject allSameIcon;
    [SerializeField] GameObject sumEqualIcon;
    [SerializeField] GameObject straightIcon;
    List<GameObject> allIcons;
    [SerializeField] ComboIconData iconData;

    void Start()
    {
        // Init icons
        //iconData = ScriptableObject.CreateInstance<ComboIconData>();
        UpdateIconImage();

        // Save as list 
        allIcons = new List<GameObject>();
        allIcons.Add(doubleIcon);
        allIcons.Add(tripleIcon);
        allIcons.Add(fourAKindIcon);
        allIcons.Add(allSameIcon);
        allIcons.Add(sumEqualIcon);
        allIcons.Add(straightIcon);

        // Turn Off All Icons
        foreach (var item in allIcons)
            item.SetActive(false);

        StartCoroutine(TurnOnRelevantIcon());
    }

    IEnumerator TurnOnRelevantIcon()
    {
        // Wait till player is ready
        while (Player.playerStat == null)
            yield return null;

        
        // TUrn on relevant icons
        foreach (var dice in Player.playerStat.gearDices)
        {
            if (dice.combo != null)
            {
                if (dice.combo is DoubleCombo)
                    doubleIcon.SetActive(true);
                else if (dice.combo is TripleCombo)
                    tripleIcon.SetActive(true);
                else if (dice.combo is FourOfAKindCombo)
                    fourAKindIcon.SetActive(true);
                else if (dice.combo is AllSameCombo)
                    allSameIcon.SetActive(true);
                else if (dice.combo is SumEqual)
                    sumEqualIcon.SetActive(true);
                else if (dice.combo is Straight)
                    straightIcon.SetActive(true);
            }
        }
    }

    void UpdateIconImage()
    {
        doubleIcon.transform.GetChild(0).GetComponent<Image>().sprite = iconData.doubleIcon;
        tripleIcon.transform.GetChild(0).GetComponent<Image>().sprite = iconData.tripleIcon;
        fourAKindIcon.transform.GetChild(0).GetComponent<Image>().sprite = iconData.fourAKindIcon;
        allSameIcon.transform.GetChild(0).GetComponent<Image>().sprite = iconData.allSameIcon;
        sumEqualIcon.transform.GetChild(0).GetComponent<Image>().sprite = iconData.sumEqualIcon;
        straightIcon.transform.GetChild(0).GetComponent<Image>().sprite = iconData.straightIcon;

    }
}
