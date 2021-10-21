using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenRotation : MonoBehaviour
{
    public float rotationSpeed = 1f; // Euler angle / s

    void Update()
    {
        this.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
