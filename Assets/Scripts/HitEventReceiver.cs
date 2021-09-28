using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEventReceiver : MonoBehaviour
{

    System.Action hitCallback;

    public void SetHitCallback(System.Action callback) => hitCallback = callback;

    public void HitEvent()
    {
        if (hitCallback != null)
            hitCallback();
        //Debug.Log("Monster Hit ");
    }
}
