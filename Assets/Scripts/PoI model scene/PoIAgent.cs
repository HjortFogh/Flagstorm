using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;

public class PoIAgent : Agent
{
    void Update()
    {
        AddReward(transform.position.x);
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

    public override void OnEpisodeBegin()
    {
        transform.position = Vector3.zero;
        // transform.position = new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f));

        // Debug.Log("OnEpisodeBegin");
        //hver episode ram nyt sted til ens base :)
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position.x);
        // sensor.AddObservation(transform.position.z);
        // sensor.AddObservation(0);
        // sensor.AddObservation(0);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float dir = actions.ContinuousActions[0];
        transform.position += new Vector3(dir, 0f, 0f);

        // float[] strengths = new float[4];

        // for (int i = 0; i < 4; i++)
        //     strengths[i] = actions.ContinuousActions[i];

        // Cardinal cardinal = (Cardinal)ArgMax(strengths);
        // Vector2Int? direction = Directions.CardinalToMove(cardinal);

        // if (direction is Vector2Int vec)
        //     transform.position += new Vector3(vec.x, 0, vec.y);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // var actions = actionsOut.ContinuousActions;
        // Debug.Log("Heuristic");
    }
}
