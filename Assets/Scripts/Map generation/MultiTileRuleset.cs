using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Multi-Tile Ruleset", menuName = "Capture the Flag/Multi-Tile Ruleset")]
public class MultiTileRuleset : TileRuleset
{
    public GameObject[] prefabs = new GameObject[6];

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
        if (surrounding.Length != 4)
            return new Placement(prefabs[0], Cardinal.North);

        int bitmask = 0;
        for (int i = 0; i < 4; i++)
        {
            if (surrounding[i] == type)
                bitmask |= 1 << i;
        }

        return bitmask switch
        {
            // End tile
            0b0001 => new Placement(prefabs[1], Cardinal.North),
            0b0010 => new Placement(prefabs[1], Cardinal.East),
            0b0100 => new Placement(prefabs[1], Cardinal.South),
            0b1000 => new Placement(prefabs[1], Cardinal.West),

            // Line tile
            0b0101 => new Placement(prefabs[2], Cardinal.North),
            0b1010 => new Placement(prefabs[2], Cardinal.East),

            // Turn tile
            0b1001 => new Placement(prefabs[3], Cardinal.North),
            0b0011 => new Placement(prefabs[3], Cardinal.East),
            0b0110 => new Placement(prefabs[3], Cardinal.South),
            0b1100 => new Placement(prefabs[3], Cardinal.West),

            // T tile
            0b1101 => new Placement(prefabs[4], Cardinal.North),
            0b1011 => new Placement(prefabs[4], Cardinal.East),
            0b0111 => new Placement(prefabs[4], Cardinal.South),
            0b1110 => new Placement(prefabs[4], Cardinal.West),

            // Cross tile
            0b1111 => new Placement(prefabs[5], Cardinal.North),

            // Dot tile
            _ => new Placement(prefabs[0], Cardinal.North),
        };
    }

    public override void PlaceInWorld(int x, int y, TileType[] surrounding)
    {
        Placement placement = PickCorrectPlacement(surrounding);
        Vector3 position = new(x, 0, y);
        Instantiate(placement.prefab, position, Directions.CardinalToRotation(placement.direction));
    }
}