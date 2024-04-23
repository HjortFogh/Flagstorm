using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapGenerator
{
    public enum Direction {
        North = 0, East, South, West
    };

    static Direction OppositeDirection(Direction dir) { return (Direction)(((int)dir + 2) % 4); }

    static List<TileObject> GrabEdge(TileObject tile, Direction dir) {
        switch(dir) 
        {
        case Direction.North:
            return tile.north;
        case Direction.East:
            return tile.east;
        case Direction.South:
            return tile.south;
        case Direction.West:
            return tile.west;
        }
        return null;
    }

    static bool Matches(TileObject from, TileObject to, Direction dir) {
        List<TileObject> fromEdge = GrabEdge(from, dir);
        List<TileObject> toEdge = GrabEdge(to, OppositeDirection(dir));

        return fromEdge.Contains(to) && toEdge.Contains(from);
    }

    class Cell {
        List<TileObject> options;
        public TileObject tile = null;
        
        public Cell(List<TileObject> _options) {
            options = _options;
        }

        public int Entropy() {
            if (tile != null) return 0;
            return options.Count;
        }

        public void CollapseAs(TileObject _tile) {
            tile = _tile;
        }

        public void Collapse() {
            if (Entropy() == 0)
                return;

            if (Entropy() == 1)
            {
                tile = options[0];
                return;
            }

            int totalWeight = options.Sum(obj => obj.weight);
            int weightTarget = Random.Range(0, totalWeight + 1);

            tile = options.First(obj =>
            {
                weightTarget -= obj.weight;
                return weightTarget <= 0;
            });
        }

        public void Propagate(TileObject other, Direction dirFromThis) {
            options = options.Where(possible => Matches(possible, other, dirFromThis)).ToList();
        }
    }

    static Vector2Int? MinimalEntropy(Cell[,] grid) {

        int minEntropy = int.MaxValue;
        List<Vector2Int> minCoords = new();

        for (int i = 0; i < grid.GetLength(0); i++) {
            for (int j = 0; j < grid.GetLength(1); j++) {
                int entropy = grid[i, j].Entropy();

                if (entropy == 0)
                    continue;

                if (entropy == minEntropy)
                    minCoords.Add(new Vector2Int(i, j));
                else if (entropy < minEntropy) {
                    minCoords.Clear();
                    minCoords.Add(new Vector2Int(i, j));
                    minEntropy = entropy;
                }
            }
        }

        if (minCoords.Count == 0)
            return null;

        return minCoords[Random.Range(0, minCoords.Count)];
    }

    static void PopulateMap(GenerationConfig config, Cell[,] grid, Map map) {
        Texture2D source = config.maps[Random.Range(0, config.maps.Count)];
        Color[] pixels = source.GetPixels();

        if (pixels.GetLength(0) != config.width * config.height)
            return;

        for (int x = 0; x < config.width; x++) {
            for (int y = 0; y < config.width; y++) {
                int index = y * config.width + x;
                Color color = pixels[index];

                if (Mathf.Approximately(color.a, 0.0f))
                    continue;
                
                TileObject obj = ColorToTileObject.Find(config.mapper, color);
                
                if (obj == null)
                    continue;

                grid[x, y].CollapseAs(obj);

                if (y + 1 < config.height)
                    grid[x, y + 1].Propagate(obj, Direction.North);
                if (x - 1 >= 0)
                    grid[x - 1, y].Propagate(obj, Direction.East);
                if (y - 1 >= 0)
                    grid[x, y - 1].Propagate(obj, Direction.South);
                if (x + 1 < config.width)
                    grid[x + 1, y].Propagate(obj, Direction.West);

                map.SetCell(new Vector2Int(x, y), config.tileset.IndexOf(obj));
            }
        }
    }

    public static Map Generate(GenerationConfig config) {
        Map map = new();
        map.Initialize(config.width, config.height, config.tileset);

        Cell[,] grid = new Cell[config.width, config.height];

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
                grid[i, j] = new Cell(new List<TileObject>(config.tileset));
        }

        PopulateMap(config, grid, map);

        for (int i = 0; i < config.width * config.height; i++) {
            Vector2Int? nullableCoord = MinimalEntropy(grid);
            if (nullableCoord == null)
                break;
            
            Vector2Int coord = (Vector2Int)nullableCoord;

            grid[coord.x, coord.y].Collapse();
            TileObject other = grid[coord.x, coord.y].tile;
            
            if (coord.y + 1 < config.height)
                grid[coord.x, coord.y + 1].Propagate(other, Direction.North);
            if (coord.x - 1 >= 0)
                grid[coord.x - 1, coord.y].Propagate(other, Direction.East);
            if (coord.y - 1 >= 0)
                grid[coord.x, coord.y - 1].Propagate(other, Direction.South);
            if (coord.x + 1 < config.width)
                grid[coord.x + 1, coord.y].Propagate(other, Direction.West);

            map.SetCell(coord, config.tileset.IndexOf(other));
        }
        
        return map;
    }
}
