using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GenerationConfig generationConfig;
    // private Map m_Map;

    // public Team redTeam;
    // public Team blueTeam;

    // private int m_CurrentTeamIndex = 0;

    void Awake()
    {
        MapGenerator.Generate(generationConfig);
        // redTeam.Initialize();
        // blueTeam.Initialize();
    }

    // void Update()
    // {
    //     if (m_CurrentTeamIndex == 0)
    //     {
    //         int move = redTeam.RequestMove();
    //         if (move != -1)
    //         {
    //             redTeam.MovePlayer(move);
    //             m_CurrentTeamIndex = (m_CurrentTeamIndex + 1) % 2;

    //         }

    //     }
    //     else
    //     {
    //         int move = blueTeam.RequestMove();
    //         if (move != -1)
    //         {
    //             blueTeam.MovePlayer(move);
    //             m_CurrentTeamIndex = (m_CurrentTeamIndex + 1) % 2;
    //         }

    //     }

    // }
}
