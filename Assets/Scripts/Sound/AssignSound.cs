using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssignSound : MonoBehaviour
{
    [SerializeField] AudioClip buttonClickClip;
    void Start()
    {
        Button btn = GetComponent<Button>();

        if (SoundManager.Instance)
            btn.onClick.AddListener(() => SoundManager.Instance.Play(buttonClickClip));
    }

}
