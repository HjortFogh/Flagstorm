using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GenerationConfig generationConfig;

    public GameObject agentPrefab, playerPrefab;
    private readonly List<BaseTeam> m_Teams = new();

    private Map m_Map;
    private int m_CurrentTeamIndex = 0;

    bool ValidateMove(Move move, Map map)
    {
        int gx = move.x + move.piece.x, gy = move.y + move.piece.y;
        TileType type = map.AtCoord(gx, gy);

        if (type == null)
            return false;

        foreach (BaseTeam team in m_Teams)
        {
            if (team.BlocksCoord(gx, gy))
                return false;
        }

        return type.walkable;
    }

    void Start()
    {
        m_Map = MapGenerator.Generate(generationConfig);
        InitializeTeams();
    }

    void Update()
    {
        BaseTeam currentTeam = m_Teams[m_CurrentTeamIndex];

        Move? move = currentTeam.RequestMove();
        if (move != null && ValidateMove((Move)move, m_Map))
        {
            currentTeam.ApproveMove((Move)move);
            m_CurrentTeamIndex = (m_CurrentTeamIndex + 1) % m_Teams.Count;
        }
    }

    void InitializeTeams()
    {
        PlayerTeam playerTeam = new();
        playerTeam.InitializeTeam(6, playerPrefab);
        m_Teams.Add(playerTeam);

        MLAgentTeam mlAgentTeam = new();
        mlAgentTeam.InitializeTeam(6, agentPrefab);
        m_Teams.Add(mlAgentTeam);
    }
}