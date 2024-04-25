using UnityEngine;

[CreateAssetMenu(fileName = "New Tile Type", menuName = "Capture the Flag/Tile Type")]
public class TileType : ScriptableObject
{
    public string identifier = "";
    public bool walkable = false;
}
