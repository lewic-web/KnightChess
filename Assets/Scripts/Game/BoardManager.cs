using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int boardSize = 5;
    public GameObject cellPrefab;    // 棋盘格子预制体
    public Transform boardParent;    // 棋盘父物体（比如一个空的GameObject）

    void Start()
    {
        GenerateBoard();
    }

    void GenerateBoard()
    {
        float offset = 1.1f; // 格子间距
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                GameObject cell = Instantiate(cellPrefab, new Vector3(x * offset, y * offset, 0), Quaternion.identity, boardParent);
                cell.name = $"Cell_{x}_{y}";
            }
        }
    }
}