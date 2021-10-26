using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Background: Gear Dice has bug where it cannot be dragged when the CameraUI is there. But if it is turned off and on immediately,
/// the dice can then be dragged. Reason is still not being found
/// 
/// This patch will turn off and on the UICamera at 1sec mark after game start.
/// </summary>
public class _PatchCamera_UI : MonoBehaviour
{
    public Camera cameraUI;
    float countdown = 1f;


    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown < 0)
        {
            if (cameraUI.gameObject.activeInHierarchy)
            {
                cameraUI.gameObject.SetActive(false);
            }
            else
            {
                cameraUI.gameObject.SetActive(true);
                countdown = Mathf.Infinity;
            }
            
        }
        
    }

}
