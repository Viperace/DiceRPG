using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachMaskToChild : MonoBehaviour
{
    void Awake()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            if( transform.GetChild(i).gameObject.GetComponent<MaskObject>() == null)
                transform.GetChild(i).gameObject.AddComponent<MaskObject>();
        }
    }

}
