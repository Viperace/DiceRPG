using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOn3dItemUI : MonoBehaviour
{
    GearTextNumber gearTextNumber;
    [SerializeField] GameObject threeDobject;
    void Start()
    {
        gearTextNumber = GetComponent<GearTextNumber>();

        StartCoroutine(ShowShieldOrSwordOrBoth(1f));
    }

    IEnumerator ShowShieldOrSwordOrBoth(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (gearTextNumber && gearTextNumber.LinkedGearDice != null)
        {
            TurnOnGameObject(gearTextNumber.LinkedGearDice.prefabName);
        }
    }

    void TurnOnGameObject(string name)
    {
        MeshRenderer[] childItems = threeDobject.GetComponentsInChildren<MeshRenderer>(true);

        foreach (var item in childItems)
        {
            if (item.name == name)
                item.gameObject.SetActive(true);
            else
                item.gameObject.SetActive(false);
        }
    }
}
