using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickGameObject : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    PlayerAvatar playerAvatar;
    PlayerMapHolder mapHolder;

    System.Action reachAction;
    void Awake()
    {
        mapHolder = FindObjectOfType<PlayerMapHolder>();

        // Check scene
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

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
                if (poi)
                {
                    //reachAction = () => poi.ReachDestinationAction();
                    //playerAvatar.MoveToNode(poi.GetNode, reachAction);
                    playerAvatar.MoveToNode(poi.GetNode, poi);
                    Debug.Log("Player click on " + poi.gameObject.name);

                    _cooldown = 10f; // Prevent multiple clicks
                }
            }
        }
        _cooldown -= Time.deltaTime;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(!playerAvatar)
            playerAvatar = FindObjectOfType<PlayerAvatar>();
    }
}
