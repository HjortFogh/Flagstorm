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


public class Piece : MonoBehaviour
{
    // Attacker, Defender
    public enum PieceType { Static, Nonstatic };
    public PieceType type;

    public int x, y;
    private bool m_OnBoard;

    //private IPieceObject m_Holding;

    public bool OnBoard
    {
        get => m_OnBoard;
        set
        {
            m_OnBoard = value;
            gameObject.SetActive(m_OnBoard);
        }
    }

    void Awake()
    {
        OnBoard = false;
    }

    public void UpdatePosition(Vector3? forward = null)
    {
        Vector3 newPosition = new(x, 0, y);
        transform.position = newPosition;
        if (forward != null)
            transform.rotation = Quaternion.LookRotation((Vector3)forward, transform.up);
    }

    public void Move(int ox, int oy)
    {
        x += ox;
        y += oy;
        UpdatePosition();
    }

    public void Teleport(int gx, int gy)
    {
        x = gx;
        y = gy;
        UpdatePosition();
    }
}


public class PlayerPiece : Piece
{

}

public class AgentPiece : Piece
{

}

public class FlagPiece : Piece
{

}

public class ForestPiece : Piece
{

}

/*

PlayerPiece : Piece, Nonstatic
AgentPiece : Piece, Nonstatic

FlagPiece : Piece, Static
ForestPiece : Piece, Static

Hvis piece går ind forest piece:
    ForestPiece.OnBoard = false;
    opdater PlayerPiece.inventory = ForestPiece

*/