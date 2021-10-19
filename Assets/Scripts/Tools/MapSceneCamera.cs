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
        cam.transform.position = new Vector3(this.transform.position.x, 
            this.transform.position.y,
            avatar.transform.position.z);
    }


    public float moveSpeed = 5f;
    public float boostDistance = 100f;
    bool _isFollowingMode = true;
    void LateUpdate()
    {
        // Lerp towards player pos
        if (_isFollowingMode && avatar)
        {
            float boost = (avatar.transform.position.z - this.transform.position.z > boostDistance) ? 10 : 1;

            float interpolation = boost * moveSpeed * Time.deltaTime;
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
            Input.GetKey(KeyCode.Tab) || IsTouched())
        {
            _isFollowingMode = true;
        }
        else
            _isFollowingMode = false;
        
        // 
    }

    bool IsTouched()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                //fingerUp = touch.position;
                //fingerDown = touch.position;
            }

            //Detects Swipe while finger is still moving
            if (touch.phase == TouchPhase.Moved)
            {
                return true;
            }

            //Detects swipe after finger is released
            if (touch.phase == TouchPhase.Ended)
            {
                //fingerDown = touch.position;
                //checkSwipe();
            }
        }

        return false;
    }
}
