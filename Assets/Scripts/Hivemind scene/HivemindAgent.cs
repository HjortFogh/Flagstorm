using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using System.Linq;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class HivemindAgent : Agent
{
    private enum CellType { NonWalkable, Walkable, Barricade, Flag }

    /*
    Map contains integer values where 0 = non-walkable, 1 = walkable, 2 = barricade
    Barricades can be placed on walkable squares
    */
    public Vector2Int mapSize = new(28, 8);
    private int[] m_Grid;

    public int numTurnsBeforeReset = 20;
    private int m_NumTurns;

    private Vector2Int flagPosition;

    // ...

    private readonly Color[] m_GizmosColors = { Color.black, Color.white, Color.red, Color.green };

    public float noiseSmoothness = 0.5f;
    public float walkableHeight = 0.3f;
    // public float distanceDivideFactor = 40f;

    private void Awake()
    {
        BehaviorParameters parameters = GetComponent<BehaviorParameters>();

        parameters.BehaviorName = "Hivemind Agent";
        // parameters.BrainParameters.VectorObservationSize = mapSize.x * mapSize.y;
        parameters.BrainParameters.VectorObservationSize = 2;
        // parameters.BrainParameters.ActionSpec = new ActionSpec(0, new int[] { mapSize.x, mapSize.y });
        parameters.BrainParameters.ActionSpec = new ActionSpec(mapSize.x * mapSize.y);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            RequestDecision();
        if (Input.GetKeyDown(KeyCode.Escape))
            EndEpisode();
    }

    private int CoordinateToIndex(Vector2Int coord) { return CoordinateToIndex(coord.x, coord.y); }
    private int CoordinateToIndex(int x, int y) { return x * mapSize.y + y; }
    private Vector2Int IndexToCoordinate(int i) { return new(i / mapSize.y, i % mapSize.y); }

    private IEnumerable<Vector2Int> TopRow(int[] grid)
    {
        List<Vector2Int> cells = new();
        for (int x = 0; x < mapSize.x; x++)
        {
            int index = CoordinateToIndex(x, mapSize.y - 1);
            if (grid[index] == (int)CellType.Walkable)
                cells.Add(new(x, mapSize.y - 1));
        }
        return cells;
    }

    private bool CheckIfValidMap(int[] grid, Vector2Int flagPos)
    {
        foreach (Vector2Int cell in TopRow(grid))
        {
            if (AStar.AStarDistance(flagPos, cell, grid, mapSize) != int.MaxValue)
                return true;
        }
        return false;
    }

    private int[] GenerateMap()
    {
        Vector2 noiseOffset = new(Random.Range(-999999f, +999999f), Random.Range(-999999f, +999999f));

        int[] grid = new int[mapSize.x * mapSize.y];
        for (int i = 0; i < grid.Length; i++)
        {
            Vector2Int coord = IndexToCoordinate(i);
            float z = Mathf.PerlinNoise(coord.x * noiseSmoothness + noiseOffset.x, coord.y * noiseSmoothness + noiseOffset.y);
            grid[i] = z > walkableHeight ? (int)CellType.Walkable : (int)CellType.NonWalkable;
        }

        flagPosition = new(Random.Range(0, mapSize.x), 1);
        grid[CoordinateToIndex(flagPosition)] = (int)CellType.Flag;

        if (!CheckIfValidMap(grid, flagPosition))
            return GenerateMap();

        return grid;
    }

    private void RecalculateRewards(int xBarricade, int yBarricade)
    {
        int index = CoordinateToIndex(xBarricade, yBarricade);
        if (m_Grid[index] != (int)CellType.Walkable)
            AddReward(-1f);
        if (xBarricade == flagPosition.x && yBarricade == flagPosition.y)
            AddReward(-5f);

        Vector2Int barricade = new(xBarricade, yBarricade);
        int distance = AStar.AStarDistance(barricade, flagPosition, m_Grid, mapSize);

        // Distance=1, rewarded=7
        // Distance=8, rewarded=0
        // Distance=20, rewarded=-12

        if (distance != int.MaxValue)
            AddReward((8 - distance) * 0.5f);

        m_NumTurns--;
        if (m_NumTurns < 0)
            EndEpisode();
    }

    public override void OnEpisodeBegin()
    {
        m_Grid = GenerateMap();
        m_NumTurns = numTurnsBeforeReset;
    }

    private Vector2Int GetNearbyFlag()
    {
        for (int i = 0; i < 20; i++)
        {
            int x = flagPosition.x + Random.Range(-4, 5);
            int y = flagPosition.y + Random.Range(-4, 5);

            if (x < 0 || x >= mapSize.x || y < 0 || y >= mapSize.y)
                continue;

            int cell = m_Grid[CoordinateToIndex(x, y)];

            if (cell == (int)CellType.Walkable)
                return new(x, y);
        }

        return new(Random.Range(0, mapSize.x), Random.Range(0, mapSize.y));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // foreach (int cell in m_Grid)
        //     sensor.AddObservation(cell);
        // sensor.AddObservation(flagPosition.x);
        // sensor.AddObservation(flagPosition.y);

        Vector2Int pos = GetNearbyFlag();
        sensor.AddObservation(pos.x);
        sensor.AddObservation(pos.y);
    }

    private int ArgMax(float[] array)
    {
        float max = array[0];
        int maxIndex = 0;
        for (int i = 1; i < array.Length; i++)
        {
            if (array[i] > max)
            {
                max = array[i];
                maxIndex = i;
            }
        }
        return maxIndex;
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        // int xCoord = actions.DiscreteActions[0];
        // int yCoord = actions.DiscreteActions[1];
        // RecalculateRewards(xCoord, yCoord);
        // m_Grid[CoordinateToIndex(xCoord, yCoord)] = (int)CellType.Barricade;

        int index = ArgMax(actions.ContinuousActions.Array);
        Vector2Int coord = IndexToCoordinate(index);
        RecalculateRewards(coord.x, coord.y);
        m_Grid[CoordinateToIndex(coord.x, coord.y)] = (int)CellType.Barricade;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var action = actionsOut.DiscreteActions;
        action[0] = Random.Range(0, mapSize.x);
        action[1] = Random.Range(0, mapSize.y);
    }

    // Gizmos...

    private void OnDrawGizmos()
    {
        if (m_Grid == null)
            return;

        for (int i = 0; i < m_Grid.Length; i++)
        {
            Vector2Int coord = IndexToCoordinate(i);

            Vector3 center = new(coord.x, 0, coord.y);
            Vector3 size = new(0.7f, 0.1f, 0.7f);

            Color color = m_GizmosColors[m_Grid[i]];

            // if (m_Grid[i] == (int)CellType.Walkable)
            // {
            //     Vector2Int from = new(mapSize.x / 2, 2);
            //     int distance = AStar.AStarDistance(from, coord, m_Grid, mapSize);
            //     color.r = distance / distanceDivideFactor;
            // }

            Gizmos.color = color;
            Gizmos.DrawCube(center, size);
        }
    }

}
