using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightLinkedDiceWhenMouseOver : MonoBehaviour
{
    Camera raycastCamera;
    GearDiceBehavior gearDiceBehavior;
    HighlightPlus.HighlightEffect targetToHighlight;

    void Start()
    {
        raycastCamera = GameObject.Find("Camera_UI").GetComponent<Camera>();

        StartCoroutine(LinkDiceCoroutine());
    }

    GearTextNumber linkedDice;
    IEnumerator LinkDiceCoroutine()
    {
        while (gearDiceBehavior == null)
        {
            gearDiceBehavior = GetComponent<GearDiceBehavior>();
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(0.9f);
        while (gearDiceBehavior.LinkedArsenalDice == null)
            yield return null;

        linkedDice = gearDiceBehavior.LinkedArsenalDice;
        targetToHighlight = linkedDice.GetComponent<HighlightPlus.HighlightEffect>();

    }

    void Update()
    {
        if (raycastCamera == null)
            return;

        if (targetToHighlight)
        {
            Ray ray;
            ray = raycastCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.transform.gameObject == this.gameObject)
                {
                    targetToHighlight.SetHighlighted(true);
                    return;
                }
            }
            
            // Turn off
            targetToHighlight.SetHighlighted(false);
        }
    }

}
