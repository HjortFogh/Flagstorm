using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New tile", menuName = "Capture the Flag/Tile Object")]
public class TileObject : ScriptableObject
{
    public GameObject prefab;
    public int weight = 1;
    public List<TileObject> north;
    public List<TileObject> east;
    public List<TileObject> south;
    public List<TileObject> west;
}