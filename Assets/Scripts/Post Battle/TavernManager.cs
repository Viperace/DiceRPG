using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TavernManager : MonoBehaviour
{
    GameParameter gameParameter;
    InnLightManager lightManager;

    [SerializeField] TMP_Text restCostText;
    void Start()
    {
        gameParameter = ScriptableObject.CreateInstance<GameParameter>();
        lightManager = FindObjectOfType<InnLightManager>();

        restCostText.text = gameParameter.InnRestingCost.ToString();
    }

    public void RestAtInn()
    {
        if (Player.playerStat != null)
        {
            if (Player.playerStat.Gold >= gameParameter.InnRestingCost & !IsPlayerStaminaFull())
            {
                Player.playerStat.Gold -= gameParameter.InnRestingCost;
                RestoreEnergyFully();

                // Play animation
                lightManager.LightsOffAnimation();
            }
        }
    }

    bool IsPlayerStaminaFull()
    {
        return Player.playerStat.stamina >= Player.playerStat.maxStamina;
    }

    public void RestoreEnergy(int x)
    {
        if (Player.playerStat != null)
        {
            Player.playerStat.stamina += x;
            Player.playerStat.stamina = Mathf.Clamp(Player.playerStat.stamina, 0, Player.playerStat.maxStamina);
        }
    }

    public void RestoreEnergyFully()
    {
        if (Player.playerStat != null)
            Player.playerStat.stamina = Player.playerStat.maxStamina;
    }
}
