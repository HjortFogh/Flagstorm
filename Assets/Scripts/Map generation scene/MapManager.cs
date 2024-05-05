using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GenerationConfig generationConfig;

    private void Start()
    {
        MapGenerator.Generate(generationConfig);
    }
}
