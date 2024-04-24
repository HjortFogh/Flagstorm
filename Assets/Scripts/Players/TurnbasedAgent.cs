using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;

public class TurnbasedAgent : Agent
{
    Piece m_Piece;

    Action<Move> OnDone;
    bool makingMove = false;

    void Awake()
    {
        if (!TryGetComponent(out m_Piece))
            gameObject.AddComponent<Piece>();
    }

    public override void OnEpisodeBegin()
    {
    }

    //tjek om der findes andre sensorer end vectorsensor
    public override void CollectObservations(VectorSensor sensor)
    {
        // // Flatten the multi-dimensional array to a 1D array
        // float[] flattenedArray = FlattenArray(multiDimensionalArray);

        // // Add the flattened array as observations to the sensor
        // foreach (float value in flattenedArray)
        // {
        //     sensor.AddObservation(value);
        // }


        // sensor.AddObservation(,);
        // sensor.addObservation(ABC.QueryBoard());
        sensor.AddObservation(transform.position);
    }
    /*
    0 = ikke walkable
    1 = friendly agent
    2 = unfriendly agent
    3 = enemy flag
    4 = friendly flag
    5 = barriers

    * = YOU.
    [0, 0, 1, 1, 1, 1, 3, 4, 5]
    */

    public override void OnActionReceived(ActionBuffers actions)
    {
        int x = (int)actions.ContinuousActions[0];
        int y = (int)actions.ContinuousActions[1];

        makingMove = false;
        OnDone(new Move(x, y));
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.ContinuousActions;
        actions[0] = Input.GetAxisRaw("Horizontal");
        actions[1] = Input.GetAxisRaw("Vertical");
    }

    public void SetCallback(Action<Move> callback)
    {
        OnDone = callback;
    }

    public void RequestMove()
    {
        if (!makingMove)
        {
            makingMove = true;
            RequestDecision();
        }
    }
}
