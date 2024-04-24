public struct Move
{
    public int x;
    public int y;

    public Move(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class PlayerAgent
{

}

public interface IBaseTeam
{
    public void InitializeTeam(int teamSize);
    public Move? RequestMove();

}

public class PlayerTeam : IBaseTeam
{
    Piece[] m_Pieces;

    public void InitializeTeam(int teamSize)
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
    private TurnbasedAgent[] m_Agents;
    private int m_CurrentPlayerIndex = 0;
    private int m_TeamSize = 6;

    Move? ourMove;

    public void InitializeTeam()
    {
        m_Agents = new TurnbasedAgent[teamSize];

        // for (int i = 0; i < teamSize; i++) FIXME:
        //     m_Agents[i] = new();

        foreach (TurnbasedAgent agent in m_Agents)
        {
            agent.SetCallback((Move agentMove) => { ourMove = agentMove; });
        }
    }

    public Move? RequestMove()
    {
        m_Agents[m_CurrentPlayerIndex].RequestMove();
        Move? currentMove = ourMove;
        ourMove = null;

        if (currentMove != null)
            m_CurrentPlayerIndex = (m_CurrentPlayerIndex + 1) % m_TeamSize;

        return currentMove;
    }
}