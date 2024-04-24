using UnityEngine;


[System.Serializable]
public class TeamConfig
{
    public GameObject prefab;
    public enum TeamType { Human, Model }
    public TeamType type;
    public int teamSize = 6;
    // public Vector2Int startingPosition

    public BaseTeam GetClass()
    {
        return type switch
        {
            TeamType.Human => new PlayerTeam(),
            _ => new MLAgentTeam(),
        };
    }
}

public abstract class BaseTeam
{
    public GameObject piecePrefab;
    protected Piece[] m_Pieces;

    protected int m_CurrentPlayerIndex;

    // private 

    public virtual bool BlocksCoord(int xGlobal, int yGlobal)
    {
        foreach (Piece piece in m_Pieces)
        {
            if (piece.x == xGlobal && piece.y == yGlobal)
                return true;
        }
        return false;
    }

    public virtual void InitializeTeam(int teamSize, GameObject prefab)
    {
        piecePrefab = prefab;
        m_Pieces = new Piece[teamSize];

        for (int i = 0; i < teamSize; i++)
        {
            GameObject obj = Object.Instantiate(piecePrefab, Vector3.zero, Quaternion.identity);
            m_Pieces[i] = obj.GetComponent<Piece>();
        }
    }

    public abstract Move? RequestMove();

    public virtual void NextPlayer()
    {
        m_CurrentPlayerIndex = (m_CurrentPlayerIndex + 1) % m_Pieces.GetLength(0);
    }

    public virtual void DeclineMove(Move move) { }
}

public class PlayerTeam : BaseTeam
{
    public override Move? RequestMove()
    {
        Piece currentPiece = m_Pieces[m_CurrentPlayerIndex];

        if (!currentPiece.OnBoard)
        {
            m_Pieces[m_CurrentPlayerIndex].OnBoard = true;
            m_Pieces[m_CurrentPlayerIndex].Teleport(14, 24);

            m_CurrentPlayerIndex = 0;
            currentPiece = m_Pieces[m_CurrentPlayerIndex];
        }

        if (Input.GetKey(KeyCode.UpArrow))
            return new Move(currentPiece, +0, +1);
        else if (Input.GetKey(KeyCode.DownArrow))
            return new Move(currentPiece, +0, -1);
        else if (Input.GetKey(KeyCode.LeftArrow))
            return new Move(currentPiece, -1, +0);
        else if (Input.GetKey(KeyCode.RightArrow))
            return new Move(currentPiece, +1, +0);

        return null;
    }
}

public class MLAgentTeam : BaseTeam
{
    private TurnbasedAgent[] m_Agents;
    private Move? m_OurMove;

    public override void InitializeTeam(int teamSize, GameObject prefab)
    {
        base.InitializeTeam(teamSize, prefab);
        m_Agents = new TurnbasedAgent[teamSize];

        for (int i = 0; i < teamSize; i++)
        {
            m_Agents[i] = m_Pieces[i].gameObject.GetComponent<TurnbasedAgent>();
            m_Agents[i].SetCallback((Move agentMove) => { m_OurMove = agentMove; });
        }
    }

    public override Move? RequestMove()
    {
        Piece currentPiece = m_Pieces[m_CurrentPlayerIndex];

        if (!currentPiece.OnBoard)
        {
            m_Pieces[m_CurrentPlayerIndex].OnBoard = true;
            m_Pieces[m_CurrentPlayerIndex].Teleport(14, 4);
            m_CurrentPlayerIndex = 0;
        }

        m_Agents[m_CurrentPlayerIndex].RequestMove();
        Move? currentMove = m_OurMove;
        m_OurMove = null;

        return currentMove;
    }

    public override void DeclineMove(Move move)
    {
        TurnbasedAgent agent = m_Agents[m_CurrentPlayerIndex];

        // agent.SetReward(-1.00f);
        // agent.EndEpisode();

    }
}