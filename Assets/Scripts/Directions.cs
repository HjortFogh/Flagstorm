using UnityEngine;

/// <summary>
/// Enum to represent the cardinal directions
/// </summary>
public enum Cardinal
{
    North = 0, East, South, West
};

/// <summary>
/// Static class to handle directions
/// </summary>
public static class Directions
{
    /// <summary>
    /// Returns the opposite cardinal direction of the given one
    /// </summary>
    public static Cardinal OppositeCardinal(Cardinal dir) { return (Cardinal)(((int)dir + 2) % 4); }

    /// <summary>
    /// Returns which direction to move in based on the given cardinal direction
    /// </summary>
    public static Vector2Int CardinalToMove(Cardinal cardinal)
    {
        return cardinal switch
        {
            Cardinal.East => new Vector2Int(+1, 0),
            Cardinal.South => new Vector2Int(0, -1),
            Cardinal.West => new Vector2Int(-1, 0),
            _ => new Vector2Int(0, +1),
        };
    }

    /// <summary>
    /// Maps a cardinal direction to a rotation
    /// </summary>
    public static Quaternion CardinalToRotation(Cardinal cardinal)
    {
        Vector2Int move2D = CardinalToMove(cardinal);
        Vector3 move = new(move2D.x, 0, move2D.y);
        return Quaternion.LookRotation(move);
    }
}
