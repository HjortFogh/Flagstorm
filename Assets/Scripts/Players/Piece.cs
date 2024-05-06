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

/// <summary>
/// Base class for all pieces in the game.
/// </summary>
public abstract class Piece : MonoBehaviour
{
    protected int m_X, m_Y;
    protected bool m_OnBoard;

    protected Piece m_Owner = null;

    public bool IsDestroyed = false;

    public Piece Owner
    {
        get { return m_Owner; }
        set { m_Owner = value; OnBoard = value == null; }
    }

    public int X
    {
        get { if (m_Owner != null) return m_Owner.X; return m_X; }
        set { if (m_Owner == null) m_X = value; }
    }

    public int Y
    {
        get { if (m_Owner != null) return m_Owner.Y; return m_Y; }
        set { if (m_Owner == null) m_Y = value; }
    }

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
        gameObject.AddComponent<MeshRenderer>();
        LoadMesh();
    }

    public abstract void LoadMesh();

    public void UpdatePosition()
    {
        Vector3 newPosition = new(m_X, 0, m_Y);
        transform.position = newPosition;
    }

    public void Move(int ox, int oy)
    {
        m_X += ox;
        m_Y += oy;
        UpdatePosition();
    }

    public void Teleport(int gx, int gy)
    {
        m_X = gx;
        m_Y = gy;
        UpdatePosition();
    }

    public virtual bool CanInteract(Piece other) { return false; }
    public virtual void Interact(Piece other) { }
}

/// <summary>
/// Base class for all pieces that can be placed on the board.
/// </summary>
public class ForestPiece : Piece
{
    public override void LoadMesh()
    {
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        filter.mesh = MeshCollection.RequestMesh("forest");
    }
}

/// <summary>
/// Base class for all pieces that can be placed on the board.
/// </summary>
public class BarricadePiece : Piece
{
    public override void LoadMesh()
    {
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        filter.mesh = MeshCollection.RequestMesh("barricade");
    }
}

/// <summary>
/// TeamPiece is a piece that belongs to a team.
/// </summary>
public abstract class TeamPiece : Piece
{
    public BaseTeam Team;

    public void SetupTeam(BaseTeam team)
    {
        Team = team;
    }

    /// <summary>
    /// Checks if the agent is blocked. (i.e. can't move in any direction)
    /// </summary>
    public bool IsBlocked()
    {
        bool isNorthBlocked = !GameState.Instance.Map.IsWalkable(X, Y + 1) || Team.BlocksCoord(X, Y + 1);
        bool isEastBlocked = !GameState.Instance.Map.IsWalkable(X + 1, Y) || Team.BlocksCoord(X + 1, Y);
        bool isSouthBlocked = !GameState.Instance.Map.IsWalkable(X, Y - 1) || Team.BlocksCoord(X, Y - 1);
        bool isWestBlocked = !GameState.Instance.Map.IsWalkable(X - 1, Y) || Team.BlocksCoord(X - 1, Y);

        return isNorthBlocked && isEastBlocked && isSouthBlocked && isWestBlocked;
    }
}

/// <summary>
/// FlagPiece is a piece that represents the flag of a team
/// </summary>
public class FlagPiece : TeamPiece
{
    public bool AtHome
    {
        get { return Team.teamConfig.flagPosition.x == X && Team.teamConfig.flagPosition.y == Y; }
        set { if (value) Teleport(Team.teamConfig.flagPosition.x, Team.teamConfig.flagPosition.y); }
    }

    public override void LoadMesh()
    {
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        filter.mesh = MeshCollection.RequestMesh("flag");
    }
}


/// <summary>
/// MovablePiece is a piece that can move around the board.
/// </summary>
public abstract class MovablePiece : TeamPiece
{
    protected Piece m_Inventory = null;
    protected bool m_IsDead;

    public bool IsDead
    {
        get => m_IsDead;
        set
        {
            m_IsDead = value;
            OnBoard = !m_IsDead;

            if (m_IsDead == false)
                Teleport(Team.spawnPoint.x, Team.spawnPoint.y);
        }
    }
}

/// <summary>
/// DefenderPiece is a piece that can gather resources.
/// </summary>
public class DefenderPiece : MovablePiece
{
    public override void LoadMesh()
    {
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        filter.mesh = MeshCollection.RequestMesh("defender");
    }

    public override bool CanInteract(Piece other)
    {
        return other switch
        {
            ForestPiece => true,
            _ => false
        };
    }

    public override void Interact(Piece other)
    {
        if (other is ForestPiece)
        {
            m_Inventory = other;
            other.Owner = this;
        }
    }
}

/// <summary>
/// AttackerPiece is a piece that can attack other pieces and interact with flags.
/// </summary>
public class AttackerPiece : MovablePiece
{
    public override void LoadMesh()
    {
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        filter.mesh = MeshCollection.RequestMesh("attacker");
    }

    public override bool CanInteract(Piece other)
    {
        if (other is FlagPiece flagPiece)
            return (Team != flagPiece.Team) || (flagPiece.Team == Team && !flagPiece.AtHome);

        if (other is MovablePiece movablePiece)
            return movablePiece.Team != Team;

        if (other is BarricadePiece)
            return true;

        return false;
    }

    public override void Interact(Piece other)
    {
        if (other is FlagPiece)
        {
            m_Inventory = other;
            other.Owner = this;
        }

        if (other is DefenderPiece defenderPiece)
        {
            defenderPiece.IsDead = true;
            Debug.Log("Dead defender, L");
        }

        if (other is AttackerPiece attackerPiece)
        {
            attackerPiece.IsDead = true;
            IsDead = true;
        }

        if (other is BarricadePiece barricadePiece)
            barricadePiece.IsDestroyed = true;
    }
}
