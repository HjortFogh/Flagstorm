using UnityEngine;

public class GameState : MonoBehaviour
{
    private int[,] m_Board;

    public GameState(int[,] board)
    {
        m_Board = board;
    }

    public void MakeMove(int x, int y, int player)
    {
        m_Board[x, y] = player;
    }
}