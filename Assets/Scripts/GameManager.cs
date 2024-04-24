using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GenerationConfig generationConfig;
    private Map m_Map;
    private List<IBaseTeam> m_Teams = new List<IBaseTeam>();
    private int m_CurrentTeamIndex = 0;

    void Awake()
    {
        m_Map = MapGenerator.Generate(generationConfig);
    }

    void Update()
    {
        // IBaseTeam currentTeam = m_Teams[m_CurrentTeamIndex];

        // if (currentTeam is MLAgentTeam)
        //     AiUtils.GenerateThisFrameBoard(m_Map, m_Teams, m_CurrentTeamIndex);
        
        // Move? move = currentTeam.RequestMove();
        // if (move != null)
        // {
        //     // currentTeam.MakeMove(move.Value);
        //     m_CurrentTeamIndex = (m_CurrentTeamIndex + 1) % m_Teams.Count;
        // }

    }

    void InitializeTeams(/* maybe some parameters */)
    {
        PlayerTeam playerTeam = new PlayerTeam();
        playerTeam.InitializeTeam();
        m_Teams.Add(playerTeam);

        MLAgentTeam mlAgentTeam = new MLAgentTeam();
        mlAgentTeam.InitializeTeam();
        m_Teams.Add(mlAgentTeam);
    }


}