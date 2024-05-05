using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Hivemind : MonoBehaviour
{
    /*
      -- Heat Map Generation
    Flag reward     -- gain heat from being close to flag
    Path reward     -- gain heat from being a valid path
    Blockade reward -- gain heat from blocking a passage

    */

    [SerializeField]
    private MapHandler m_MapHandler;

    private float[] m_HeatMap;

    private void ApplyBlur(float[] heatmap, CellType[] grid, int kernalSize = 1)
    {
        int area = (kernalSize * 2 + 1) * (kernalSize * 2 + 1);

        for (int i = 0; i < grid.Length; i++)
        {
            // if (grid[i] == CellType.NonWalkable || grid[i] == CellType.Barricade)
            //     continue;

            float sum = 0.0f;
            // float nonWalkableSum = 0.0f;
            // int numNonWalkable = 0;

            foreach (int j in m_MapHandler.InRadius(m_MapHandler.IndexToCoordinate(i), kernalSize))
            {
                if (grid[j] == CellType.Walkable || grid[j] == CellType.Blocked)
                    sum += heatmap[j];
                else if (grid[j] == CellType.NonWalkable || grid[j] == CellType.Barricade)
                    // numNonWalkable++;
                    sum += 0.5f;
            }

            // float f = (float)numNonWalkable / area;
            // sum += sum * Mathf.Pow(f, 8f) * 10f;

            // if (f > 0.3f)
            //     sum += maxHeat * area;

            // sum += Mathf.Pow((float)numNonWalkable / area, 2f) * maxHeat;
            heatmap[i] += sum / area;
        }
    }

    private float[] GenerateHeatMap()
    {
        CellType[] grid = m_MapHandler.Grid;
        Vector2Int flagPos = m_MapHandler.FlagPosition;

        float[] heatmap = new float[grid.Length];

        // Set inital heat to zero
        for (int i = 0; i < grid.Length; i++)
            heatmap[i] = 0f;

        // Add heat around flag
        foreach (int i in m_MapHandler.InRadius(flagPos, m_MapHandler.flagClearRadius))
            heatmap[i] += 1.5f;

        // Set heat at flag position to zero
        heatmap[m_MapHandler.CoordinateToIndex(flagPos)] = 0f;

        ApplyBlur(heatmap, grid, 2);

        for (int i = 0; i < grid.Length; i++)
        {
            if (grid[i] != CellType.Walkable)
                heatmap[i] = 0f;
        }

        return heatmap;
    }

    public void PlaceBarricade()
    {
        m_HeatMap = GenerateHeatMap();
        int maxIndex = System.Array.IndexOf(m_HeatMap, m_HeatMap.Max());
        m_MapHandler.TryPlaceBarricade(maxIndex);
    }

    private void Start()
    {
        PlaceBarricade();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            PlaceBarricade();
    }

    private void OnDrawGizmos()
    {
        if (m_HeatMap == null)
            return;

        for (int i = 0; i < m_HeatMap.Length; i++)
        {
            Vector2Int coord = m_MapHandler.IndexToCoordinate(i);
            float heat = m_HeatMap[i];

            Vector3 center = new(coord.x, 0.2f, coord.y);
            Vector3 size = new(0.35f, 0.1f, 0.35f);

            Gizmos.color = new Color(heat, heat, heat);
            Gizmos.DrawCube(center, size);
        }
    }
}
