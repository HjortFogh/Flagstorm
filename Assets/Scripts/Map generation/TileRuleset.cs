using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tile Ruleset", menuName = "Capture the Flag/Tile Ruleset")]
public class TileRuleset : ScriptableObject
{
    public TileType type;
    public GameObject prefab;

    public int weight = 1;

    public List<TileRuleset> north;
    public List<TileRuleset> east;
    public List<TileRuleset> south;
    public List<TileRuleset> west;

    public virtual void PlaceInWorld(int x, int y, TileType[] surrounding)
    {
        Vector3 position = new(x, 0, y);
        Instantiate(prefab, position, Quaternion.identity);
    }
}

[CreateAssetMenu(fileName = "New Multi-Tile Ruleset", menuName = "Capture the Flag/Multi-Tile Ruleset")]
public class MultiTileRuleset : TileRuleset
{
    public new GameObject[] prefab = new GameObject[6];

    private struct Placement
    {
        public GameObject prefab;
        public Cardinal direction;

        public Placement(GameObject _prefab, Cardinal _direction)
        {
            prefab = _prefab;
            direction = _direction;
        }
    }

    private Placement PickCorrectPlacement(TileType[] surrounding)
    {
        if (prefab.Length != 4)
            return new Placement(prefab[0], Cardinal.North);

        int bitmask = 0;
        for (int i = 0; i < 4; i++)
        {
            if (surrounding[i] == type)
                bitmask |= 1 << i;
        }

        return bitmask switch
        {
            // End tile
            0b1000 => new Placement(prefab[1], Cardinal.North),
            0b0100 => new Placement(prefab[1], Cardinal.East),
            0b0010 => new Placement(prefab[1], Cardinal.South),
            0b0001 => new Placement(prefab[1], Cardinal.West),

            // Line tile
            0b1010 => new Placement(prefab[2], Cardinal.North),
            0b0101 => new Placement(prefab[2], Cardinal.East),

            // Turn tile
            0b1100 => new Placement(prefab[3], Cardinal.North),
            0b0110 => new Placement(prefab[3], Cardinal.East),
            0b0011 => new Placement(prefab[3], Cardinal.South),
            0b1001 => new Placement(prefab[3], Cardinal.West),

            // T tile
            0b0111 => new Placement(prefab[4], Cardinal.North),
            0b1011 => new Placement(prefab[4], Cardinal.East),
            0b1101 => new Placement(prefab[4], Cardinal.South),
            0b1110 => new Placement(prefab[4], Cardinal.West),

            // Cross tile
            0b1111 => new Placement(prefab[5], Cardinal.North),

            // Dot tile
            _ => new Placement(prefab[0], Cardinal.North),
        };
    }

    public override void PlaceInWorld(int x, int y, TileType[] surrounding)
    {
        Placement placement = PickCorrectPlacement(surrounding);
        Vector3 position = new(x, 0, y);
        Instantiate(placement.prefab, position, Directions.CardinalToRotation(placement.direction));
    }
}