using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickControl : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    PlayerAvatar playerAvatar;
  
    void Start()
    {
        playerAvatar = FindObjectOfType<PlayerAvatar>();
    }


    float _cooldown = 0;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                MapWayPoint poi = hit.transform.GetComponent<MapWayPoint>();
                if (poi & _cooldown < 0)
                {
                    //reachAction = () => poi.ReachDestinationAction();
                    //playerAvatar.MoveToNode(poi.GetNode, reachAction);
                    playerAvatar.MoveToNode(poi.GetNode, poi);
                    Debug.Log("Player click on " + poi.gameObject.name);

                    _cooldown = 0.5f; // Prevent multiple clicks
                }
            }
        }
        _cooldown -= Time.deltaTime;
    }

}
