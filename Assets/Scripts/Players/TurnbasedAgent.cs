using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;

public class TurnbasedAgent : Agent
{
    private MovablePiece m_Piece;

    private System.Action<Move> m_OnDone;
    private bool m_MakingMove = false;

    public int observationRadius = 2;

    void Awake()
    {
        if (!TryGetComponent(out m_Piece))
            gameObject.AddComponent<MovablePiece>();

        BehaviorParameters bh = gameObject.GetComponent<BehaviorParameters>();
        bh.BrainParameters.VectorObservationSize = 23;

        bh.BrainParameters.ActionSpec = new ActionSpec { NumContinuousActions = 4 };

        bh.BehaviorName = "TurnbasedAgent";
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

        // Team flag position
        sensor.AddObservation(m_Piece.Team.teamFlag.X);
        sensor.AddObservation(m_Piece.Team.teamFlag.Y);
        sensor.AddObservation(m_Piece.Team.teamFlag.AtHome);

        // Team-base position
        sensor.AddObservation(m_Piece.Team.spawnPoint.x);
        sensor.AddObservation(m_Piece.Team.spawnPoint.y);

        // Closest point of interest (e.g. EnemyFlag/Wood)
        // sensor.AddObservation(ClosestPointOfInterest(this));
        sensor.AddObservation(14);
        sensor.AddObservation(22);

        // Walkable information
        for (int dx = -observationRadius; dx <= observationRadius; dx++)
        {
            for (int dy = -observationRadius; dy <= observationRadius; dy++)
            {
                int x = m_Piece.X + dx;
                int y = m_Piece.Y + dy;

                sensor.AddObservation(GameState.Instance.Map.IsWalkable(x, y) ? 1 : 0);

                // if (x >= 0 && x < 28 && y >= 0 && y < 28)
                // {
                //     // sensor.AddObservation(GameState.Instance.); ...
                // }
                // else
                // {
                //     sensor.AddObservation(0);
                // }
            }
        }
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

    Vector2 MapVector3ToVector2(Vector3 v)
    {
        return new Vector2(v.x, v.z);
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
