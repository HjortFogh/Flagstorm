using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Piece
{
    // ...
};

public struct Move
{
    int x, y;
    Piece piece;
}

public interface IBaseTeam
{
    public void InitializeTeam();
    public Move? RequestMove();
}

public class PlayerTeam : IBaseTeam
{
    public void InitializeTeam()
    {
        throw new System.NotImplementedException();
    }

    public Move? RequestMove()
    {
        throw new System.NotImplementedException();
    }
}

public class MLAgentTeam : IBaseTeam
{
    TurnbasedAgent[] agents;
    Move? ourMove;

    public void InitializeTeam()
    {
        agents = new TurnbasedAgent[6];
        foreach (TurnbasedAgent agent in agents)
            agent.SetCallback((Move agentMove) => { ourMove = agentMove; });
    }

    public Move? RequestMove()
    {
        Move? currentMove = ourMove;
        ourMove = null;

        if (currentMove != null)
        { /* increment current player */ }

        return currentMove;
    }
}