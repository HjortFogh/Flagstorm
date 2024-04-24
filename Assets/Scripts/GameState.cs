using System.Collections.Generic;
using UnityEngine;

class GameState
{
    private readonly List<BaseTeam> m_Teams;
    private Map m_Map;
    private int m_CurrentTeamIndex = 0;

    public BaseTeam CurrentTeam { get { return m_Teams[m_CurrentTeamIndex]; } }
    public List<BaseTeam> Teams { get { return m_Teams; } }
    public Map Map { get { return m_Map; } }

    private List<Piece> m_Pieces = new(); // trees, flags, ...

    public GameState(Map map, List<TeamConfig> teamConfigs)
    {
        m_Map = map;
        m_Teams = new();

        // m_Pieces = new Piece[w, h];
        // map -> pieces

        foreach (TeamConfig config in teamConfigs)
        {
            BaseTeam newTeam = config.GetClass();
            newTeam.InitializeTeam(config.teamSize, config.prefab);
            m_Teams.Add(newTeam);
        }
    }

    public void CheckCollisions(Move move)
    {
        foreach (Piece piece in m_Pieces)
        {
            if (piece.x == move.piece.x + move.x && piece.y == move.piece.y + move.y)
            {
                //yay
            }

        }



    }

    public bool ValidateMove(Move move)
    {
        int gx = move.x + move.piece.x, gy = move.y + move.piece.y;
        TileType type = m_Map.AtCoord(gx, gy);

        if (type == null)
            return false;

        foreach (BaseTeam team in m_Teams)
        {
            if (team.BlocksCoord(gx, gy))
                return false;
        }

        return type.walkable;
    }

    public void NextTeam()
    {
        m_CurrentTeamIndex = (m_CurrentTeamIndex + 1) % m_Teams.Count;
    }
}