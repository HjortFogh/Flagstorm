using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class MapGenerator
{
    static List<TileRuleset> GrabEdge(TileRuleset tile, Cardinal dir)
    {
        return dir switch
        {
            Cardinal.East => tile.east,
            Cardinal.South => tile.south,
            Cardinal.West => tile.west,
            _ => tile.north,
        };
    }

    static bool Matches(TileRuleset from, TileRuleset to, Cardinal dir)
    {
        List<TileRuleset> fromEdge = GrabEdge(from, dir);
        List<TileRuleset> toEdge = GrabEdge(to, Directions.OppositeCardinal(dir));

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

        public void Propagate(TileRuleset other, Cardinal dirFromThis)
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
            grid[x, y + 1].Propagate(updated.tile, Cardinal.North);
        if (x - 1 >= 0)
            grid[x - 1, y].Propagate(updated.tile, Cardinal.East);
        if (y - 1 >= 0)
            grid[x, y - 1].Propagate(updated.tile, Cardinal.South);
        if (x + 1 < width)
            grid[x + 1, y].Propagate(updated.tile, Cardinal.West);
    }

    private static void TryPopulateMap(GenerationConfig config, Cell[,] grid)
    {
        Texture2D generationTexture = config.PickTexture();

        if (generationTexture == null ||
            generationTexture.width != config.width ||
            generationTexture.height != config.height)
            return;

        Color[] pixels = generationTexture.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            int x = i % config.width, y = i / config.width;
            TileRuleset tileType = config.PixelToRuleset(pixels[i]);

            if (tileType == null)
                continue;

            grid[x, y].CollapseAs(tileType);
            PropagateCell(x, y, grid);
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
