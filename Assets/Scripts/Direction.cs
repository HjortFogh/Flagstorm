using UnityEngine;

public enum Cardinal
{
    North = 0, East, South, West
};

public static class Directions
{
    public static Cardinal OppositeCardinal(Cardinal dir) { return (Cardinal)(((int)dir + 2) % 4); }

    public static Vector2Int? CardinalToMove(Cardinal cardinal)
    {
        return cardinal switch
        {
            Cardinal.North => (Vector2Int?)new Vector2Int(0, +1),
            Cardinal.East => (Vector2Int?)new Vector2Int(+1, 0),
            Cardinal.South => (Vector2Int?)new Vector2Int(0, -1),
            Cardinal.West => (Vector2Int?)new Vector2Int(-1, 0),
            _ => null,
        };
    }
}


