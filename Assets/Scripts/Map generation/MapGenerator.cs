using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class MapGenerator
{
    public enum Direction
    {
        North = 0, East, South, West
    };

    static Direction OppositeDirection(Direction dir) { return (Direction)(((int)dir + 2) % 4); }

    static List<TileRuleset> GrabEdge(TileRuleset tile, Direction dir)
    {
        return dir switch
        {
            Direction.North => tile.north,
            Direction.East => tile.east,
            Direction.South => tile.south,
            Direction.West => tile.west,
            _ => null,
        };
    }

    static bool Matches(TileRuleset from, TileRuleset to, Direction dir)
    {
        List<TileRuleset> fromEdge = GrabEdge(from, dir);
        List<TileRuleset> toEdge = GrabEdge(to, OppositeDirection(dir));

        return fromEdge.Contains(to) && toEdge.Contains(from);
    }

    class Cell
    {
        List<TileRuleset> options;
        public TileRuleset tile = null;

        public Cell(List<TileRuleset> _options)
        {
            options = _options;
        }

        public int Entropy()
        {
            if (tile != null) return 0;
            return options.Count;
        }

        public void CollapseAs(TileRuleset _tile)
        {
            tile = _tile;
        }

        public void Collapse()
        {
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

        public void Propagate(TileRuleset other, Direction dirFromThis)
        {
            options = options.Where(possible => Matches(possible, other, dirFromThis)).ToList();
        }
    }

    static Vector2Int? MinimalEntropy(Cell[,] grid)
    {

        int minEntropy = int.MaxValue;
        List<Vector2Int> minCoords = new();

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                int entropy = grid[i, j].Entropy();

                if (entropy == 0)
                    continue;

                if (entropy == minEntropy)
                    minCoords.Add(new Vector2Int(i, j));
                else if (entropy < minEntropy)
                {
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

    static void PropagateCell(int x, int y, Cell[,] grid)
    {
        int width = grid.GetLength(0), height = grid.GetLength(1);
        Cell updated = grid[x, y];

        if (y + 1 < height)
            grid[x, y + 1].Propagate(updated.tile, Direction.North);
        if (x - 1 >= 0)
            grid[x - 1, y].Propagate(updated.tile, Direction.East);
        if (y - 1 >= 0)
            grid[x, y - 1].Propagate(updated.tile, Direction.South);
        if (x + 1 < width)
            grid[x + 1, y].Propagate(updated.tile, Direction.West);
    }

    static void TryPopulateMap(GenerationConfig config, Cell[,] grid)
    {
        Texture2D source = config.maps[Random.Range(0, config.maps.Count)];
        Color[] pixels = source.GetPixels();

        if (pixels.GetLength(0) != config.width * config.height)
            return;

        for (int x = 0; x < config.width; x++)
        {
            for (int y = 0; y < config.width; y++)
            {
                int index = y * config.width + x;
                Color color = pixels[index];

                if (Mathf.Approximately(color.a, 0.0f))
                    continue;

                TileRuleset obj = ColorToTileRuleset.Find(config.mapper, color);

                if (obj == null)
                    continue;

                grid[x, y].CollapseAs(obj);
                PropagateCell(x, y, grid);
            }
        }
    }

    public static Map Generate(GenerationConfig config)
    {
        Cell[,] grid = new Cell[config.width, config.height];

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
                grid[i, j] = new Cell(new List<TileRuleset>(config.tileset));
        }

        TryPopulateMap(config, grid);

        for (int i = 0; i < config.width * config.height; i++)
        {
            Vector2Int? nullableCoord = MinimalEntropy(grid);
            if (nullableCoord == null)
                break;

            Vector2Int coord = (Vector2Int)nullableCoord;

            grid[coord.x, coord.y].Collapse();
            PropagateCell(coord.x, coord.y, grid);
        }

        TileRuleset[,] rulesets = new TileRuleset[config.width, config.height];
        for (int i = 0; i < config.width * config.height; i++)
        {
            int x = i / config.height, y = i % config.height;
            rulesets[x, y] = grid[x, y].tile;
        }

        return Map.Create(rulesets);
    }
}
