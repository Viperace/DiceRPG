using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// What kind of tile is it, where is it located
/// </summary>
public class Tile
{
    public TerrainType Type { get; set; }
    public int Elevation { get; set; }  // Decide ice or grass or sea
    public int SoilFertility { get; set; }  // Decide tree grows or not
    public Tile() { }
    public Tile(TerrainType type)
    {
        this.Type = type;
    }
}

public enum TerrainType
{
    Grass,
    Plains,
    Lava,
    Snow,
    Water
}
