using UnityEngine;

public class Piece : MonoBehaviour
{
    public int x, y;
    public bool onBoard = false;

    public void Move(int ox, int oy)
    {
        x += ox;
        y += oy;
    }

    public void Teleport(int gx, int gy)
    {
        x = gx;
        y = gy;
    }
}