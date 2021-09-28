using UnityEngine;
using TMPro;

public class StaminaUI : MonoBehaviour
{
    TMP_Text text;
    [SerializeField] StaminaTextShowType staminaTextShowType = StaminaTextShowType.StaminaOnly;
    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (Player.playerStat != null)
        {
            if(staminaTextShowType == StaminaTextShowType.StaminaOnly)
                text.text = string.Concat(Player.playerStat.stamina);
            else if(staminaTextShowType == StaminaTextShowType.StaminaAndMaxStamina)
            {
                text.text = string.Concat(Player.playerStat.stamina, "/",
                Player.playerStat.maxStamina);
            }
            else if (staminaTextShowType == StaminaTextShowType.MaxStaminaOnly)
            {
                text.text = string.Concat("/", Player.playerStat.maxStamina);
            }
        }
    }

    enum StaminaTextShowType
    {
        StaminaOnly,
        MaxStaminaOnly,
        StaminaAndMaxStamina,
    }
}
