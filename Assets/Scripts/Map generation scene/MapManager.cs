using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GenerationConfig generationConfig;

    void Start()
    {
        MapGenerator.Generate(generationConfig);
    }
}
