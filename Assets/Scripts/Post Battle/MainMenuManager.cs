using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenuManager : MonoBehaviour
{
    [SerializeField] Button continueButton;
    void Start()
    {
        // Check if there is existing player journey
        if (Player.journeyLog == null || Player.journeyLog.Length == 0)
            continueButton.gameObject.SetActive(false);
        else
            continueButton.gameObject.SetActive(true);

    }

}
