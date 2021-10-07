using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

/// <summary>
/// A map tell us the total size, what is contain within
/// which coordinate contain what tile
/// </summary>
public class Map 
{
    public Vector2 mapSize;
    public Vector2 tileSizeInWorldSpace; 
    Dictionary<Coordinate, Tile> tileDict;
    Dictionary<Tile, Coordinate> coordDict;
    const int dx = 1;
    const int dy = 1;

    public Map() { }
    public Map(Vector2 size, Vector2 tileSizeInWorldSpace) 
    {
        this.mapSize = size;
        this.tileSizeInWorldSpace = tileSizeInWorldSpace;

        // Create dict
        GenerateEmptyTiles();
    }

    // Copy constructor
    public Map(Map map)
    {
        this.mapSize = map.mapSize;
        this.tileSizeInWorldSpace = map.tileSizeInWorldSpace;
        this.tileDict = new Dictionary<Coordinate, Tile>(map.tileDict);
        this.coordDict = new Dictionary<Tile, Coordinate>(map.coordDict);
    }

    public void Add(Tile tile, Coordinate coordinate)
    {
        if (tileDict.ContainsKey(coordinate)) 
        {
            // Remove old
            coordDict.Remove(tileDict[coordinate]);

            // Add new 
            coordDict.Add(tile, coordinate);

            // Replace
            tileDict[coordinate] = tile;
        }
        else
        {
            tileDict.Add(coordinate, tile);
            coordDict.Add(tile, coordinate);
        }
    }

    public void GenerateEmptyTiles(TerrainType type = TerrainType.Grass)
    {
        tileDict = new Dictionary<Coordinate, Tile>();
        coordDict = new Dictionary<Tile, Coordinate>();
        for (int i = 0; i < mapSize.x; i++)
            for (int j = 0; j < mapSize.y; j++)
            {
                Coordinate coord = new Coordinate(i, j);
                Tile tile = new Tile(TerrainType.Grass);
                Add(tile, coord) ;
            }
    }

    public void _GenerateRandom()
    {
        tileDict = new Dictionary<Coordinate, Tile>();
        for (int i = 0; i < mapSize.x; i++)
            for (int j = 0; j < mapSize.y; j++)
            {
                Coordinate coord = new Coordinate(i, j);
                Tile tile;
                if (Random.value > 0.6f)
                    tile = new Tile(TerrainType.Plains);
                else
                    tile = new Tile(TerrainType.Grass);
                this.Add(tile, coord);
            }
    }

    #region Getter
    public List<Tile> Tiles
    {
        get { return tileDict.Values.ToList(); }
    }
    public List<Coordinate> Coordinates
    {
        get { return tileDict.Keys.ToList(); }
    }

    public Tile GetTile(Coordinate coordinate)
    {
        if (tileDict.Keys.Contains(coordinate))
            return tileDict[coordinate];
        else
            return null;
    }

    public Coordinate GetCoordinate(Tile tile)
    {
        return coordDict[tile];
    }

    // Height included
    public Vector3 GetWorldPosition(Tile tile)
    {
        Coordinate coord = coordDict[tile];
        Vector3 pos = new Vector3(coord.x * tileSizeInWorldSpace.x, tile.Elevation,
            coord.y * tileSizeInWorldSpace.y);
        return pos;
    }


    public Vector3 GetWorldPosition(Coordinate coord)
    {
        Vector3 pos = new Vector3(coord.x * tileSizeInWorldSpace.x, 0,
            coord.y * tileSizeInWorldSpace.y);
        return pos;
    }

    public Vector3 GetWorldPositionInVector2(Coordinate coord)
    {
        Vector3 pos = GetWorldPosition(coord);
        return new Vector2(pos.x, pos.z);
    }

    public float GetElevationAt(Coordinate coord)
    {
        if (tileDict.Keys.Contains(coord))
            return tileDict[coord].Elevation;
        else
            return 0;
    }

    // Given (x, y, z), y will be ignord
    public float GetElevationAtWorldPosition(Vector3 pos)
    {
        Coordinate coord = FindNearestCoordinateFromWorldPosition(pos);
        return GetElevationAt(coord);
    }


    public List<Coordinate> GetNeighbors(Coordinate coord)
    {
        List<Coordinate> validNeighbors = new List<Coordinate>();
        foreach (Coordinate neighbor in coord.GetNeighbors())
        {
            if (tileDict.Keys.Contains(neighbor))
                validNeighbors.Add(neighbor);
        }
        return validNeighbors;
    }


    public List<Coordinate> GetNeighborsOfDifferentType(Coordinate coord)
    {
        List<Coordinate> neighbors = GetNeighbors(coord);
        List<Coordinate> output = new List<Coordinate>();
        foreach (Coordinate n in neighbors)
            if (tileDict[n].Type != tileDict[coord].Type)
                output.Add(n);
        return output;
    }

    public List<Coordinate> GetNeighborsOfSameType(Coordinate coord)
    {
        List<Coordinate> neighbors = GetNeighbors(coord);
        List<Coordinate> output = new List<Coordinate>();
        foreach (Coordinate n in neighbors)
            if (tileDict[n].Type == tileDict[coord].Type)
                output.Add(n);
        return output;
    }

    public CoordinateBound GetBound()
    {
        float minX = Mathf.Infinity;
        float maxX = -Mathf.Infinity;
        float minY = Mathf.Infinity;
        float maxY = -Mathf.Infinity;
        foreach (var coord in Coordinates)
        {
            if (coord.x < minX) 
                minX = coord.x;
            if (coord.x > maxX)
                maxX = coord.x;
            if (coord.y < minY)
                minY = coord.y;
            if (coord.y > maxY)
                maxY = coord.y;
        }

        CoordinateBound bound = new CoordinateBound(new Coordinate(minX, minY),
            new Coordinate(maxX, maxY));

        return bound;
    }

    public Vector2 GetUpperWorldPos()
    {
        CoordinateBound bound = GetBound();
        return this.GetWorldPositionInVector2(bound.upper);
    }

    public Vector2 GetLowerWorldPos()
    {
        CoordinateBound bound = GetBound();
        return this.GetWorldPositionInVector2(bound.lower);
    }
    #endregion

    /// <summary>
    /// Given a world position,  (ignoring the height of the pos), find the corresponding coordinate that is nearest
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Coordinate FindNearestCoordinateFromWorldPosition(Vector3 pos)
    {
        Coordinate targetCoord = new Coordinate(pos.x / tileSizeInWorldSpace.x, pos.z / tileSizeInWorldSpace.y);
        return FindNearestCoordinateFromCoord(targetCoord);
    }

    public Coordinate FindNearestCoordinateFromCoord(Coordinate targetCoord)
    {
        CoordinateBound bound = GetBound();
        int xLo = Mathf.FloorToInt(bound.lower.x) - 1;
        int xHi = Mathf.CeilToInt(bound.upper.x) + 1;
        int yLo = Mathf.FloorToInt(bound.lower.y) - 1;
        int yHi = Mathf.CeilToInt(bound.upper.y) + 1;

        // Search through all coord for this target (in x)
        int xFound = 0;
        for (int i = xLo; i < xHi; i++)
            if (targetCoord.x - i < 0.5f * dx)
            {
                xFound = i;
                break;
            }

        // Search in y
        int yFound = 0;
        for (int j = yLo; j < yHi; j++)
            if (targetCoord.y - j < 0.5f * dy)
            {
                yFound = j;
                break;
            }

        return new Coordinate(xFound, yFound);
    }


    public void SetTiles(TerrainType type, params Coordinate[] coords)
    {
        foreach (var c in coords)
            if(tileDict.Keys.Contains(c))
                tileDict[c].Type = type;
    }

    /// <summary>
    /// Return list of coord that has square area 'areaSize' , with coverage of at least 'minimumPercentage'% of type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="areaSize"></param>
    /// <param name="minimumPercentage"></param>
    /// <returns></returns>
    public List<Coordinate> SearchAreaForType(TerrainType type, int areaSize, float minimumPercentage)
    {
        List<Coordinate> allCandidates = new List<Coordinate>();
        foreach (var coord in coordDict.Values)
        {
            float pct = _GetCoveragePercentageOfType(coord, type, areaSize);
            if (pct > minimumPercentage)
            {
                allCandidates.Add(coord);
            }
        }
        return allCandidates;
    }

    float _GetCoveragePercentageOfType(Coordinate coord, TerrainType type, int areaSize)
    {
        int count = 0;
        for (int i = -areaSize; i < areaSize; i++)
            for (int j = -areaSize; j < areaSize; j++)
            {
                Coordinate searchCoord = coord + new Coordinate(i, j);
                if (tileDict.Keys.Contains(searchCoord) && tileDict[searchCoord].Type == type)
                    count++;
            }

        return count / (areaSize * areaSize);
    }
}

[System.Serializable]
public struct Coordinate
{
    public const float kEpsilon = 1E-05F;
    //
    // Summary:
    //     X component of the vector.
    public float x;
   
    public float y;
    public Coordinate(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
    public Coordinate(Coordinate a)
    {
        this.x = a.x;
        this.y = a.y;
    }

    public static Coordinate east { get { return new Coordinate(1, 0); } }
    public static Coordinate west { get { return new Coordinate(-1, 0); } }
    public static Coordinate north { get { return new Coordinate(0, 1); } }
    public static Coordinate south { get { return new Coordinate(0, -1); } }
    public static Coordinate zero { get { return new Coordinate(0, 0); } }

    public static Coordinate operator +(Coordinate a, Coordinate b)
    {
        return new Coordinate(a.x + b.x, a.y + b.y);
    }
    public static Coordinate operator -(Coordinate a)
    {
        return new Coordinate(-a.x, -a.y);
    }
    public static Coordinate operator -(Coordinate a, Coordinate b)
    {
        return new Coordinate(a.x - b.x, a.y - b.y);
    }
    public static Coordinate operator *(float d, Coordinate a)
    {
        return new Coordinate(d * a.x, d * a.y);
    }
    public static Coordinate operator *(Coordinate a, float d)
    {
        return new Coordinate(d * a.x, d * a.y);
    }
    
    public static Coordinate operator /(Coordinate a, float d)
    {
        return new Coordinate( a.x /d,  a.y /d);
    }
    public  static bool operator ==(Coordinate lhs, Coordinate rhs)
    {
        if (Mathf.Abs(lhs.x - rhs.x) < kEpsilon &&
            Mathf.Abs(lhs.y - rhs.y) < kEpsilon)
            return true;
        else
            return false;
    }
    public static bool operator !=(Coordinate lhs, Coordinate rhs)
    {
        return !(lhs == rhs);
    }

    public List<Coordinate> GetNeighbors()
    {
        List<Coordinate> output = new List<Coordinate>();
        output.Add(this + Coordinate.east);
        output.Add(this + Coordinate.west);
        output.Add(this + Coordinate.north);
        output.Add(this + Coordinate.south);
        return output;
    }

    public Vector2 ToVector2()
    {
        return new Vector2(this.x, this.y);
    }

    public Coordinate Round() 
    {
        return new Coordinate(Mathf.Round(this.x), Mathf.Round(this.y));
    }

    public static float Distance(Coordinate a, Coordinate b)
    {
        return Vector2.Distance(a.ToVector2(), b.ToVector2());
    }
}

public class CoordinateObject
{
    public Coordinate coordinate { get; private set; }
    public CoordinateObject() { }
    public CoordinateObject(Coordinate coordinate) { this.coordinate = coordinate; }
    public CoordinateObject(float x, float y) 
    {
        this.coordinate = new Coordinate(x, y);
    }
    public void Set(Coordinate coordinate) => this.coordinate = coordinate;
    public Coordinate Value { get { return coordinate; } }
}

public class CoordinateBound
{
    public Coordinate lower { get; set; }
    public Coordinate upper { get; set; }

    public CoordinateBound(Coordinate lowerLeft, Coordinate upperRight)
    {
        this.lower = lowerLeft;
        this.upper = upperRight;
    }
    public CoordinateBound Copy(CoordinateBound bound)
    {
        CoordinateBound copy = new CoordinateBound(bound.lower, bound.upper);
        return copy;
    }

    public bool IsWithinBound(Coordinate coord)
    {
        if (upper.x >= coord.x & upper.y >= coord.y &
            coord.x >= lower.x & coord.y >= lower.y)
            return true;
        else
            return false;
    }

    public Coordinate ClampCoordinateWithinBound(Coordinate coord)
    {
        Coordinate newCoord = new Coordinate(coord);
        if (newCoord.x > upper.x)
            newCoord = new Coordinate(upper.x, newCoord.y);
        if (newCoord.y > upper.y)
            newCoord = new Coordinate(newCoord.x, upper.y);
        
        if (newCoord.x < lower.x)
            newCoord = new Coordinate(lower.x, newCoord.y);
        if (newCoord.y < lower.y)
            newCoord = new Coordinate(newCoord.x, lower.y);

        return newCoord;
    }

    public void ShiftXBy(float dx)
    {
        lower = new Coordinate(lower.x + dx, lower.y);
        upper = new Coordinate(upper.x + dx, upper.y);
    }

    public void ShiftYBy(float dy)
    {
        lower = new Coordinate(lower.x, lower.y + dy);
        upper = new Coordinate(upper.x, upper.y + dy);
    }

    public void ShiftBy(float dx, float dy)
    {
        lower = new Coordinate(lower.x + dx, lower.y + dy);
        upper = new Coordinate(upper.x + dx, upper.y + dy);
    }

    // Follow smaller of the this
    public void ClampTo(CoordinateBound restrictBound)
    {
        if (this.upper.y > restrictBound.upper.y)
            this.upper = new Coordinate(this.upper.x, restrictBound.upper.y);
        if (this.upper.x > restrictBound.upper.x)
            this.upper = new Coordinate(restrictBound.upper.x, this.upper.y);
        if (this.lower.y < restrictBound.lower.y)
            this.lower = new Coordinate(this.lower.x, restrictBound.lower.y);
        if (this.lower.x < restrictBound.lower.x)
            this.lower = new Coordinate(restrictBound.lower.x, this.lower.y);
    }
}


public static class CoordinateMath
{
    /// <summary>
    /// Function to ensure all coordinates satisfy minimum distance threshold
    /// </summary>
    /// <param name="coords"></param>
    /// <returns></returns>
    //public static List<Coordinate> __MinimumDistanceFilter(List<Coordinate> coords, int threshold, int maxIteration = 200)
    //{
       
    //    // Create indexing, for later revert
    //    Dictionary<Coordinate, int> indexDict = new Dictionary<Coordinate, int>();
    //    for (int i = 0; i < coords.Count; i++)
    //        indexDict.Add(coords[i], i);


    //    // Create pair/distance dict. For future record purpose
    //    Dictionary<Pair, float> pairDistance = new Dictionary<Pair, float>();
    //    Dictionary<int, Coordinate> newIndexDict = new Dictionary<int, Coordinate>();
    //    for (int i = 0; i < coords.Count; i++)
    //        for (int j = i + 1; j < coords.Count; j++)
    //        {
    //            int indexA = indexDict[coords[i]];
    //            int indexB = indexDict[coords[j]];
    //            Pair pair = new Pair(coords[i], coords[j], indexA, indexB);
    //            pairDistance.Add(pair, pair.Distance());
    //        }

    //    // Algorithm start here         
    //    int iter = 0;
    //    while (true)
    //    {
    //        // Find lowest distance pair for all pair
    //        float lowestDistance = Mathf.Infinity;
    //        Pair lowestPair = null;
    //        foreach (var pair in pairDistance.Keys)
    //        {
    //            float dist = pairDistance[pair];
    //            if(dist < lowestDistance)
    //            {
    //                lowestDistance = dist;
    //                lowestPair = pair;
    //            }
    //        }

    //        // Check exit condition
    //        if (iter > maxIteration)
    //        {
    //            Debug.LogWarning("[MinimumDistanceFilter] Max iteration reached!");
    //            break;
    //        }
    //        else if (lowestDistance > threshold)
    //            break;

    //        // Displace this pair distance
    //        lowestPair.EnlargeDistance();

    //        // Update
    //        pairDistance[lowestPair] = lowestPair.Distance();

                
    //        iter++;
    //    }

    //    Debug.Log("Iteration this round " + iter);

    //    // Provide output
    //    List<Coordinate> output = new List<Coordinate>();
    //    for (int i = 0; i < coords.Count; i++) // Follow order 
    //    {
    //        // Search the order
    //        foreach (var pair in pairDistance.Keys)
    //        {
    //            if (pair.indexForA == i)
    //            {
    //                output.Add(pair.a.Value);
    //                break;
    //            }
    //            else if (pair.indexForB == i)
    //            {
    //                output.Add(pair.b.Value);
    //                break;
    //            }
    //        }
    //    }

    //    // Sanity check
    //    if (output.Count != coords.Count)
    //        Debug.LogError("Problem in algo");

    //    Coordinate[] _test = FindMinimumDistanceCoords(coords);
    //    float _dist = Coordinate.Distance(_test[0], _test[1]);
    //    if(_dist < threshold)
    //        Debug.LogError("Algo still given large distance");

    //    return output;
    //}

    public static List<Coordinate> MinimumDistanceFilter(List<Coordinate> coords, int threshold, int maxIteration = 300)
    {
        // Create Instances

        // Create indexing, for later revert
        Dictionary<Coordinate, int> indexDict = new Dictionary<Coordinate, int>();
        List<CoordinateObject> coordObjects = new List<CoordinateObject>();
        for (int i = 0; i < coords.Count; i++)
        {
            CoordinateObject obj = new CoordinateObject(coords[i]);
            indexDict.Add(obj.Value, i);
            coordObjects.Add(obj);
        }


        // Create pair
        List<Pair> pairs = new List<Pair>();
        for (int i = 0; i < coords.Count; i++)
            for (int j = i + 1; j < coords.Count; j++)
            {
                int indexA = indexDict[coordObjects[i].Value];
                int indexB = indexDict[coordObjects[j].Value];
                Pair pair = new Pair(coordObjects[i], coordObjects[j], indexA, indexB);
                pairs.Add(pair);
            }

        // Algorithm start here         
        int iter = 0;
        while (true)
        {
            // Find lowest distance pair for all pair
            float lowestDistance = Mathf.Infinity;
            Pair lowestPair = null;
            foreach (var pair in pairs)
            {
                float dist = pair.Distance();
                if (dist < lowestDistance)
                {
                    lowestDistance = dist;
                    lowestPair = pair;
                }
            }

            // Check exit condition
            if (iter > maxIteration)
            {
                Debug.LogWarning("[MinimumDistanceFilter] Max iteration reached!");
                break;
            }
            else if (lowestDistance > threshold)
                break;

            // Displace this pair distance
            lowestPair.EnlargeDistance();

            iter++;
        }

        Debug.Log("Iteration this round " + iter);

        // Provide output
        List<Coordinate> output = new List<Coordinate>();
        for (int i = 0; i < coords.Count; i++) // Follow order 
        {
            // Search the order
            foreach (var pair in pairs)
            {
                if (pair.indexForA == i)
                {
                    output.Add(pair.a.Value);
                    break;
                }
                else if (pair.indexForB == i)
                {
                    output.Add(pair.b.Value);
                    break;
                }
            }
        }

        // Sanity check
        if (output.Count != coords.Count)
            Debug.LogError("Problem in algo. output.Count != coords.Count");

        Coordinate[] _test = FindMinimumDistanceCoords(output);
        float _dist = Coordinate.Distance(_test[0], _test[1]);
        if (_dist < threshold)
            Debug.LogWarning("Algo still given large distance");

        return output;
    }

    public static Coordinate[] FindMinimumDistanceCoords(List<Coordinate> coords)
    {
        // Create pair/distance dict. For future record purpose
        Dictionary<Pair, float> pairDistance = new Dictionary<Pair, float>();
        for (int i = 0; i < coords.Count; i++)
            for (int j = i + 1; j < coords.Count; j++)
            {
                Pair pair = new Pair( new CoordinateObject(coords[i]), new CoordinateObject(coords[j]));
                pairDistance.Add(pair, pair.Distance());                
            }

        // Find lowest distance pair
        float lowestDistance = Mathf.Infinity;
        Pair lowestPair = null;
        foreach (var pair in pairDistance.Keys)
        {
            float dist = pairDistance[pair];
            if (dist < lowestDistance)
            {
                lowestDistance = dist;
                lowestPair = pair;
            }
        }

        return new Coordinate[2] { lowestPair.a.Value, lowestPair.b.Value };
    }

    class Pair
    {
        public CoordinateObject a;
        public CoordinateObject b;
        public int indexForA;
        public int indexForB;

        public Pair(CoordinateObject a, CoordinateObject b)
        {
            this.a = a;
            this.b = b;
        }

        public Pair(CoordinateObject a, CoordinateObject b, int indexForA, int indexForB)
        {
            this.a = a;
            this.b = b;
            this.indexForA = indexForA;
            this.indexForB = indexForB;
        }

        public float Distance()
        {
            return Coordinate.Distance(a.coordinate, b.coordinate);
        }

        // Move both X and Y simultanouesly to increase their distance
        public void EnlargeDistance()
        {
            Vector2 avec = a.coordinate.ToVector2();
            Vector2 bvec = b.coordinate.ToVector2();

            Vector2 dir = bvec - avec;
            dir.Normalize();

            // Move them apart by 1 unit
            avec -= dir;
            bvec += dir;

            // Convert back to coord
            this.a.Set(new Coordinate(Mathf.RoundToInt(avec.x), Mathf.RoundToInt(avec.y)));
            this.b.Set(new Coordinate(Mathf.RoundToInt(bvec.x), Mathf.RoundToInt(bvec.y)));
        }
    }
}