using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public static GameState Instance;

    private readonly List<BaseTeam> m_Teams;
    public Unity.Barracuda.NNModel m_AgentBrain;
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

        foreach (TeamConfig config in teamConfigs)
        {
            GameObject flagObject = new();
            FlagPiece flagPiece = flagObject.AddComponent<FlagPiece>();

            flagPiece.Teleport(config.flagPosition.x, config.flagPosition.y);

            m_Pieces.Add(flagPiece);

            BaseTeam newTeam = config.GetClass();
            newTeam.InitializeTeam(config.teamSize, config, flagPiece);
            m_Teams.Add(newTeam);

            foreach (Piece piece in newTeam.Pieces)
                m_Pieces.Add(piece);
        }

        ((MLAgentTeam)m_Teams[0]).SetPoI(new Vector2Int(teamConfigs[1].flagPosition.x, teamConfigs[1].flagPosition.y));
        //((MLAgentTeam)m_Teams[1]).SetPoI(new Vector2Int(teamConfigs[0].flagPosition.x, teamConfigs[0].flagPosition.y));

        Instance = this;
    }

    public (bool, Piece) CheckCollisions(Move move)
    {
        Piece newPiece = null;
        foreach (Piece piece in m_Pieces)
        {
            if (piece.X == move.piece.X + move.x && piece.Y == move.piece.Y + move.y)
            {
                if (!move.piece.CanInteract(piece))
                    return (false, piece);
                else
                    newPiece = piece;
                break;
            }
        }
        return (true, newPiece);
    }

    public void CheckWin()
    {
        if (m_Teams[0].BlocksCoord(m_Teams[1].teamConfig.flagPosition.x, m_Teams[1].teamConfig.flagPosition.y))
        {
            Debug.Log("Team 1 wins!");
            Application.Quit();
        }
        else if (m_Teams[1].BlocksCoord(m_Teams[0].teamConfig.flagPosition.x, m_Teams[0].teamConfig.flagPosition.y))
        {
            Debug.Log("Team 2 wins!");
            Application.Quit();
        }
    }

    public bool ValidateMove(Move move)
    {
        int gx = move.x + move.piece.X, gy = move.y + move.piece.Y;
        TileType type = m_Map.AtCoord(gx, gy);

        if (type == null)
            return false;

        return type.walkable;
    }

    public void NextTeam()
    {
        m_CurrentTeamIndex = (m_CurrentTeamIndex + 1) % m_Teams.Count;
    }

    public void SetAgentBrain(Unity.Barracuda.NNModel brain)
    {
        m_AgentBrain = brain;
    }
}