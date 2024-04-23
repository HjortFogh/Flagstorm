using UnityEngine;

[CreateAssetMenu(fileName = "New Tile Type", menuName = "Capture the Flag/Tile Type")]
public class TileType : ScriptableObject
{
    public int identifier = 0;
    public GameObject prefab;
    public bool walkable = false;

    public virtual void PlaceInWorld(int x, int y)
    {
        Vector3 position = new(x, 0, y);
        Object.Instantiate(prefab, position, Quaternion.identity);
    }
}

[CreateAssetMenu(fileName = "New Multi-Tile Type", menuName = "Capture the Flag/Multi-Tile Type")]
public class MultiTileType : TileType
{
    public override void PlaceInWorld(int x, int y)
    {
        throw new System.NotImplementedException("");
    }
}