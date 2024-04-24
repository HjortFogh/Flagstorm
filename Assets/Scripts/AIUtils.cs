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
    4 hvis point of interest (modstander flag/træ),
    5 hvis my flag i startpos,
    6 hvis my base,
    */

    // public static void GenerateThisFrameBoard(Map map, List<BaseTeam> teams, Piece piece)
    // {

    // }

    public static void GenerateThisFrameBoard(Map map, List<BaseTeam> teams, int currentTeamIndex)
    {
        int width = map.Width;
        int height = map.Height;

        thisFrameBoard = new int[width, height];

        for (int i = 0; i < width * height; i++)
        {
            int x = i / height, y = i % height;
            thisFrameBoard[x, y] = map.AtCoord(x, y).walkable ? 0 : 1;
        }
    }
}