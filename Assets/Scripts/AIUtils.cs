using System.Collections.Generic;

static class AiUtils
{
    static int[,] thisFrameBoard;

    public static int[,] QueryBoard()
    {
        return thisFrameBoard;
    }

    /*
    0 hvis ikke walkable,
    1 hvis walkable,
    2 hvis friendly agent,
    3 hvis enemy agent
    4 hvis friendly flag,
    5 hvis enemy flag
    */

    public static void GenerateThisFrameBoard(Map map, List<IBaseTeam> teams, int currentTeamIndex)
    {
        int width = map.GetGrid().GetLength(0);
        int height = map.GetGrid().GetLength(1);

        thisFrameBoard = new int[width, height];

        for (int i = 0; i < width * height; i++)
        {
            int x = i / height, y = i % height;
            thisFrameBoard[x, y] = map.GetGrid()[x, y].walkable ? 0 : 1;
        }

        

    }
}