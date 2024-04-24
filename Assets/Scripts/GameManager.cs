using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GenerationConfig generationConfig;
    public List<TeamConfig> teamConfigs;

    private GameState m_GameState;

    void Start()
    {
        Map map = MapGenerator.Generate(generationConfig);
        m_GameState = new(map, teamConfigs);
    }

    void Update()
    {
        // AiUtils.GenerateThisFrameBoard(m_GameState);

        BaseTeam team = m_GameState.CurrentTeam;
        Move? nullableMove = team.RequestMove();

        if (nullableMove == null)
            return;

        Move move = (Move)nullableMove;

        if (!m_GameState.ValidateMove(move))
        {
            team.DeclineMove(move);
            return;
        }

        move.piece.Move(move.x, move.y);
        team.NextPlayer();
        m_GameState.NextTeam();
    }
}