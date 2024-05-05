using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using Unity.Barracuda;
using Unity.Barracuda.ONNX;
using System.IO;

public class TurnbasedAgent : Agent
{
    private MovablePiece m_Piece;

    private System.Action<Move> m_OnDone;
    private bool m_MakingMove = false;

    public int observationRadius = 2;
    public Vector2Int pointOfInterest = Vector2Int.zero;


    void Awake()
    {
        if (!TryGetComponent(out m_Piece))
            gameObject.AddComponent<MovablePiece>();

        BehaviorParameters bh = gameObject.GetComponent<BehaviorParameters>();
        bh.BrainParameters.VectorObservationSize = 29;
        bh.BrainParameters.ActionSpec = ActionSpec.MakeDiscrete(new int[] { 4 });
        bh.BehaviorName = "TurnbasedAgent";

        bh.Model = GameState.Instance.m_AgentBrain;
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("OnEpisodeBegin");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Piece position
        sensor.AddObservation(m_Piece.X);
        sensor.AddObservation(m_Piece.Y);


        // Point of interest position
        sensor.AddObservation(pointOfInterest.x);
        sensor.AddObservation(pointOfInterest.y);

        for (int xOffset = -2; xOffset <= 2; xOffset++)
        {
            for (int yOffset = -2; yOffset <= 2; yOffset++)
            {
                Vector2Int position = new Vector2Int(m_Piece.X, m_Piece.Y) + new Vector2Int(xOffset, yOffset);
                bool walkable = GameState.Instance.Map.IsWalkable(position.x, position.y);
                bool canInteract = GameState.Instance.CheckCollisions(new Move(m_Piece, xOffset, yOffset)).Item1;
                sensor.AddObservation(canInteract && walkable ? 1f : 0f);
            }
        }
    }



    public override void OnActionReceived(ActionBuffers actions)
    {

        Vector2Int dir = Vector2Int.zero;

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

        // float[] strengths = new float[4];

        // for (int i = 0; i < 4; i++)
        //     strengths[i] = actions.ContinuousActions[i];

        // Cardinal cardinal = (Cardinal)ArgMax(strengths);
        // Vector2Int? direction = Directions.CardinalToMove(cardinal);

        // if (direction == null)
        //     return;

        // m_MakingMove = false;
        // m_OnDone(new Move(m_Piece, ((Vector2Int)direction).x, ((Vector2Int)direction).y));
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.ContinuousActions;
        actions[0] = Random.value;
        actions[1] = Random.value;
        actions[2] = Random.value;
        actions[3] = Random.value;
    }

    public void SetCallback(System.Action<Move> callback)
    {
        m_OnDone = callback;
    }

    public void RequestMove()
    {
        if (!m_MakingMove)
        {
            m_MakingMove = true;
            RequestDecision();
        }
    }

}
