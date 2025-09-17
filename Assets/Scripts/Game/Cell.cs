using UnityEngine;

public class Cell : MonoBehaviour
{
    public int x, y;

    public void Init(int xPos, int yPos)
    {
        x = xPos;
        y = yPos;
    }
}