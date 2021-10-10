using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace CubeMapGenerator
{
    /// <summary>
    /// TODO: 
    ///     Biome: Desert, Greens, Ice
    /// Place rain, snow
    /// </summary>
    public class GridMapGenerator : MonoBehaviour
    {
        public GameObject grassTile;
        public GameObject plainTile;
        public GameObject iceTile;
        public GameObject pillar;
        public GameObject trees;
        public Vector2 mapSize;
        public Vector2 tileSizeInWorldSpace;

        public float heightRange = 5f;
        public float perlinModifier = 0.111f;
        public bool IsTreeRotation = true;

        public Map map { get; private set; }
        Dictionary<Tile, GameObject> tilesGameObjectDict;  // Dict that map Tile with actual GameObject
        void Start()
        {

        }

        [Button("Generate Plains", ButtonSizes.Large)]
        public void GeneratePlains()
        {
            // Create map algoritmically
            map = new Map(mapSize, tileSizeInWorldSpace);
            map.GenerateEmptyTiles();

            float cx = Mathf.RoundToInt(Random.Range(0.2f, 0.8f) * map.mapSize.x);
            float cy = Mathf.RoundToInt(Random.Range(0.2f, 0.8f) * map.mapSize.y);
            Coordinate seed = new Coordinate(cx, cy);
            Map newMap = GrowRegionFromSeed(TerrainType.Plains, seed, this.map);
            map = newMap;

            // Put tree to cover at least 50%)
            GrowTreesToCoverPercent(0.5f);

            // Adjust elevation
            AdjustElevation();

            // Update View
            PopulateMapView();

        }


        [Button("Generate Random Map", ButtonSizes.Medium)]
        public void _GenerateRandomMap()
        {
            // Create map algoritmically
            map = new Map(mapSize, tileSizeInWorldSpace);
            map._GenerateRandom();

            // Create / Search holder, delete and create new 
            holder = GameObject.Find("GridMap_Holder");
            if (holder)
                DestroyImmediate(holder);  // Delete existing
            holder = new GameObject("GridMap_Holder");

            // Put it into the view
            foreach (var tile in map.Tiles)
            {
                GameObject tilePrefab;
                if (tile.Type == TerrainType.Grass)
                    tilePrefab = Instantiate(grassTile);
                else
                    tilePrefab = Instantiate(plainTile);

                tilePrefab.transform.position = map.GetWorldPosition(tile);
                tilePrefab.transform.SetParent(holder.transform);
            }

        }

        [SerializeField] GameObject holder;
        public void PopulateMapView()
        {
            // Create / Search holder, delete and create new 
            holder = GameObject.Find("GridMap_Holder");
            if (holder)
                DestroyImmediate(holder);  // Delete existing
            holder = new GameObject("GridMap_Holder");

            //--- Put Tile into the view
            tilesGameObjectDict = new Dictionary<Tile, GameObject>();
            foreach (var tile in map.Tiles)
            {
                GameObject tileGO = null;
                if (tile.Type == TerrainType.Grass)
                    tileGO = Instantiate(grassTile);
                else if (tile.Type == TerrainType.Plains)
                    tileGO = Instantiate(plainTile);
                else
                    Debug.LogError("Havent impelment tile type");

                tileGO.transform.position = map.GetWorldPosition(tile);
                tileGO.transform.SetParent(holder.transform);

                // Save
                tilesGameObjectDict.Add(tile, tileGO);
            }

            //-- Put trees
            foreach (var tile in map.Tiles)
            {
                if (tile.SoilFertility == 1) // Grow tree
                {
                    GameObject treesGO = Instantiate(trees);
                    treesGO.transform.SetParent(tilesGameObjectDict[tile].transform);
                    treesGO.transform.localPosition = Vector3.zero;

                    if (IsTreeRotation)
                        treesGO.transform.localEulerAngles = new Vector3(0, Random.Range(0, 360f), 0);
                }
            }

            //-- Set elevation
            foreach (var tile in map.Tiles)
            {
                tilesGameObjectDict[tile].transform.localPosition = new Vector3(tilesGameObjectDict[tile].transform.localPosition.x,
                    tile.Elevation,
                    tilesGameObjectDict[tile].transform.localPosition.z);
            }

            //-- Put Pillar
            foreach (var tile in map.Tiles)
            {
                GameObject tileGO = tilesGameObjectDict[tile];
                GameObject pillarGO = Instantiate(pillar);
                pillarGO.transform.SetParent(tileGO.transform);
                pillarGO.transform.localPosition = Vector3.zero;
            }
        }

        /// <summary>
        /// TODO: Move to algorithm class
        /// </summary>
        /// <param name="type"></param>
        /// <param name="seed"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        Map GrowRegionFromSeed(TerrainType type, Coordinate seed, Map map)
        {
            // Define frontrier
            List<Coordinate> frontier = new List<Coordinate>();
            frontier.Add(seed);
            foreach (var item in seed.GetNeighbors())
                frontier.Add(item);

            //
            Map newMap = new Map(map);
            newMap.SetTiles(type, frontier.ToArray());


            // Main Loop        
            int nLoop = 0;
            while (frontier.Count > 0)
            {
                // Find all potential candidates to grow
                // Loop each frontier and find its neighbor
                List<Coordinate> candidates = new List<Coordinate>();
                foreach (Coordinate v in frontier)
                {
                    List<Coordinate> validNeighbors = newMap.GetNeighborsOfDifferentType(v);
                    foreach (var item in validNeighbors)
                    {
                        if (!candidates.Contains(item))
                            candidates.Add(item);
                    }
                }

                // Go through each candiates and grow them
                // Grow probability = func( neighbor, tile)
                List<Coordinate> growCandidates = new List<Coordinate>();
                foreach (var x in candidates)
                {
                    // Get probability
                    List<Coordinate> sameTypeNeighbors = newMap.GetNeighborsOfSameType(x);
                    float p = ProbToGrow(sameTypeNeighbors.Count);

                    // Simulate growth
                    if (Random.value < p)
                        growCandidates.Add(x);
                }

                // Redefine frontier
                frontier = growCandidates;

                // Update the map Frontier
                newMap.SetTiles(type, frontier.ToArray());

                nLoop++;
            }

            return newMap;
        }

        public float[] growParams;
        float ProbToGrow(int nNeighbor)
        {
            //float[] prob = new float[3] { 0.5f, 0.6f, 0.9f };
            float[] prob = growParams;
            if (nNeighbor == 1)
                return prob[0];
            else if (nNeighbor == 2)
                return prob[1];
            else if (nNeighbor == 3)
                return prob[2];
            else
                return 0;
        }

        float GetGrowTreeScore(int nNeighborTrees, int nGrass, bool isOnGrass)
        {
            float grassBonus = isOnGrass ? 0.15f : 0;

            float neighborBonus;
            if (nNeighborTrees == 0)
                neighborBonus = 0.2f;
            else if (nNeighborTrees == 1)
                neighborBonus = 0.25f;
            else if (nNeighborTrees == 2)
                neighborBonus = 0.4f;
            else if (nNeighborTrees == 3)
                neighborBonus = 0.3f;
            else
                neighborBonus = 0.2f;

            return neighborBonus + grassBonus;
        }
        void GrowTrees(Coordinate seed)
        {
            List<Coordinate> frontier = new List<Coordinate>();
            frontier.Add(seed);

            // Set fertility. Param to determine tree grow
            map.GetTile(seed).SoilFertility = 1;
            foreach (var nn in map.GetNeighbors(seed))
            {
                map.GetTile(nn).SoilFertility = 1;
                frontier.Add(nn);
            }

            // Main loop
            while (frontier.Count > 0)
            {
                // Find all potential candidates to grow
                // Loop each frontier and find its neighbor
                List<Coordinate> candidates = new List<Coordinate>();
                foreach (Coordinate v in frontier)
                {
                    List<Coordinate> validNeighbors = map.GetNeighbors(v);
                    foreach (var item in validNeighbors)
                    {
                        if (!candidates.Contains(item))
                            candidates.Add(item);
                    }
                }

                // Go through each candiates and grow them
                // Grow probability = func( neighbor, tile)
                List<Coordinate> growCandidates = new List<Coordinate>();
                foreach (var x in candidates)
                {
                    // Get probability
                    List<Coordinate> myNeighbors = map.GetNeighbors(x);

                    int numOfNeighborWithHighSoil = 0;
                    foreach (var n in myNeighbors)
                    {
                        if (map.GetTile(n).SoilFertility == 1)
                            numOfNeighborWithHighSoil++;
                    }

                    // Tree Score
                    bool isOnGrass = map.GetTile(x).Type == TerrainType.Grass;
                    float p = GetGrowTreeScore(numOfNeighborWithHighSoil, 0, isOnGrass);
                    if (Random.value < p && map.GetTile(x).SoilFertility < 1)
                        growCandidates.Add(x);
                }

                // Redefine frontier
                frontier = growCandidates;

                // Update the map Frontier
                foreach (var coord in growCandidates)
                    map.GetTile(coord).SoilFertility = 1;

            }

        }


        /// <summary>
        /// Search large empty grass area and do 1 grow precedure
        /// </summary>
        void GrowTreesAtRandomGrassArea()
        {
            List<Coordinate> emptyCoords = map.SearchAreaForType(TerrainType.Grass, 5, 0.8f);

            // Randomy select 
            if (emptyCoords.Count > 0)
            {
                Coordinate seed = emptyCoords[Random.Range(0, emptyCoords.Count)];
                GrowTrees(seed);
            }
        }

        /// <summary>
        /// Keep calling GrowTreesAtRandomGrassArea() untill the minimum pct has reach
        /// </summary>
        /// <param name="pct"></param>
        void GrowTreesToCoverPercent(float minimumPct)
        {
            float totalArea = map.mapSize.x * map.mapSize.y;
            int treeCount = 0;
            int _safetyCount = 0;
            while (((float)treeCount) / totalArea < minimumPct)
            {
                // Check pct        
                foreach (var item in map.Tiles)
                {
                    if (item.SoilFertility == 1)
                        treeCount++;
                }

                GrowTreesAtRandomGrassArea();
                _safetyCount++;

                if (_safetyCount > 100)
                {
                    Debug.LogWarning("Loop unfinished [GrowTreesToCoverPercent]");
                    break;
                }

            }
        }

        /// <summary>
        /// Use Perlin noise to adj
        /// </summary>
        void AdjustElevation()
        {
            float xOffset = Random.Range(0, 1f);
            float yOffset = Random.Range(0, 1f);
            foreach (var tile in map.Tiles)
            {
                Coordinate coord = map.GetCoordinate(tile);
                coord *= perlinModifier;  // Since perlin noise always return same value at integer
                coord += new Coordinate(xOffset, yOffset);
                float height = Mathf.PerlinNoise(coord.x, coord.y);
                tile.Elevation = Mathf.RoundToInt(height * heightRange);
            }

        }

        #region Getters
        public GameObject GetGameObject(Coordinate coord)
        {
            Tile tile = map.GetTile(coord);
            if (tile != null)
                return tilesGameObjectDict[tile];
            else
                return null;
        }

        public GameObject FindNearestGameObjectAtWorldPos(Vector3 pos)
        {
            Coordinate closestCoord = map.FindNearestCoordinateFromWorldPosition(pos);
            return GetGameObject(closestCoord);
        }

        public GameObject FindNearestGameObjectAtCoord(Coordinate coord)
        {
            Coordinate closestCoord = map.FindNearestCoordinateFromCoord(coord);
            return GetGameObject(closestCoord);
        }

        public GameObject Holder { get { return holder; } }
        #endregion
    }

}