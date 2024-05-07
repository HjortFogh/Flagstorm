using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main class to handle the game loop
/// </summary>
public class GameManager : MonoBehaviour
{
    public GenerationConfig generationConfig;
    public List<TeamConfig> teamConfigs;
    public Unity.Barracuda.NNModel agentBrain;

    public GameObject SpotlightPrefab;
    public float inputDelayTime = 0.8f;
    private float m_CurrentTime = 0.0f;

    private GameState m_GameState;

    /// <summary>
    /// Generates the map and initializes the game state
    /// </summary>
    private void Start()
    {
        Map map = MapGenerator.Generate(generationConfig);
        m_GameState = new(map, teamConfigs);
        m_GameState.SetAgentBrain(agentBrain);
    }

    /// <summary>
    /// Game loop
    /// </summary>
    private void Update()
    {
        m_CurrentTime += Time.deltaTime;

        if (m_CurrentTime < inputDelayTime)
            return;

        // Get the current team
        BaseTeam team = m_GameState.CurrentTeam;

        // Request a move from the team
        Move? nullableMove = team.RequestMove();

        // If the move is null, team has not made a move, return
        if (nullableMove == null)
            return;

        Move move = (Move)nullableMove;

        // CHeck if the future state of the agent is valid. It's valid if the agent moves to a piece where it can interact with it
        (bool canInteract, Piece piece) = m_GameState.CheckCollisions(move);

        // If the move is invalid, decline the move or if it can't interact with the piece, decline the move
        if (!canInteract || !m_GameState.ValidateMove(move))
        {
            team.DeclineMove(move);
            return;
        }

        // Move the piece
        move.piece.Move(move.x, move.y);

        // Interact with the piece if it exists
        if (piece != null)
            move.piece.Interact(piece);

        // Check if the game is won
        m_GameState.CheckWin();

        // Move to the next player
        team.NextPlayer();

        // Move to the next team
        m_GameState.NextTeam();

        // Move the spotlight to the current piece
        Vector3 piecePosition = m_GameState.CurrentTeam.CurrentPiece.transform.position;
        SpotlightPrefab.transform.position = piecePosition;

        m_CurrentTime = 0f;
    }
}