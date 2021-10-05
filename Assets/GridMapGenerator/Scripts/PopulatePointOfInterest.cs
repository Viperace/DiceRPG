using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace CubeMapGenerator
{

    /// <summary>
    /// Algoirthm to put location onto map
    /// </summary>
    public class PopulatePointOfInterest : MonoBehaviour
    {
        public GameObject[] allPOIprefabs;
        public float coverPercentage = 0.01f;

        GridMapGenerator mapGenerator;

        Map map;

        GameObject gameObjectHolder;

        public PopulatePointOfInterest() { }
        void Start()
        {

            Init();
        }

        void Init()
        {
            mapGenerator = FindObjectOfType<GridMapGenerator>();
            map = mapGenerator.map;

            // Create / Search holder, create new if not exists
            gameObjectHolder = GameObject.Find("PointOfInterest");
            if (!gameObjectHolder)
                gameObjectHolder = new GameObject("PointOfInterest");
        }

        [Button("Spawn POI Randomly", ButtonSizes.Large)]
        public void _SpawnRandomPOI()
        {
            Init();

            // Find the coords
            List<Coordinate> coordinates = SuggestRandomPoints();

            foreach (var coord in coordinates)
            {
                // Random select POI 
                int roll = Random.Range(0, allPOIprefabs.Length);

                // Spawn POI
                SpawnPrefabAt(coord, allPOIprefabs[roll]);

            }
        }

        GameObject SpawnPrefabAt(Coordinate coord, GameObject prefab)
        {
            GameObject poiGO = Instantiate(prefab);

            poiGO.transform.position = map.GetWorldPosition(coord);
            float height = map.GetElevationAt(coord);
            poiGO.transform.position += new Vector3(0, height, 0);
            poiGO.transform.SetParent(gameObjectHolder.transform);

            return poiGO;
        }

        public void SpawnPOIbyGraph(GraphWithDepthNode graph)
        {
            // Decide location to place
            //Dictionary<Node, Coordinate> nodeCoordsDict = ComputePositionToPlace(graph);
            List<Coordinate> nodeCoords = ComputePositionToPlace(graph);

            // Place Nodes (POI) first
            StartCoroutine( PlacePrefabs(graph, nodeCoords));
        }

        IEnumerator PlacePrefabs(GraphWithDepthNode graph, List<Coordinate> allCoords)
        {
            int maxDepth = graph.GetMaxDepth();

            /**************** Place points ****************
            */
            for (int idepth = 0; idepth < maxDepth + 1; idepth++)
            {
                List<DepthNode> nodesInThisDepth = graph.GetNodesAtDepth(idepth);
                foreach (var node in nodesInThisDepth)
                {
                    Coordinate coord = allCoords[0];
                    GameObject go = SpawnPrefabAt(coord, node.prefab);
                    node.AssignGameObject(go);

                    // Pop
                    allCoords.RemoveAt(0);

                    if (allCoords.Count == 0)
                        break;
                }
            }

            yield return null;
            //yield return new WaitForSeconds(0.5f);

            //---  Place Routes
            RouteGenerator routeGenerator = FindObjectOfType<RouteGenerator>();

            // Loop each connector and place
            graph.MapConnectors();
            foreach (var con in graph.connectors)
            {
                Vector3 start = con.node1.gameObject.transform.position;
                Vector3 end = con.node2.gameObject.transform.position;
                routeGenerator.GenerateRoute(start, end);
            }

        }

        //Dictionary<Node, Coordinate> ComputePositionToPlace(GraphWithDepthNode graph)
        List<Coordinate> ComputePositionToPlace(GraphWithDepthNode graph)
        {
            Init();

            //************ Place POI based on depth *************
            // Slowly move by y-axis ?
            int maxDepth = graph.GetMaxDepth();
            int numberOfDepth = graph.GetNumberOfDepths();

            CoordinateBound mapBound = map.GetBound();

            // Generate coord first
            /* We breakdown map in axis-Y based on depth level, orderly.
             * For each dept level, we spawn the corresponding nodes only
             * So, node at lower depth must have lower Y value than nodes with higher depth
             * 
             * We decide number of area = Ylength / depth level. Then each area populate with nodes of the same depths
             * 
             * For each area, we generate points that are at far enough apart
             */
            float mapLength = mapBound.upper.y - mapBound.lower.y;
            float yLengthPerDepthLevel = mapLength / ((float)maxDepth);
            Coordinate _startingLower = map.GetBound().lower;
            Coordinate _startingUpper = new Coordinate(map.GetBound().upper.x, _startingLower.y + yLengthPerDepthLevel);
            CoordinateBound smallArea = new CoordinateBound(_startingLower, _startingUpper);

            List<Coordinate> allCoords = new List<Coordinate>();
            Dictionary<Node, Coordinate> coordNodeDict = new Dictionary<Node, Coordinate>();
            for (int i = 0; i < numberOfDepth; i++)
            {
                Node[] nodes = graph.GetNodesAtDepth(i).ToArray();
                for (int j = 0; j < nodes.Length; j++)
                {
                    int _counter = 0;
                    while (true)
                    {
                        // Generate random point within this area, but must not repeat ! If repeat, keep looping till can find
                        int nx = Random.Range((int)smallArea.lower.x, (int)smallArea.upper.x + 1);
                        int ny = Random.Range((int)smallArea.lower.y, (int)smallArea.upper.y + 1);
                        Coordinate point = new Coordinate(nx, ny);

                        if (!allCoords.Contains(point) || _counter > 200)
                        {
                            allCoords.Add(point);
                            coordNodeDict.Add(nodes[j], point);
                            break;
                        }
                        _counter++;
                    }
                }

                // Shift bound
                smallArea.ShiftYBy(yLengthPerDepthLevel);

                // Clamp
                smallArea.ClampTo(mapBound);
            }

            if (allCoords.Count < graph.NumberOfNodes)
                Debug.LogError("not enough coord");

            // Filter for the distance
            // 1st pass
            allCoords = CoordinateMath.MinimumDistanceFilter(allCoords, 3);

            // 2nd pass
            allCoords = CoordinateMath.MinimumDistanceFilter(allCoords, 6);

            // Clamp all coordinate bound
            for (int i = 0; i < allCoords.Count; i++)
            {
                if (!mapBound.IsWithinBound(allCoords[i])) // Set it to bound if not
                    allCoords[i] = mapBound.ClampCoordinateWithinBound(allCoords[i]);
            }

            // FIX ME
            // Can I just do like this?
            

            return allCoords;
        }

        List<Coordinate> SuggestRandomPoints()
        {
            // Parameter

            // Init
            int Ny = Mathf.RoundToInt(map.mapSize.y);
            int Nx = Mathf.RoundToInt(map.mapSize.x);
            int totalPOI = Mathf.RoundToInt(map.Tiles.Count * coverPercentage);

            // buffer size. 
            int bufferSize = 15; // Every this size, geernate 
            int totalPOIperArea = Mathf.RoundToInt(Nx * bufferSize * coverPercentage);
            int nBuffer = Mathf.CeilToInt(Ny / ((float)bufferSize));

            // Init
            List<Coordinate> coords = new List<Coordinate>();

            // Go thorugh each buffer 
            CoordinateBound mapBound = map.GetBound();
            CoordinateBound bufferBound = new CoordinateBound(
                new Coordinate(mapBound.lower.x, mapBound.lower.y),
                new Coordinate(mapBound.upper.x, bufferSize));
            for (int i = 0; i < nBuffer; i++)
            {
                // Spawn POI
                for (int j = 0; j < totalPOIperArea; j++)
                {
                    Coordinate rollCoord = SuggestLocation(PointOfInterestLocationType.Random, bufferBound);
                    coords.Add(rollCoord);
                }

                // Define next area
                bufferBound.ShiftYBy(bufferSize);
            }

            return coords;
        }

        public Coordinate SuggestLocation(PointOfInterestLocationType type, CoordinateBound searchBound)
        {
            int x = (int)Random.Range(searchBound.lower.x, searchBound.upper.x);
            int y = (int)Random.Range(searchBound.lower.y, searchBound.upper.y);
            return new Coordinate(x, y);
        }

        public Coordinate GetHighestElevation(CoordinateBound searchBound, CoordinateBound exclusionBound = null)
        {
            Coordinate bestCoord = Coordinate.zero;
            float bestElevation = -Mathf.Infinity;
            for (float i = searchBound.lower.x; i < searchBound.upper.x; i++)
                for (float j = searchBound.upper.x; j < searchBound.upper.x; j++)
                {
                    if (exclusionBound != null && exclusionBound.IsWithinBound(new Coordinate(i, j))) // Dont consider if its excluded
                        continue;

                    float h = map.GetElevationAt(new Coordinate(i, j));
                    if (h > bestElevation)
                    {
                        bestElevation = h;
                        bestCoord = new Coordinate(i, j);
                    }
                }

            return bestCoord;
        }


       
        [Button("_Gen Graph", ButtonSizes.Medium)]
        public void _GenerateRandomGraph()
        {
            Init();

            // All prefabs that will be used
            GameObject[] prefabs = new GameObject[3] { allPOIprefabs[0], allPOIprefabs[1], allPOIprefabs[2] };

            //************ Do Graph *************
            GraphWithDepthNode graph = new GraphWithDepthNode();

            // Create first node
            DepthNode node0 = new DepthNode(0, allPOIprefabs[0]);
            graph.Add(node0);


            // Spawn tree
            Node pastNode = node0;
            for (int i = 1; i < 10; i++)
            {
                int n = Random.Range(1, 2 + 1);
                //int n = 2;
                List<Node> tempNodes = new List<Node>();
                for (int j = 0; j < n; j++)
                {
                    DepthNode node = new DepthNode(i, prefabs[j]);
                    node.ConnectTo(pastNode);  // Connect past node
                    graph.Add(node);
                    tempNodes.Add(node);
                }

                // Revise pastNode
                pastNode = tempNodes[Random.Range(0, tempNodes.Count)];
            }

            //************ Place POI based on depth *************
            // Slowly move by y-axis ?
            int maxDepth = graph.GetMaxDepth();
            int numberOfDepth = graph.GetNumberOfDepths();

            CoordinateBound mapBound = map.GetBound();

            // Generate coord first
            /* We breakdown map in axis-Y based on depth level, orderly.
             * For each dept level, we spawn the corresponding nodes only
             * So, node at lower depth must have lower Y value than nodes with higher depth
             * 
             * We decide number of area = Ylength / depth level. Then each area populate with nodes of the same depths
             * 
             * For each area, we generate points that are at far enough apart
             */
            float mapLength = mapBound.upper.y - mapBound.lower.y;
            float yLengthPerDepthLevel = mapLength /((float) maxDepth);
            Coordinate _startingLower = map.GetBound().lower;
            Coordinate _startingUpper = new Coordinate(map.GetBound().upper.x, _startingLower.y + yLengthPerDepthLevel);
            CoordinateBound smallArea = new CoordinateBound(_startingLower, _startingUpper);

            List<Coordinate> allCoords = new List<Coordinate>();
            for (int i = 0; i < numberOfDepth; i++)
            {
                Node[] nodes = graph.GetNodesAtDepth(i).ToArray();
                for (int j = 0; j < nodes.Length; j++)
                {
                    int _counter = 0;
                    while (true)
                    {
                        // Generate random point within this area, but must not repeat ! If repeat, keep looping till can find
                        int nx = Random.Range((int)smallArea.lower.x, (int)smallArea.upper.x + 1);
                        int ny = Random.Range((int)smallArea.lower.y, (int)smallArea.upper.y + 1);
                        Coordinate point = new Coordinate(nx, ny);

                        if (!allCoords.Contains(point) || _counter > 200)
                        {
                            allCoords.Add(point);
                            break;
                        }
                        _counter++;
                    }
                }

                // Shift bound
                smallArea.ShiftYBy(yLengthPerDepthLevel);

                // Clamp
                smallArea.ClampTo(mapBound);
            }

            if (allCoords.Count < graph.NumberOfNodes)
                Debug.LogError("not enough coord");

            // Filter for the distance
            // 1st pass
            allCoords = CoordinateMath.MinimumDistanceFilter(allCoords, 3);

            // 2nd pass
            allCoords = CoordinateMath.MinimumDistanceFilter(allCoords, 6);

            // Clamp all coordinate bound
            for (int i = 0; i < allCoords.Count; i++)           
            {
                if (!mapBound.IsWithinBound(allCoords[i])) // Set it to bound if not
                    allCoords[i] = mapBound.ClampCoordinateWithinBound(allCoords[i]);
            }

            /**************** Place points ****************
             */
            for (int idepth = 0; idepth < maxDepth + 1; idepth++)
            {
                List<DepthNode> nodesInThisDepth = graph.GetNodesAtDepth(idepth);
                foreach (var node in nodesInThisDepth)                
                {
                    Coordinate coord = allCoords[0];
                    GameObject go = SpawnPrefabAt(coord, node.prefab);
                    node.AssignGameObject(go);

                    // Pop
                    allCoords.RemoveAt(0);

                    if (allCoords.Count == 0)
                        break;
                }
            }
            //Debug.Log("Totalnode is  "  + totalNode);

            //---  Place Routes
            RouteGenerator routeGenerator = FindObjectOfType<RouteGenerator>();

            // Loop each connector and place
            graph.MapConnectors();
            foreach (var con in graph.connectors)
            {
                Vector3 start = con.node1.gameObject.transform.position;
                Vector3 end = con.node2.gameObject.transform.position;
                routeGenerator.GenerateRoute(start, end);
            }

        }

        public enum PointOfInterestLocationType
        {
            Random = 0,
            Peak,
            HillSide,
            HillBottom,
            DeepForest,
            ForestSide
        }

    }
}