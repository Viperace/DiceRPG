using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _TestMapGeneration : MonoBehaviour
{
    public Toggle waypointToggle;

    void Start()
    {
        waypointToggle.onValueChanged.AddListener(delegate {
            ToggleWaypointCanLoadScene();
        });
    }

    public void _RegenMap()
    {
        // Clear old stuff
        GameObject go = GameObject.Find("Route_Holder");
        if (go) Destroy(go);

        GameObject poi = GameObject.Find("PointOfInterest");
        if (poi) Destroy(poi);

        // Gen New
        FindObjectOfType<JourneySceneLoader>().GenerateNewMap();
    }

    void ToggleWaypointCanLoadScene()
    {
        MapWayPoint[] waypoints = FindObjectsOfType<MapWayPoint>();
        foreach (var wp in waypoints)
            wp.allowLoadScene = waypointToggle.isOn;

    }
}
