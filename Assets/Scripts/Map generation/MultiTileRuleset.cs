using UnityEngine;

/// <summary>
/// A child-class of the TileRuleset-class, but where the tile prefab depends on surrounding tiles
/// </summary>
[CreateAssetMenu(fileName = "New Multi-Tile Ruleset", menuName = "Capture the Flag/Multi-Tile Ruleset")]
public class MultiTileRuleset : TileRuleset
{
    public GameObject[] prefabs = new GameObject[6];

    /// <summary>
    /// Chooses the correct prefab model given the surrounding tile-types
    /// </summary>
    private (GameObject, Cardinal) PickCorrectPlacement(TileType[] surrounding)
    {
        if (surrounding.Length != 4)
            return (prefabs[0], Cardinal.North);

        int bitmask = 0;
        for (int i = 0; i < 4; i++)
        {
            if (surrounding[i] == type)
                bitmask |= 1 << i;
        }

        return bitmask switch
        {
            // End tile
            0b0001 => (prefabs[1], Cardinal.North),
            0b0010 => (prefabs[1], Cardinal.East),
            0b0100 => (prefabs[1], Cardinal.South),
            0b1000 => (prefabs[1], Cardinal.West),

            // Line tile
            0b0101 => (prefabs[2], Cardinal.North),
            0b1010 => (prefabs[2], Cardinal.East),

            // Turn tile
            0b1001 => (prefabs[3], Cardinal.North),
            0b0011 => (prefabs[3], Cardinal.East),
            0b0110 => (prefabs[3], Cardinal.South),
            0b1100 => (prefabs[3], Cardinal.West),

            // T tile
            0b1101 => (prefabs[4], Cardinal.North),
            0b1011 => (prefabs[4], Cardinal.East),
            0b0111 => (prefabs[4], Cardinal.South),
            0b1110 => (prefabs[4], Cardinal.West),

            // Cross tile
            0b1111 => (prefabs[5], Cardinal.North),

            // Dot tile
            _ => (prefabs[0], Cardinal.North),
        };
    }

    /// <summary>
    /// Places this tile in world
    /// </summary>
    public override void PlaceInWorld(int x, int y, TileType[] surrounding)
    {
        (GameObject prefab, Cardinal direction) = PickCorrectPlacement(surrounding);
        Vector3 position = new(x, 0, y);
        Instantiate(prefab, position, Directions.CardinalToRotation(direction));
    }
}