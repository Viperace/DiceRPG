using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class RouteMonobehavior : MonoBehaviour
{
    SplineComputer spline;
    List<GameObject> pointGOs;

    float distanceBetweenTwoGO ;
    float heightOffsetFromTile;

    void Awake()
    {
        spline = GetComponent<SplineComputer>();
    }

    public void Init(Vector3[] waypoints, float distanceBetweenTwoGO, float heightOffsetFromTile)
    {
        StartCoroutine(InitLate( waypoints,  distanceBetweenTwoGO,  heightOffsetFromTile));
    }
    IEnumerator InitLate(Vector3[] waypoints, float distanceBetweenTwoGO, float heightOffsetFromTile)
    {
        yield return null;

        // Set 
        if (spline == null)
            spline = GetComponent<SplineComputer>();
        this.distanceBetweenTwoGO = distanceBetweenTwoGO;
        this.heightOffsetFromTile = heightOffsetFromTile;

        //Define points
        SplinePoint[] points = new SplinePoint[waypoints.Length];
        for (int i = 0; i < waypoints.Length; i++)
        {
            points[i] = new SplinePoint();
            points[i].position = waypoints[i];
            points[i].normal = Vector3.up;
        }
        spline.SetPoints(points);
    }

    // TODO:
    // Waypoint use diff prefab
    // prefab should be on top of tiles
    public void CreateRouteOnScene(GameObject pathPrefab, GameObject waypointPrefab, Map map)
    {
        StartCoroutine(CreateRouteOnSceneCoroutine( pathPrefab, waypointPrefab,  map));
    }
    IEnumerator CreateRouteOnSceneCoroutine(GameObject pathPrefab, GameObject waypointPrefab, Map map)
    {
        yield return null;
        yield return null;

        float frac = distanceBetweenTwoGO / this.RouteLength;

        int numberOfPoints = Mathf.RoundToInt(1 / frac);

        // Move and instatiate dot
        for (int i = 0; i < numberOfPoints; i++)
        {
            GameObject go = Instantiate(pathPrefab);
            Vector3 surfacePos = spline.EvaluatePosition(i * frac); // Set X,Z
            float height = map.GetElevationAtWorldPosition(surfacePos);

            go.transform.position = new Vector3(surfacePos.x, height + heightOffsetFromTile, surfacePos.z);  // Set height

            go.transform.SetParent(this.transform);
        }
        yield return new WaitForSeconds(0.1f);

        // Move and instatiate wp
        for (int i = 0; i < spline.pointCount; i++)
        {
            SplinePoint p = spline.GetPoint(i);
            GameObject go = Instantiate(waypointPrefab);
            float height = map.GetElevationAtWorldPosition(p.position);
            go.transform.position = new Vector3(p.position.x, height + heightOffsetFromTile, p.position.z);  // Set height

            go.transform.SetParent(this.transform);
            go.transform.localScale *= 2;
        }

    }

    public float RouteLength
    {
        get { return spline.CalculateLength(); }    
    }
}
