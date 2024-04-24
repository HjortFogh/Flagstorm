public class Map
{
    TileType[,] grid;

    public int Width { get => grid.GetLength(0); }
    public int Height { get => grid.GetLength(1); }

    public static Map Create(TileRuleset[,] rulesetGrid)
    {
        Map map = new();
        map.SetGrid(rulesetGrid);
        return map;
    }

    public void SetGrid(TileRuleset[,] rulesetGrid)
    {
        int width = rulesetGrid.GetLength(0), height = rulesetGrid.GetLength(1);
        grid = new TileType[width, height];

        for (int i = 0; i < width * height; i++)
        {
            int x = i / height, y = i % height;
            grid[x, y] = rulesetGrid[x, y].type;
            grid[x, y].PlaceInWorld(x, y);
        }
    }

    // public TileType[,] GetGrid() { return grid; }

    public TileType AtCoord(int x, int y)
    {
        if (x < 0 || x >= grid.GetLength(0) || y < 0 || y >= grid.GetLength(1))
            return null;
        return grid[x, y];
    }

    public bool IsWalkable(int x, int y)
    {
        if (x < 0 || x >= grid.GetLength(0) || y < 0 || y >= grid.GetLength(1))
            return false;
        return grid[x, y].walkable;
    }
}