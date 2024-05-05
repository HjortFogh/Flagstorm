using UnityEngine;

[System.Serializable]
public class TeamConfig
{
    public enum TeamType { Human, Model }
    public TeamType type;
    public int teamSize = 6;

    public Material material;

    public Vector2Int basePosition;
    public Vector2Int flagPosition;

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
    public MovablePiece[] Pieces;

    public FlagPiece teamFlag;
    public Vector2Int spawnPoint;
    public TeamConfig teamConfig;

    protected int m_CurrentPlayerIndex;

    public virtual bool BlocksCoord(int xGlobal, int yGlobal)
    {
        foreach (Piece piece in Pieces)
        {
            if (piece.X == xGlobal && piece.Y == yGlobal)
                return true;
        }
        return false;
    }

    public virtual void InitializeTeam(int teamSize, TeamConfig config, FlagPiece flag)
    {
        Pieces = new MovablePiece[teamSize];
        teamConfig = config;

        for (int i = 0; i < teamSize; i++)
        {
            bool isAttacker = i % 2 == 0;
            GameObject piece = new();

            if (isAttacker)
                Pieces[i] = piece.AddComponent<AttackerPiece>();
            else
                Pieces[i] = piece.AddComponent<DefenderPiece>();

            Pieces[i].Teleport(config.basePosition.x, config.basePosition.y);
            Pieces[i].OnBoard = false;

            Pieces[i].SetupTeam(this);
            Pieces[i].gameObject.GetComponent<MeshRenderer>().material = config.material;
        }

        teamFlag = flag;
        spawnPoint = config.basePosition;
    }

    public abstract Move? RequestMove();

    public virtual void NextPlayer()
    {
        m_CurrentPlayerIndex = (m_CurrentPlayerIndex + 1) % Pieces.GetLength(0);
    }

    public virtual void DeclineMove(Move move) { }
}

public class PlayerTeam : BaseTeam
{
    public override Move? RequestMove()
    {
        Piece currentPiece = Pieces[m_CurrentPlayerIndex];

        if (!currentPiece.OnBoard)
        {
            Pieces[m_CurrentPlayerIndex].OnBoard = true;
            // m_Pieces[m_CurrentPlayerIndex].Teleport(14, 24);

            m_CurrentPlayerIndex = 0;
            currentPiece = Pieces[m_CurrentPlayerIndex];
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

    public override void InitializeTeam(int teamSize, TeamConfig config, FlagPiece flag)
    {
        base.InitializeTeam(teamSize, config, flag);
        m_Agents = new TurnbasedAgent[teamSize];

        for (int i = 0; i < teamSize; i++)
        {
            m_Agents[i] = Pieces[i].gameObject.AddComponent<TurnbasedAgent>();
            m_Agents[i].SetCallback((Move agentMove) => { m_OurMove = agentMove; });
        }
    }

    public override Move? RequestMove()
    {
        Piece currentPiece = Pieces[m_CurrentPlayerIndex];

        if (!currentPiece.OnBoard)
        {
            Pieces[m_CurrentPlayerIndex].OnBoard = true;
            //m_Pieces[m_CurrentPlayerIndex].Teleport(14, 4);
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

        if (agent.IsBlocked())
        {
            GameState.Instance.NextTeam();
        }


    }
}
