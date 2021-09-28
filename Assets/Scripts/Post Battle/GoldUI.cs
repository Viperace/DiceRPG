using UnityEngine;
using TMPro;

public class GoldUI : MonoBehaviour
{
    TMP_Text text;
    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (Player.playerStat != null)
            text.text = Player.playerStat.Gold.ToString();
        else
            text.text = "";
    }
}
