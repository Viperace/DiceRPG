using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MaintainPosition : MonoBehaviour
{
    Vector3 origPosition;
    float origY;
    [SerializeField] float delay = 4f;
    [SerializeField] float offset = 0.1f;

    void Start()
    {
        origPosition = this.transform.localPosition;
    }

    float _countTime = 0;
    void Update()
    {
        if (Player.playerStat != null && Player.playerStat.HP > 0)
        {
            if( Vector3.Distance(this.transform.localPosition, origPosition) > offset)
                _countTime += Time.deltaTime;

            if (_countTime > delay)
            {
                this.transform.DOLocalMove(origPosition, 0.5f);
                _countTime = 0;
            }
        }
    }

}
