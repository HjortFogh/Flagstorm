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
        BaseTeam team = m_GameState.CurrentTeam;
        Move? nullableMove = team.RequestMove();

        if (nullableMove == null)
            return;

        Move move = (Move)nullableMove;

        (bool canInteract, Piece piece) = m_GameState.CheckCollisions(move);

        if (!canInteract || !m_GameState.ValidateMove(move))
        {
            team.DeclineMove(move);
            return;
        }

        move.piece.Move(move.x, move.y);

        if (piece != null)
            move.piece.Interact(piece);

        team.NextPlayer();
        m_GameState.NextTeam();
    }
}