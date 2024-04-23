using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ColorToTileRuleset
{
    public Color color;
    public TileRuleset tile;

    public static TileRuleset Find(List<ColorToTileRuleset> list, Color key)
    {
        foreach (ColorToTileRuleset pair in list)
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

[CreateAssetMenu(fileName = "New Generation Config", menuName = "Capture the Flag/Generation Config")]
public class GenerationConfig : ScriptableObject
{
    public List<TileRuleset> tileset;
    public int width = 28, height = 28;
    public List<Texture2D> maps;
    public List<ColorToTileRuleset> mapper;
}
