using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoosterChargeUI : MonoBehaviour
{
    Image[] chargeImages;

    void Start()
    {
        chargeImages = GetComponentsInChildren<Image>(true);
    }

    void Update()
    {
        if(GameManager.Instance && GameManager.Instance.GetDiceBooster != null)
        {
            SetChargeNumber(GameManager.Instance.GetDiceBooster.chargeLeft);
        }
    }

    void SetChargeNumber(int n)
    {
        for (int i = 0; i < chargeImages.Length; i++)
        {
            if (i < n)
                chargeImages[i].gameObject.SetActive(true);
            else
                chargeImages[i].gameObject.SetActive(false);
        }
    }
}
