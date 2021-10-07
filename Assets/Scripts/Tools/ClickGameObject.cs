using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickGameObject : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    PlayerAvatar playerAvatar;
    void Start()
    {
        playerAvatar = FindObjectOfType<PlayerAvatar>();
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
                    playerAvatar.MoveToNode(poi.GetNode, () => poi.ReachDestinationAction());
                    Debug.Log("Player click on " + poi.gameObject.name);
                }
            }
        }
    }
}
