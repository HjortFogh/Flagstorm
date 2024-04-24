using UnityEngine;

public class Piece : MonoBehaviour
{
    public enum PieceType
    {
        Attacker, Defender
    };

    public int x, y;
    private bool m_OnBoard;

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
        Vector3 newPosition = new(x + 0.5f, 0, y + 0.5f);
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