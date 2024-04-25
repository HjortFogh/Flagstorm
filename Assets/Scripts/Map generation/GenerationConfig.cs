using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ColorRulesetPair
{
    public Color color;
    public TileRuleset tile;
}

[System.Serializable]
public class GenerationTexture
{
    public List<ColorRulesetPair> pairs;
    public List<Texture2D> textures;
}

[CreateAssetMenu(fileName = "New Generation Config", menuName = "Capture the Flag/Generation Config")]
public class GenerationConfig : ScriptableObject
{
    public List<TileRuleset> tileset;
    public int width = 28, height = 28;
    [SerializeField]
    private GenerationTexture m_GenerationTexture;

    public Texture2D PickTexture()
    {
        if (m_GenerationTexture.textures.Count == 0)
            return null;

        int index = Random.Range(0, m_GenerationTexture.textures.Count);
        return m_GenerationTexture.textures[index];
    }

    public TileRuleset PixelToRuleset(Color pixel)
    {
        if (!Mathf.Approximately(pixel.a, 1.0f))
            return null;

        foreach (ColorRulesetPair pair in m_GenerationTexture.pairs)
        {
            if (
                Mathf.Approximately(pair.color.r, pixel.r) &&
                Mathf.Approximately(pair.color.g, pixel.g) &&
                Mathf.Approximately(pair.color.b, pixel.b)
                )
                return pair.tile;
        }

        return null;
    }
}
