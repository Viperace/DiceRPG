using UnityEngine;
using DG.Tweening;
public class TweenLight : MonoBehaviour
{
    [SerializeField] float onValue = 1;
    [SerializeField] float offValue = 0;
    //[SerializeField] float duration;    

    public void TurnOff(float duration)
    {
        Light light = GetComponent<Light>();
        light.DOIntensity(offValue, duration);
    }

}
