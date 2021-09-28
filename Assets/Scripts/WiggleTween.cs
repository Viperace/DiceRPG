using UnityEngine;
using DG.Tweening;

public class WiggleTween : MonoBehaviour
{
    [SerializeField] Vector3 startValue = new Vector3(0, 10, 0);
    [SerializeField] Vector3 endValue = new Vector3(0, -10, 0);
    [SerializeField] float duration = 2;
    [SerializeField] bool randomizeStartValue = true;
    public bool IsShowing { get; set; }
    Sequence sequence;

    void OnEnable()
    {
        IsShowing = true;

        // Keep x and z axis
        // Y axis is plus/minus original
        startValue.x = transform.localEulerAngles.x;
        startValue.z = transform.localEulerAngles.z;
        startValue.y += transform.localEulerAngles.y;

        endValue.x = transform.localEulerAngles.x;
        endValue.z = transform.localEulerAngles.z;
        endValue.y += transform.localEulerAngles.y;

        // Define original angle
        if (randomizeStartValue)
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 
                Random.Range(startValue.y, endValue.y),
                transform.localEulerAngles.z);
        Vector3 origAngle = transform.localEulerAngles;

        sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalRotate(startValue, 0.5f * duration));
        sequence.Append(transform.DOLocalRotate(endValue, duration));
        sequence.Append(transform.DOLocalRotate(origAngle, 0.25f * duration));
        sequence.SetLoops(-1);
    }

    private void OnDisable()
    {
        sequence.Kill();
    }
}
