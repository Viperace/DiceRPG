using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MaintainRotation : MonoBehaviour
{
    Vector3 origRotation;
    float origY;
    [SerializeField] float delay = 4f;
    [SerializeField] float offset = 0.5f;

    void Start()
    {
        origRotation = this.transform.localEulerAngles;
        origY = Clamp0360(origRotation.y);
    }

    float _countTime = 0;
    void Update()
    {
        if(Player.playerStat != null && Player.playerStat.HP > 0) 
        { 
            if( Mathf.Abs(Clamp0360(this.transform.localEulerAngles.y) - origY) > offset)
                _countTime += Time.deltaTime;

            if(_countTime > delay)
            {
                this.transform.DOLocalRotate(origRotation, 0.5f); 
                _countTime = 0;
            }
        }
    }

    float Clamp0360(float eulerAngles)
    {
        float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;
        if (result < 0)
        {
            result += 360f;
        }
        return result;
    }
}
