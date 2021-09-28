using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ShrineManager : MonoBehaviour
{
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] GameObject targetPanel;
    [SerializeField] GameObject godPrefabParent;
    [SerializeField] TMP_Text shrineTitle;

    GearDiceUIDisplay[] gearDiceUIs;
    GearDiceUIDisplay selectedDiceUI;
    GameParameter gameParameter;
    
    ShrineEffect loadedShrine;

    void Awake()
    {
        StartCoroutine(InitializeShrine());
    }
    void Start()
    {
        gearDiceUIs = inventoryPanel.GetComponentsInChildren<GearDiceUIDisplay>();
        selectedDiceUI = targetPanel.GetComponent<GearDiceUIDisplay>();

        gameParameter = ScriptableObject.CreateInstance<GameParameter>();

        StartCoroutine(InitializeButton());
    }

    IEnumerator InitializeShrine()
    {
        // Wait till player is ready
        while (Player.playerStat == null)
            yield return null;

        // Roll a random shirne to load
        List<ShrineEffect> allShrines = ShrineEffect.AllEffects;
        int roll = Random.Range(0, allShrines.Count);
        loadedShrine = allShrines[roll];

        // Initialize View
        for (int i = 0; i < godPrefabParent.transform.childCount; i++)
        {
            GameObject go = godPrefabParent.transform.GetChild(i).gameObject;
            if (go.name == loadedShrine.prefabName)
                go.SetActive(true);
            else
                go.SetActive(false);
        }

        shrineTitle.text = string.Concat("Shrine of ", loadedShrine.godName);
    }

    //Check for all UI, and disable the one that need not display
    IEnumerator InitializeButton()
    {
        while (gearDiceUIs[0].RepresentedGearDice == null && loadedShrine != null)
            yield return null;

        foreach (var item in gearDiceUIs)
        {
            if(item.RepresentedGearDice != null && item.RepresentedGearDice.combo != null)
            {
                if(item.RepresentedGearDice.combo.Equal(loadedShrine.combo)) // Already got Combo
                    item.GetComponent<Button>().interactable = false;
            }
        }
    }

    [SerializeField] Button prayButton;
    public void Pray()
    {        
        // Check target is available and valid
        if (Player.playerStat.Gold >= gameParameter.PrayingCost &
            selectedDiceUI.RepresentedGearDice != null)
        {
            // Take gold
            Player.playerStat.Gold -= gameParameter.PrayingCost;
            Debug.Log("Done praying!");

            // Give Power
            selectedDiceUI.RepresentedGearDice.SetCombo( loadedShrine.combo);

            // Play effect
            PlayPrayEffect();

            // Animate Button
            AnimateOut animateOut = prayButton.GetComponent<AnimateOut>();
            animateOut.DoFadeOut(() => prayButton.gameObject.SetActive(false));
        }
        else
        {
            AnimateOut animateOut = prayButton.GetComponent<AnimateOut>();
            animateOut.DoWeakFadeOut();
        }
    }


    [SerializeField] GameObject effectsGO;
    public void PlayPrayEffect()
    {
        ParticleSystem[] effectsToPlay = effectsGO.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var item in effectsToPlay)
        {
            item.gameObject.SetActive(true);
            item.Play();
        }
    }
}
