public class Map
{
    private TileType[,] m_Grid;

    public int Width { get => m_Grid.GetLength(0); }
    public int Height { get => m_Grid.GetLength(1); }

    public static Map Create(TileRuleset[,] rulesetGrid)
    {
        Map map = new();
        map.SetGrid(rulesetGrid);
        return map;
    }

    public void SetGrid(TileRuleset[,] rulesetGrid)
    {
        int width = rulesetGrid.GetLength(0), height = rulesetGrid.GetLength(1);
        m_Grid = new TileType[width, height];

        for (int i = 0; i < width * height; i++)
        {
            int x = i / height, y = i % height;
            m_Grid[x, y] = rulesetGrid[x, y].type;

        }

        for (int i = 0; i < width * height; i++)
        {
            int x = i / height, y = i % height;
            TileType[] surrounding = new TileType[4];

            if (y + 1 < height)
                surrounding[0] = m_Grid[x, y + 1];
            if (x + 1 < width)
                surrounding[1] = m_Grid[x + 1, y];
            if (x - 1 >= 0)
                surrounding[3] = m_Grid[x - 1, y];
            if (y - 1 >= 0)
                surrounding[2] = m_Grid[x, y - 1];

            rulesetGrid[x, y].PlaceInWorld(x, y, surrounding);
        }
    }

    public TileType AtCoord(int x, int y)
    {
        if (x < 0 || x >= m_Grid.GetLength(0) || y < 0 || y >= m_Grid.GetLength(1))
            return null;
        return m_Grid[x, y];
    }

    public bool IsWalkable(int x, int y)
    {
        if (x < 0 || x >= m_Grid.GetLength(0) || y < 0 || y >= m_Grid.GetLength(1))
            return false;
        return m_Grid[x, y].walkable;
    }
}