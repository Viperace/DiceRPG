using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssignSoundToAllButtons : MonoBehaviour
{
    [SerializeField] AudioClip buttonClickClip;
    
    Button[] buttons;
    void Start()
    {
        buttons = FindObjectsOfType<Button>();

        if (SoundManager.Instance) 
        {
            foreach (var btn in buttons)
                btn.onClick.AddListener(() => SoundManager.Instance.Play(buttonClickClip));
        }
    }

}
