using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;


public class PoIAgent : Agent
{

    public MapTrain map;
    [SerializeField] private GameObject PointOfInterest;

    public override void OnEpisodeBegin()
    {
        Vector2Int playerPos = map.GetRandomWalkablePosition();
        transform.localPosition = new Vector3(playerPos.x, 0.5f, playerPos.y);
        Vector2Int PoIPos = map.GetRandomWalkablePosition();
        PointOfInterest.transform.localPosition = new Vector3(PoIPos.x, 0.5f, PoIPos.y);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector2Int playerPos = new((int)transform.localPosition.x, (int)transform.localPosition.z);
        Vector2Int PoIPos = new((int)PointOfInterest.transform.localPosition.x, (int)PointOfInterest.transform.localPosition.z);

        sensor.AddObservation(playerPos);
        sensor.AddObservation(PoIPos);

        // bool[] walkableDirections = map.GetWalkableDirections(playerPos);
        // foreach (bool walkable in walkableDirections)
        // {
        //     sensor.AddObservation(walkable);
        // }

        for (int xOffset = -2; xOffset <= 2; xOffset++)
        {
            for (int yOffset = -2; yOffset <= 2; yOffset++)
            {
                Vector2Int position = playerPos + new Vector2Int(xOffset, yOffset);
                bool walkable = map.IsWalkable(position);
                sensor.AddObservation(walkable ? 1f : 0f);
            }
        }

        // //give the entire map as observation
        // for (int x = 0; x < map.width; x++)
        // {
        //     for (int y = 0; y < map.height; y++)
        //     {
        //         sensor.AddObservation(map.mapArray[x, y]);
        //     }
        // }


    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        MoveAgent(actions.DiscreteActions[0]);
        CheckCollisions();

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        if (Input.GetKey(KeyCode.W))
        {
            discreteActions[0] = 0;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            discreteActions[0] = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActions[0] = 2;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActions[0] = 3;
        }
        else
        {
            discreteActions[0] = 4;
        }
    }

    public void MoveAgent(int direction)
    {
        Vector2Int dir = Vector2Int.zero;

        switch (direction)
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

        transform.localPosition += new Vector3(dir.x, 0, dir.y);

        if (!map.IsWalkable(new Vector2Int((int)transform.localPosition.x, (int)transform.localPosition.z)))
        {
            AddReward(-0.5f);
            EndEpisode();
        }

        // if (map.IsWalkable(newPos))
        // {
        //     transform.localPosition = new Vector3(newPos.x, 0, newPos.y);
        // }

    }

    private void CheckCollisions()
    {
        if (transform.localPosition == PointOfInterest.transform.localPosition)
        {
            AddReward(1.0f);
            EndEpisode();
        }
    }


}