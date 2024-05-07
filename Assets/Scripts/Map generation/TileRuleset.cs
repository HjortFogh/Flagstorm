using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class describing a specific tile and its ruleset used by the WFC-algorithm
/// </summary>
[CreateAssetMenu(fileName = "New Tile Ruleset", menuName = "Capture the Flag/Tile Ruleset")]
public class TileRuleset : ScriptableObject
{
    public TileType type;
    public GameObject prefab;
    public bool rotatable = false;

    public int weight = 1;

    public List<TileRuleset> north;
    public List<TileRuleset> east;
    public List<TileRuleset> south;
    public List<TileRuleset> west;

    /// <summary>
    /// Place the correct prefab in the correct location in the world
    /// </summary>
    public virtual void PlaceInWorld(int x, int y, TileType[] surrounding)
    {
        Vector3 position = new(x, 0, y);
        Quaternion rotation = Quaternion.identity;
        if (rotatable)
            rotation = Directions.CardinalToRotation((Cardinal)Random.Range(0, 4));
        Instantiate(prefab, position, rotation);
    }
}
