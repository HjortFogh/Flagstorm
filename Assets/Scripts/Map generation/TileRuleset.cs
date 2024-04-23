using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tile Ruleset", menuName = "Capture the Flag/Tile Ruleset")]
public class TileRuleset : ScriptableObject
{
    public int weight = 1;

    public List<TileRuleset> north;
    public List<TileRuleset> east;
    public List<TileRuleset> south;
    public List<TileRuleset> west;

    public TileType type;
}