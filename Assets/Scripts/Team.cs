using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    public int teamSize = 6;
    int m_CurrentPlayerIndex = 0;
    
    class Agent
    {
        public int x;
        public int y;
        public GameObject gameObject;

        public Agent(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public void Move(int direction)
        {
            switch (direction)
            {
                case 0:
                    y += 1;
                    break;
                case 1:
                    x += 1;
                    break;
                case 2:
                    y -= 1;
                    break;
                case 3:
                    x -= 1;
                    break;
            }
            gameObject.transform.position = new Vector3(x, 0.5f, y);
        }
    }

    private List<Agent> agents;

    public GameObject agentPrefab;

    public void Initialize()
    {
        agents = new List<Agent>();
        for (int i = 0; i < teamSize; i++)
        {
            GameObject agentEntity = Instantiate(agentPrefab, new Vector3(i, 0.5f, (int)transform.position.z), Quaternion.identity);
            agents.Add(new Agent(i, (int)transform.position.z) { gameObject = agentEntity });

            // if (i == 0)
            // {
            //     Transform cubeTransform = agentEntity.transform.Find("Cube");

            //     if (cubeTransform != null)
            //     {
            //         Renderer cubeRenderer = cubeTransform.GetComponent<Renderer>();

            //         if (cubeRenderer != null)
            //         {
            //             cubeRenderer.material.color = Color.red;
            //         }
            //     }

            // }

        }

    }

    public void MovePlayer(int direction)
    {
        agents[m_CurrentPlayerIndex].Move(direction);
        m_CurrentPlayerIndex = (m_CurrentPlayerIndex + 1) % teamSize;
    }

    public int RequestMove()
    {
        if (Input.GetKeyDown(KeyCode.W)) { return 0; }
        else if (Input.GetKeyDown(KeyCode.A)) { return 1; }
        else if (Input.GetKeyDown(KeyCode.D)) { return 3; }
        else if (Input.GetKeyDown(KeyCode.S)) { return 2; }
        return -1;
    }
}
