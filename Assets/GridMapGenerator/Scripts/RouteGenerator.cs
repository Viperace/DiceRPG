using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using Sirenix.OdinInspector;


namespace CubeMapGenerator
{
    [ExecuteAlways]
    public class RouteGenerator : MonoBehaviour
    {
        public int numberOfIntermediatePoints;
        public float averageDistancePerWaypoint; // In terms of coordinates
        public float varianceOfDistancePerWaypoint;

        [SerializeField]
        public Coordinate startPoint;

        [SerializeField]
        public Coordinate endPoint;

        [LabelText("Unit in World Space")]
        public RoutePerturbationStyle perturbStyle;
        public float perturbModifier;
        public float distanceBetweenTwoGO = 1f;
        public float heightOffsetFromTile = 0.1f;

        // Views
        public GameObject routeMonobehaviorPrefab;
        public GameObject pathPrefab;
        public GameObject waypointPrefab;
        public GameObject destinationPrefab;


        GridMapGenerator mapGenerator;
        Map map;

        GameObject gameObjectHolder;
        void Awake()
        {
            Init();
        }

        void Update()
        {

        }

        void Init()
        {
            mapGenerator = GetComponent<GridMapGenerator>();
            map = mapGenerator.map;

            // Create / Search holder, create new if not exists
            gameObjectHolder = GameObject.Find("Route_Holder");
            if (!gameObjectHolder)
                gameObjectHolder = new GameObject("Route_Holder");
        }

        [Button("Generate Route", ButtonSizes.Large)]
        public void GenerateRoute()
        {
            Init();

            // Generate coordinates
            List<Vector2> waypoints = GenerateWaypointWorldPosd();

            // Spawn gameObject based on coords;
            List<Vector3> waypoints3d = new List<Vector3>();
            foreach (var pos in waypoints)
            {
                GameObject tileGO = mapGenerator.FindNearestGameObjectAtWorldPos(new Vector3(pos.x, 0, pos.y));
                waypoints3d.Add(tileGO.transform.position);
            }


            // Initialize Route
            GameObject routeGO = Instantiate(routeMonobehaviorPrefab);
            RouteMonobehavior routeBehavior = routeGO.GetComponent<RouteMonobehavior>();
            routeBehavior.Init(waypoints3d.ToArray(), distanceBetweenTwoGO, heightOffsetFromTile);

            // Spwan Each rounte
            routeBehavior.CreateRouteOnScene(pathPrefab, waypointPrefab, map);
            routeBehavior.transform.SetParent(gameObjectHolder.transform);

        }

        public void GenerateRoute(Vector3 start, Vector3 end)
        {
            Init();

            // Redefine
            startPoint = map.FindNearestCoordinateFromWorldPosition(start);
            endPoint = map.FindNearestCoordinateFromWorldPosition(end);

            // Gen
            GenerateRoute();
        }

        /// <summary>
        /// Given startPoint and EndPOint, function to chop this into user-defined N pieces of way points.
        /// Output in terms of wolrd pos (but flatten height)
        /// </summary>
        /// <returns></returns>
        List<Vector2> GenerateWaypointWorldPosd()
        {
            // Gather data
            Vector3 xStart3 = map.GetWorldPosition(startPoint);
            Vector3 xEnd3 = map.GetWorldPosition(endPoint);

            Vector2 xStart = new Vector2(xStart3.x, xStart3.z);
            Vector2 xEnd = new Vector2(xEnd3.x, xEnd3.z);

            Vector2 dir = xEnd - xStart;

            
            List<Vector2> output = new List<Vector2>();
            output.Add(xStart);

            //** Generate intermediate points, (at coordinate level)
            // Distance per intermediate segment
            float totalDist = Vector2.Distance(xEnd, xStart);
            float pctDistPerSegment = 1 / ((float)numberOfIntermediatePoints + 1);            
            for (int i = 0; i < numberOfIntermediatePoints; i++)
            {
                float currPct = pctDistPerSegment * (i + 1);

                // Position of this i-th waypoint
                Vector2 waypointPos = Vector2.Lerp(xStart, xEnd, currPct);

                // Perturb it perpendicularly
                float perturbSize = CalculatePerturbSize(totalDist, currPct * totalDist);
                Vector2 dirPerp = CalculatePerturbDirection(dir, totalDist, currPct * totalDist);
                waypointPos += dirPerp * perturbSize;

                // Clamp it to bound
                waypointPos = ClampToMapBound(waypointPos);

                output.Add(waypointPos);
            }
            output.Add(xEnd);

            return output;
        }

        Vector2 ClampToMapBound(Vector2 pos)
        {
            // Get bound
            Vector2 upperPos = map.GetUpperWorldPos();
            Vector2 lowerPos = map.GetLowerWorldPos();

            return new Vector2(Mathf.Clamp(pos.x, lowerPos.x, upperPos.x), Mathf.Clamp(pos.y, lowerPos.y, upperPos.y));
        }

        // Based on the style, calc the relevant
        Vector2 CalculatePerturbDirection(Vector2 straightDir, float totalLength, float currentLength)
        {
            Vector3 perpendicular3 = Vector3.Cross(new Vector3(straightDir.x, 0, straightDir.y), Vector3.up);
            Vector2 dirPerp = new Vector2(perpendicular3.x, perpendicular3.z);
            dirPerp.Normalize();

            float eps = Random.value > 0.5f ? 1 : -1;
            switch (perturbStyle)
            {
                case RoutePerturbationStyle.LengthDependent: // 1/2 of total length                    
                    return eps  * dirPerp;
                case RoutePerturbationStyle.Random:
                    return eps * dirPerp;
                case RoutePerturbationStyle.NoPerturbation:
                    return dirPerp;
                case RoutePerturbationStyle.LeftArc:
                    return dirPerp;
                case RoutePerturbationStyle.RightArc:
                    return -dirPerp;
                default:
                    return dirPerp;
            }
        }

        float CalculatePerturbSize(float totalLength, float currentLength)
        {
            float eps;
            float pct = Mathf.Min(currentLength / totalLength, 1f - currentLength / totalLength);

            switch (perturbStyle)
            {
                case RoutePerturbationStyle.LengthDependent: // 1/2 of total length
                    eps = Random.value > 0.5f ? 1f : -1f;
                    return eps * Random.Range(0, 0.333f * totalLength * perturbModifier);
                case RoutePerturbationStyle.Random:
                    eps = Random.value > 0.5f ? 1f : -1f;
                    return eps * Random.Range(0, 1f) * perturbModifier;
                case RoutePerturbationStyle.NoPerturbation:
                    return 0;
                case RoutePerturbationStyle.LeftArc:
                    return pct * perturbModifier * totalLength;
                case RoutePerturbationStyle.RightArc:
                    return pct * perturbModifier * totalLength;
                default:
                    return 0;
            }
        }

        SplineComputer spline;
        /// <summary>
        /// Using SplineComputer to define waypoints as spline point 
        /// </summary>
        /// <returns></returns>
        SplineComputer CreateSplineFromWaypoints(List<Vector3> waypoints)
        {
            spline = gameObject.AddComponent<SplineComputer>();

            SplinePoint[] points = new SplinePoint[waypoints.Count];
            for (int i = 0; i < waypoints.Count; i++)
            {
                points[i] = new SplinePoint();
                points[i].position = waypoints[i];
                points[i].normal = Vector3.up;
            }
            spline.SetPoints(points);

            return spline;
        }
    }

    public enum RoutePerturbationStyle
    {
        LengthDependent, // The longer the distance, the more perturb
        Random,
        NoPerturbation, // Fixed
        LeftArc,        // Perturb to the left only
        RightArc,
    }
}