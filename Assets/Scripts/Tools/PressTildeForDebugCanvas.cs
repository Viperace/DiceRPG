using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PressTildeForDebugCanvas : MonoBehaviour
{
    public GameObject goToTurnOn;
    public TMP_InputField commandLineInput;
    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Tilde) | Input.GetKeyUp(KeyCode.BackQuote))
        {
            if (goToTurnOn != null)
            {
                goToTurnOn.SetActive(!goToTurnOn.activeInHierarchy);                
            }

            if(commandLineInput != null)
                commandLineInput.gameObject.SetActive(!commandLineInput.gameObject.activeInHierarchy);
        }
    }
}
