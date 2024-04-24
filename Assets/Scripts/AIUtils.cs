using System;
using System.Collections.Generic;
using UnityEngine;

static class AiUtils
{
    static int[,] thisFrameBoard;

    public static int[,] QueryBoard(Piece piece = null, int radius = 2)
    {
        if (thisFrameBoard == null)
        {
            throw new Exception("Frameboard not set");
        }

        int width = thisFrameBoard.GetLength(0);

        int sx = Mathf.Max(0, piece.x - radius);
        int sy = Mathf.Max(0, piece.y - radius);

        int ex = Mathf.Min(width - 1, piece.x + radius);
        int ey = Mathf.Min(width - 1, piece.y + radius);

        int[,] specializedBoard = new int[ex - sx + 1, ey - sy + 1];


        for (int i = sx; i <= ex; i++)
        {
            for (int j = sy; j <= ey; j++)
            {
                specializedBoard[i - sx, j - sy] = thisFrameBoard[i, j];
            }

        }

        return specializedBoard;

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

    // public static void GenerateThisFrameBoard(Map map, List<BaseTeam> teams, int currentTeamIndex)
    public static void GenerateThisFrameBoard(GameState state)
    {
        int width = state.Map.Width;
        int height = state.Map.Height;

        thisFrameBoard = new int[width, height];

        for (int i = 0; i < width * height; i++)
        {
            int x = i / height, y = i % height;
            thisFrameBoard[x, y] = state.Map.AtCoord(x, y).walkable ? 0 : 1;
        }
    }

    // public static void GenerateThisFrameBoard(Map map, List<BaseTeam> team)
    // {

    // }

    public static void CollectFlagObservations(Map map, List<BaseTeam> team)
    {

    }
}