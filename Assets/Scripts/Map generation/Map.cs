using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class Map {
    int[,] grid;
    bool isInitialized = false;
    List<TileObject> tileset;

    public void Initialize(int width, int height, List<TileObject> _tileset) {
        grid = new int[width, height];

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
                grid[i, j] = -1;
        }

        tileset = _tileset;
        isInitialized = true;
    }

    public void Spawn() {
        if (!isInitialized)
            throw new Exception("Cannot spawn non-initalized map");
        
        int width = grid.GetLength(0), height = (grid.GetLength(1));

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                int tileIndex = grid[x, y];
                if (tileIndex == -1)
                    continue;

                GameObject prefab = tileset[tileIndex].prefab;
                Vector3 position = new(x, 0, y);

                GameObject.Instantiate(prefab, position, Quaternion.identity);
            }
        }
    }

    public void SetCell(Vector2Int coord, int index) {
        // Debug.Log("Set coord: " + coord.ToString() + " to value: " + index.ToString());
        grid[coord.x, coord.y] = index;
    }

    public bool IsWalkable(int x, int y) {
        if (x < 0 || x >= grid.GetLength(0) || y < 0 || y >= grid.GetLength(1))
            return false;

        int tileIndex = grid[x, y];

        if (tileIndex == -1 || tileIndex >= tileset.Count)
            return false;

        TileObject tile = tileset[tileIndex];

        Debug.Log(tileIndex);
        return false;
        

    }
}