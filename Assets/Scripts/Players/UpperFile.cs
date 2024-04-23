using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class UpperFile : MonoBehaviour
{
    public TurnbasedAgent agent;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            agent.RequestDecision();
    }
}