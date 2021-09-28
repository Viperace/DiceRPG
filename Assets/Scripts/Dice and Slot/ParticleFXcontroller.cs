using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFXcontroller : MonoBehaviour
{
    ParticleSystem[] particleSystems;
    void Start()
    {
        particleSystems = transform.Find("FX").GetComponentsInChildren<ParticleSystem>(true);
    }

    public void ToggleEffect(string name, bool value)
    {
        foreach (var fx in particleSystems)
            if(fx.name == name)
            {
                fx.gameObject.SetActive(value);
                return;
            }
    }

    public void PlayGlowYellow() => ToggleEffect("FX_GlowSpot_01", true);

    public void PlayGlowBlue() => ToggleEffect("FX_GlowSpot_02", true);

    public void PlayGlowGreen() => ToggleEffect("FX_GlowSpot_03", true);


    public void StopAllEffects()
    {
        foreach (var item in particleSystems)
            item.gameObject.SetActive(false);
    }
}
