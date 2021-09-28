using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class TweenDirectionalLight : MonoBehaviour
{
    Vector3 noonRotationValue;
    Vector3 nightRotationValue;
    [SerializeField] Vector3 morningRotationValue;
    void Awake()
    {
        nightRotationValue = this.transform.localEulerAngles;
    }

    public void RotateToMorning(float duration, float delay)
    {
        Sequence seq = DOTween.Sequence();
        seq.PrependInterval(delay);
        seq.Append( this.transform.DOLocalRotate(morningRotationValue, duration));
    }

    public void RotateToNight(float duration)
    {
        this.transform.DOLocalRotate(nightRotationValue, duration);
    }

    [Button("Toggle Morning", ButtonSizes.Large)]
    public void ToggleMorning()
    {
        RotateToMorning(1, 0);
    }

    [Button("Toggle Night", ButtonSizes.Large)]
    public void ToggleNight()
    {
        RotateToNight(1);
    }
}
