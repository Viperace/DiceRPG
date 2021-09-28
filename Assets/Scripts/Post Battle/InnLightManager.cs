using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class InnLightManager : MonoBehaviour
{
    TweenCandleStickLight[] candles;
    TweenDirectionalLight[] directionalLights;
    TweenLight[] genericLights;
    void Start()
    {
        candles = FindObjectsOfType<TweenCandleStickLight>();
        directionalLights = FindObjectsOfType<TweenDirectionalLight>();
        genericLights = FindObjectsOfType<TweenLight>();
    }

    [Button("Lights Off", ButtonSizes.Gigantic)]
    public void LightsOffAnimation()
    {
        float candlesOffDuration = 0.8f;
        float allLightsOffDuration = 1f;
        float rotateToMorningDuration = 2f;
        

        foreach (var item in candles)
            item.TurnOffInXseconds(candlesOffDuration);
        
        foreach (var item in genericLights)
            item.TurnOff(allLightsOffDuration);

        foreach (var item in directionalLights)
            item.RotateToMorning(rotateToMorningDuration, allLightsOffDuration);
    }
}
