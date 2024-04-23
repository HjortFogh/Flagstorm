using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    public void MovePlayer(int direction)
    {
        switch (direction)
        {
            case 0:
                transform.position += Vector3.forward;
                break;
            case 1:
                transform.position += Vector3.right;
                break;
            case 2:
                transform.position += Vector3.back;
                break;
            case 3:
                transform.position += Vector3.left;
                break;
        }

    }

    public void MoveAI(int [,] board){
        // AI logic here
    }

}
