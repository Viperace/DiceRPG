using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoosterChargeUI : MonoBehaviour
{
    MeshRenderer[] chargeImages;

    void Start()
    {
        chargeImages = GetComponentsInChildren<MeshRenderer>(true);
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
