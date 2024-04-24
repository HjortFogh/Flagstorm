using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class TurnbasedAgent : Agent
{
    private Piece m_Piece;

    private System.Action<Move> m_OnDone;
    private bool m_MakingMove = false;

    const int observationRadius = 2;

    void Awake()
    {
        if (!TryGetComponent(out m_Piece))
            gameObject.AddComponent<Piece>();
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("OnEpisodeBegin");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Piece position
        sensor.AddObservation(m_Piece.x);
        sensor.AddObservation(m_Piece.y);

        // Team flag position

        // Team-base position

        // Point of interest (e.g. EnemyFlag/Wood)

        // Frame-board 4x4 around agent
        // int[,] board = AiUtils.QueryBoard(m_Piece, observationRadius);

        // for (int i = 0; i < board.GetLength(0); i++)
        // {
        //     for (int j = 0; j < board.GetLength(1); j++)
        //     {
        //         sensor.AddObservation(board[i, j]);
        //     }
        // }

    }

    private int ArgMax(float[] array)
    {
        float max = array[0];
        int maxIndex = 0;
        for (int i = 1; i < array.Length; i++)
        {
            if (array[i] > max)
            {
                max = array[i];
                maxIndex = i;
            }
        }
        return maxIndex;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float[] strengths = new float[4];

        for (int i = 0; i < 4; i++)
            strengths[i] = actions.ContinuousActions[i];

        Cardinal cardinal = (Cardinal)ArgMax(strengths);
        Vector2Int? direction = Directions.CardinalToMove(cardinal);

        if (direction == null)
            return;

        m_MakingMove = false;
        m_OnDone(new Move(m_Piece, ((Vector2Int)direction).x, ((Vector2Int)direction).y));
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
