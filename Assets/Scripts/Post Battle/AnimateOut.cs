using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class AnimateOut : MonoBehaviour
{    
    public void DoFadeOut( System.Action action = null)
    {
        if(action != null)
            transform.DOShakeScale(0.3f).OnComplete( () => action());
        else
            transform.DOShakeScale(0.3f);
    }
    public void DoWeakFadeOut(System.Action action = null)
    {
        if (action != null)
            transform.DOShakeScale(0.3f, 0.3f).OnComplete(() => action());
        else
            transform.DOShakeScale(0.3f, 0.3f);
    }
}
