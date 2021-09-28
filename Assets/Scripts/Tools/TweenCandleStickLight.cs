using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TweenCandleStickLight : MonoBehaviour
{
    
    IEnumerator TurnOffChildLight(float delay)
    {
        Light light = GetComponentInChildren<Light>();

        if (light)
        {
            yield return new WaitForSeconds(delay);
            light.intensity = 0;
        }
    }

    IEnumerator TurnOffParticleSystem(float delay)
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        if (ps)
        {
            yield return new WaitForSeconds(delay);
            var main = ps.main;
            main.loop = false;

            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }


    public void TurnOffInXseconds(float X)
    {
        StartCoroutine(TurnOffParticleSystem(X));
        StartCoroutine(TurnOffChildLight(X));
    }

    [Button("Turn Off", ButtonSizes.Large)]
    public void TurnOff()
    {
        TurnOffInXseconds(1.5f);
    }
}
