using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellType { Walkable, NonWalkable, Barricade, Flag, Blocked }

public class MapHandler : MonoBehaviour
{
    [Header("Map Generation")]
    public Vector2Int mapSize = new(20, 10);
    private CellType[] m_Grid;
    private Vector2Int m_FlagPosition;
    public int flagClearRadius = 2;

    [Header("Noise Settings")]
    public float noiseSmoothness = 0.35f;
    public float noiseHeight = 0.35f;

    public int CoordinateToIndex(Vector2Int coord) { return coord.x * mapSize.y + coord.y; }
    public int CoordinateToIndex(int x, int y) { return x * mapSize.y + y; }
    public Vector2Int IndexToCoordinate(int i) { return new(i / mapSize.y, i % mapSize.y); }
    public static int CoordinateToIndex(Vector2Int coord, Vector2Int mapSize) { return coord.x * mapSize.y + coord.y; }
    public static int CoordinateToIndex(int x, int y, Vector2Int mapSize) { return x * mapSize.y + y; }
    public static Vector2Int IndexToCoordinate(int i, Vector2Int mapSize) { return new(i / mapSize.y, i % mapSize.y); }

    public CellType[] Grid { get { if (m_Grid == null) (m_Grid, m_FlagPosition) = GenerateMap(); return m_Grid; } }
    public Vector2Int FlagPosition { get { return m_FlagPosition; } }

    public IEnumerable<int> InRadius(Vector2Int coord, int radius)
    {
        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                int x = coord.x + dx, y = coord.y + dy;
                if (x < 0 || x >= mapSize.x || y < 0 || y >= mapSize.y)
                    continue;
                yield return CoordinateToIndex(x, y);
            }
        }
    }

    public IEnumerable<int> TopRow()
    {
        for (int x = 0; x < mapSize.x; x++)
            yield return CoordinateToIndex(x, mapSize.y - 1);
    }

    private (CellType[], Vector2Int) GenerateMap()
    {
        CellType[] grid = new CellType[mapSize.x * mapSize.y];

        Vector2 noiseOffset = new(Random.Range(-999999f, +999999f), Random.Range(-999999f, +999999f));

        for (int i = 0; i < grid.Length; i++)
        {
            Vector2Int coord = IndexToCoordinate(i);
            float noise = Mathf.PerlinNoise(coord.x * noiseSmoothness + noiseOffset.x, coord.y * noiseSmoothness + noiseOffset.y);
            grid[i] = noise > noiseHeight ? CellType.Walkable : CellType.NonWalkable;
        }

        Vector2Int flagPos = new(Random.Range(0, mapSize.x), 1);

        // List<int> availableTopCells = new();

        // foreach (int i in TopRow())
        // {
        //     if (grid[i] != CellType.Walkable)
        //         continue;

        //     Vector2Int coord = IndexToCoordinate(i);
        //     int distance = AStar.Distance(flagPos, coord, grid, mapSize);

        //     if (distance != 0)
        //         availableTopCells.Add(i);
        // }

        // if (availableTopCells.Count == 0)
        //     return GenerateMap();

        // int selectedTopCell = availableTopCells[Random.Range(0, availableTopCells.Count)];
        // foreach (Vector2Int coord in AStar.Path(flagPos, IndexToCoordinate(selectedTopCell), grid, mapSize))
        // {
        //     int index = CoordinateToIndex(coord);
        //     grid[index] = CellType.Blocked;
        // }

        foreach (int index in InRadius(flagPos, flagClearRadius))
            grid[index] = CellType.Walkable;

        grid[CoordinateToIndex(flagPos)] = CellType.Flag;

        return (grid, flagPos);
    }

    public bool TryPlaceBarricade(int index)
    {
        if (m_Grid[index] != CellType.Walkable)
            return false;

        m_Grid[index] = CellType.Barricade;
        return true;
    }

    private void Start()
    {
        (m_Grid, m_FlagPosition) = GenerateMap();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            (m_Grid, m_FlagPosition) = GenerateMap();
    }

    private readonly Color[] m_GizmosColors = { Color.white, Color.black, Color.red, Color.green, Color.gray };

    private void OnDrawGizmos()
    {
        if (m_Grid == null)
            return;

        for (int i = 0; i < m_Grid.Length; i++)
        {
            Vector2Int coord = IndexToCoordinate(i);

            Vector3 center = new(coord.x, 0, coord.y);
            Vector3 size = new(0.7f, 0.1f, 0.7f);

            Gizmos.color = m_GizmosColors[(int)m_Grid[i]];
            Gizmos.DrawCube(center, size);
        }
    }
}
