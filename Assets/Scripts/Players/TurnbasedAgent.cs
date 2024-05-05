using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using Unity.Barracuda;
using Unity.Barracuda.ONNX;
using System.IO;

/// <summary>
/// This class is responsible for the agent's behavior in the game.
/// </summary>
public class TurnbasedAgent : Agent
{
    private MovablePiece m_Piece;

    private System.Action<Move> m_OnDone;
    private bool m_MakingMove = false;

    public int observationRadius = 2;
    public Vector2Int pointOfInterest = Vector2Int.zero;


    void Awake()
    {
        // Add MovablePiece component if it doesn't exist
        if (!TryGetComponent(out m_Piece))
            gameObject.AddComponent<MovablePiece>();

        /// Set the agent's brain parameters
        BehaviorParameters bh = gameObject.GetComponent<BehaviorParameters>();
        bh.BrainParameters.VectorObservationSize = 29;
        bh.BrainParameters.ActionSpec = ActionSpec.MakeDiscrete(new int[] { 4 });
        bh.BehaviorName = "TurnbasedAgent";

        /// Set the agent's brain model
        bh.Model = GameState.Instance.m_AgentBrain;
    }

    /// <summary>
    /// Called when episode begins.
    /// </summary>
    public override void OnEpisodeBegin()
    {
        Debug.Log("OnEpisodeBegin");
    }

    /// <summary>
    /// Collects the agent's observations.
    /// </summary>
    /// <param name="sensor"></param>
    public override void CollectObservations(VectorSensor sensor)
    {
        // Piece position
        sensor.AddObservation(m_Piece.X);
        sensor.AddObservation(m_Piece.Y);


        // Point of interest position
        sensor.AddObservation(pointOfInterest.x);
        sensor.AddObservation(pointOfInterest.y);

        // Observe what is walkable around the agent
        for (int xOffset = -2; xOffset <= 2; xOffset++)
        {
            for (int yOffset = -2; yOffset <= 2; yOffset++)
            {
                Vector2Int position = new Vector2Int(m_Piece.X, m_Piece.Y) + new Vector2Int(xOffset, yOffset);
                bool walkable = GameState.Instance.Map.IsWalkable(position.x, position.y);
                bool coordBlocked = m_Piece.Team.BlocksCoord(position.x, position.y);
                // 0 = non-walkable, 1 = walkable
                sensor.AddObservation((!coordBlocked && walkable) ? 1f : 0f);
            }
        }
    }

    /// <summary>
    /// Called when the agent receives an action.
    /// </summary>
    /// <param name="actions"></param>
    public override void OnActionReceived(ActionBuffers actions)
    {

        Vector2Int dir = Vector2Int.zero;

        /// Convert the action to a direction
        switch (actions.DiscreteActions[0])
        {
            case 0:
                dir += Vector2Int.up;
                break;
            case 1:
                dir += Vector2Int.right;
                break;
            case 2:
                dir += Vector2Int.down;
                break;
            case 3:
                dir += Vector2Int.left;
                break;
            case 4:
                return;
        }

        m_MakingMove = false;
        m_OnDone(new Move(m_Piece, dir.x, dir.y));
    }

    /// <summary>
    /// Called when the agent requests a decision.
    /// </summary>
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.ContinuousActions;
        actions[0] = Random.value;
        actions[1] = Random.value;
        actions[2] = Random.value;
        actions[3] = Random.value;
    }

    /// <summary>
    /// Sets the callback for when the agent is done making a move.
    /// </summary>
    public void SetCallback(System.Action<Move> callback)
    {
        m_OnDone = callback;
    }

    /// <summary>
    /// Requests the agent to make a move.
    /// </summary>
    public void RequestMove()
    {
        if (!m_MakingMove)
        {
            m_MakingMove = true;
            RequestDecision();
        }
    }
}
