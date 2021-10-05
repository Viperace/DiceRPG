using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _TestMapGeneration : MonoBehaviour
{
    void Start()
    {
        
    }

    public void _RegenMap()
    {
        // Clear old stuff
        GameObject go = GameObject.Find("Route_Holder");
        if (go) Destroy(go);

        GameObject poi = GameObject.Find("PointOfInterest");
        if (poi) Destroy(poi);

        // Gen New
        FindObjectOfType<JourneySceneLoader>().GenerateMap();
    }
}
