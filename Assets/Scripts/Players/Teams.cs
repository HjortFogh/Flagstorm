using UnityEngine;

public struct Move
{
    public int x;
    public int y;

    public Piece piece;

    public Move(Piece _piece, int _x, int _y)
    {
        piece = _piece;
        x = _x;
        y = _y;
    }
}

public abstract class BaseTeam
{
    public GameObject piecePrefab;
    protected Piece[] m_Pieces;

    protected int m_CurrentPlayerIndex;

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

    public virtual void ApproveMove(Move move)
    {
        move.piece.Move(move.x, move.y);
        m_CurrentPlayerIndex = (m_CurrentPlayerIndex + 1) % m_Pieces.GetLength(0);
    }

    public abstract Move? RequestMove();
}

public class PlayerTeam : BaseTeam
{
    public override Move? RequestMove()
    {
        Piece currentPiece = m_Pieces[m_CurrentPlayerIndex];

        if (!currentPiece.OnBoard)
        {
            m_Pieces[m_CurrentPlayerIndex].OnBoard = true;
            m_Pieces[m_CurrentPlayerIndex].Teleport(14, 14);

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
    Move? m_OurMove;

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
        m_Agents[m_CurrentPlayerIndex].RequestMove();
        Move? currentMove = m_OurMove;
        m_OurMove = null;
        return currentMove;
    }
}