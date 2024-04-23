using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ColorToTileObject {
    public Color color;
    public TileObject tile;

    public static TileObject Find(List<ColorToTileObject> list, Color key) {
        foreach (ColorToTileObject pair in list)
        {
            if (
                Mathf.Approximately(pair.color.r, key.r) &&
                Mathf.Approximately(pair.color.g, key.g) &&
                Mathf.Approximately(pair.color.b, key.b)
            )
                return pair.tile;
        }
        return null;
    }
}

[CreateAssetMenu(fileName = "New generation config", menuName = "Capture the Flag/Generation Config")]
public class GenerationConfig : ScriptableObject
{
    public List<TileObject> tileset;
    public int width = 28, height = 28;
    public List<Texture2D> maps;
    public List<ColorToTileObject> mapper;
}
