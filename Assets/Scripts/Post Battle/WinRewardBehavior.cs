using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WinRewardBehavior : MonoBehaviour
{
    [SerializeField] TMP_Text goldText;
    void Awake()
    {
        
    }
    void OnEnable()
    {
        if (GameManager.Instance.currentEnemy != null) 
        { 
            Reward reward = GameManager.Instance.currentEnemy.reward;
            Player.playerStat.Gold += reward.gold;

            goldText.text = reward.gold.ToString();
        }
    }

}


public class Reward
{
    public int gold;
    public int stamina;
    public int maxStamina;
    public int HP;
    public int maxHP;

}