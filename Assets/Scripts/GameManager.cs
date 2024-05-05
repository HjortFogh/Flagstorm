using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GenerationConfig generationConfig;
    public List<TeamConfig> teamConfigs;
    public Unity.Barracuda.NNModel agentBrain;

    public GameObject SpotlightPrefab;
    public float inputDelayTime = 0.8f;
    private float m_CurrentTime = 0.0f;

    private GameState m_GameState;

    private void Start()
    {
        Map map = MapGenerator.Generate(generationConfig);
        m_GameState = new(map, teamConfigs);
        m_GameState.SetAgentBrain(agentBrain);
    }

    private void Update()
    {
        m_CurrentTime += Time.deltaTime;

        if (m_CurrentTime < inputDelayTime)
            return;

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

        m_GameState.CheckWin();
        team.NextPlayer();
        m_GameState.NextTeam();

        Vector3 piecePosition = m_GameState.CurrentTeam.CurrentPiece.transform.position;
        SpotlightPrefab.transform.position = piecePosition;

        m_CurrentTime = 0f;
    }
}