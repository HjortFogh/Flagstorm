using System;
using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    private enum CellType { NonWalkable, Walkable, Barricade }

    public static int AStarDistance(Vector2Int from, Vector2Int to, int[] grid, Vector2Int mapSize)
    {
        // Initialize open and closed lists
        HashSet<Vector2Int> openSet = new HashSet<Vector2Int>();
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

        // Add starting node to open set
        openSet.Add(from);

        // Initialize scores
        Dictionary<Vector2Int, int> gScore = new Dictionary<Vector2Int, int>();
        Dictionary<Vector2Int, int> fScore = new Dictionary<Vector2Int, int>();

        foreach (Vector2Int cell in GetGridPositions(mapSize))
        {
            gScore[cell] = int.MaxValue;
            fScore[cell] = int.MaxValue;
        }
        gScore[from] = 0;
        fScore[from] = HeuristicCostEstimate(from, to);

        while (openSet.Count > 0)
        {
            // Find node with lowest fScore in openSet
            Vector2Int current = FindLowestFScore(openSet, fScore);

            // If goal is reached, return distance
            if (current == to)
                return gScore[current];

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Vector2Int neighbor in GetNeighbors(current, mapSize))
            {
                if (closedSet.Contains(neighbor) || grid[GetIndex(neighbor, mapSize)] != (int)CellType.Walkable)
                    continue;

                int tentativeGScore = gScore[current] + 1;

                if (!openSet.Contains(neighbor))
                    openSet.Add(neighbor);
                else if (tentativeGScore >= gScore[neighbor])
                    continue;

                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, to);
            }
        }

        // If no path found, return a large value indicating unreachable
        return int.MaxValue;
    }

    private static int HeuristicCostEstimate(Vector2Int from, Vector2Int to)
    {
        return Mathf.Abs(to.x - from.x) + Mathf.Abs(to.y - from.y);
    }

    private static Vector2Int FindLowestFScore(HashSet<Vector2Int> set, Dictionary<Vector2Int, int> fScore)
    {
        Vector2Int lowestNode = Vector2Int.zero;
        int lowestFScore = int.MaxValue;

        foreach (Vector2Int node in set)
        {
            if (fScore[node] < lowestFScore)
            {
                lowestNode = node;
                lowestFScore = fScore[node];
            }
        }

        return lowestNode;
    }

    private static IEnumerable<Vector2Int> GetNeighbors(Vector2Int cell, Vector2Int mapSize)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        if (cell.x > 0)
            neighbors.Add(new Vector2Int(cell.x - 1, cell.y));
        if (cell.x < mapSize.x - 1)
            neighbors.Add(new Vector2Int(cell.x + 1, cell.y));
        if (cell.y > 0)
            neighbors.Add(new Vector2Int(cell.x, cell.y - 1));
        if (cell.y < mapSize.y - 1)
            neighbors.Add(new Vector2Int(cell.x, cell.y + 1));

        return neighbors;
    }

    private static IEnumerable<Vector2Int> GetGridPositions(Vector2Int mapSize)
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                yield return new Vector2Int(x, y);
            }
        }
    }

    private static int GetIndex(Vector2Int position, Vector2Int mapSize)
    {
        return position.x * mapSize.y + position.y;
    }
}
