using UnityEngine;

public class MapTrain : MonoBehaviour
{
    public int[,] mapArray;

    public int width = 10;
    public int height = 10;

    private void Awake()
    {
        mapArray = new int[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (i == 0 || i == width - 1 || j == 0 || j == height - 1)
                {
                    mapArray[i, j] = 0;
                }
                else
                {
                    mapArray[i, j] = 1;
                }
            }
        }

        for (int i = 0; i < 200; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            mapArray[x, y] = 0;
        }
    }

    public bool IsWalkable(Vector2Int coord)
    {
        if (coord.x < 0 || coord.x >= width || coord.y < 0 || coord.y >= height)
        {
            return false;
        }

        return mapArray[coord.x, coord.y] == 1;
    }

    public bool[] GetWalkableDirections(Vector2Int coord)
    {
        bool[] walkableDirections = new bool[4];

        walkableDirections[0] = IsWalkable(coord + Vector2Int.up);
        walkableDirections[1] = IsWalkable(coord + Vector2Int.right);
        walkableDirections[2] = IsWalkable(coord + Vector2Int.down);
        walkableDirections[3] = IsWalkable(coord + Vector2Int.left);

        return walkableDirections;
    }

    public Vector2Int GetRandomWalkablePosition()
    {
        Vector2Int randomPosition = new Vector2Int(Random.Range(0, width), Random.Range(0, height));

        while (!IsWalkable(randomPosition) || randomPosition.x == 1 || randomPosition.x == width - 1 || randomPosition.y == 1 || randomPosition.y == height - 1)
        {
            randomPosition = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
        }

        return randomPosition;
    }
}
