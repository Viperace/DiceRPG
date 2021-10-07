using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSceneCamera : MonoBehaviour
{
    [SerializeField] PlayerAvatar avatar;
    [SerializeField] bool fixX = true;
    Camera cam;
    
    float camHeight;
    public Vector3 offsetFromAvatar; 
    
    void Awake()
    {
        cam = this.GetComponent<Camera>();
        camHeight = cam.transform.position.y;
    }

    void Start()
    {
        
    }

    public float moveSpeed = 5f;
    bool _isFollowingMode = true;
    void LateUpdate()
    {
        // Lerp towards player pos
        if (_isFollowingMode && avatar)
        {
            float interpolation = moveSpeed * Time.deltaTime;
            Vector3 newPosition = cam.transform.position;
            newPosition.z = Mathf.Lerp(cam.transform.position.z, avatar.transform.position.z + offsetFromAvatar.z, interpolation);

            if (!fixX)
                newPosition.x = Mathf.Lerp(cam.transform.position.x, avatar.transform.position.x + offsetFromAvatar.x, interpolation);

            cam.transform.position = newPosition;
        }
        else
        {
            Vector3 avatarFront = avatar.transform.position + new Vector3(0, 0, -30);

            // Focus on front
            float interpolation = moveSpeed * Time.deltaTime;
            Vector3 newPosition = cam.transform.position;
            newPosition.z = Mathf.Lerp(cam.transform.position.z, avatarFront.z + offsetFromAvatar.z, interpolation);

            if(!fixX)
                newPosition.x = Mathf.Lerp(cam.transform.position.x, avatarFront.x + offsetFromAvatar.x, interpolation);

            cam.transform.position = newPosition;
        }

        // Toggle
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.Tab))
        {
            _isFollowingMode = true;
        }
        else
            _isFollowingMode = false;
        
        // 
    }
}
