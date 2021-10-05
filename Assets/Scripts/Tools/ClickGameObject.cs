using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickGameObject : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                MapWayPoint poi = hit.transform.GetComponent<MapWayPoint>();
                if (poi)
                {
                    Debug.Log("Player click on " + poi.gameObject.name);
                }
            }
        }
    }
}
