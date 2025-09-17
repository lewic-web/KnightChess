using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BoardUI : MonoBehaviour
{
    [Header("UI组件")]
    public GridLayoutGroup boardGrid;
    public Button cellPrefab;
    public Text currentPlayerText;
    public Text whiteScoreText;
    public Text blackScoreText;
    
    [Header("棋子选择UI")]
    public Button swordButton;
    public Button shieldButton;
    public Button soldierButton;
    
    [Header("剩余棋子显示")]
    public Text whiteSwordCount;
    public Text whiteShieldCount;
    public Text whiteSoldierCount;
    public Text blackSwordCount;
    public Text blackShieldCount;
    public Text blackSoldierCount;
    
    [Header("颜色设置")]
    public Color whitePieceColor = Color.white;
    public Color blackPieceColor = Color.black;
    public Color swordColor = Color.red;
    public Color shieldColor = Color.blue;
    public Color soldierColor = Color.green;
    
    private Button[,] cellButtons;
    private PieceType selectedPieceType = PieceType.Sword;
    private Dictionary<Button, Vector2Int> buttonPositions;
    
    private void Start()
    {
        InitializeBoardUI();
        SetupPieceButtons();
        UpdateUI();
    }
    
    private void InitializeBoardUI()
    {
        buttonPositions = new Dictionary<Button, Vector2Int>();
        cellButtons = new Button[5, 5];
        
        // 设置网格布局
        boardGrid.constraintCount = 5;
        
        // 创建棋盘格子
        for (int y = 4; y >= 0; y--) // 从上到下创建，但y坐标从4到0
        {
            for (int x = 0; x < 5; x++)
            {
                Button cellButton = Instantiate(cellPrefab, boardGrid.transform);
                cellButtons[x, y] = cellButton;
                
                Vector2Int position = new Vector2Int(x, y);
                buttonPositions[cellButton] = position;
                
                cellButton.onClick.AddListener(() => OnCellClick(position));
                
                // 设置格子的初始外观
                cellButton.GetComponent<Image>().color = Color.white;
            }
        }
    }
    
    private void SetupPieceButtons()
    {
        if (swordButton != null)
            swordButton.onClick.AddListener(() => SelectPieceType(PieceType.Sword));
        
        if (shieldButton != null)
            shieldButton.onClick.AddListener(() => SelectPieceType(PieceType.Shield));
        
        if (soldierButton != null)
            soldierButton.onClick.AddListener(() => SelectPieceType(PieceType.Soldier));
        
        UpdatePieceButtonSelection();
    }
    
    private void SelectPieceType(PieceType pieceType)
    {
        selectedPieceType = pieceType;
        UpdatePieceButtonSelection();
    }
    
    private void UpdatePieceButtonSelection()
    {
        // 更新按钮选中状态的视觉反馈
        if (swordButton != null)
            swordButton.GetComponent<Image>().color = selectedPieceType == PieceType.Sword ? Color.yellow : Color.white;
        
        if (shieldButton != null)
            shieldButton.GetComponent<Image>().color = selectedPieceType == PieceType.Shield ? Color.yellow : Color.white;
        
        if (soldierButton != null)
            soldierButton.GetComponent<Image>().color = selectedPieceType == PieceType.Soldier ? Color.yellow : Color.white;
    }
    
    private void OnCellClick(Vector2Int position)
    {
        bool success = GameManager.Instance.TryPlacePiece(position.x, position.y, selectedPieceType);
        
        if (success)
        {
            UpdateUI();
        }
        else
        {
            Debug.Log("无法在此位置放置棋子！");
        }
    }
    
    private void UpdateUI()
    {
        UpdateBoard();
        UpdatePlayerInfo();
        UpdateScores();
        UpdateRemainingPieces();
    }
    
    private void UpdateBoard()
    {
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                UpdateCell(x, y);
            }
        }
    }
    
    private void UpdateCell(int x, int y)
    {
        Button cellButton = cellButtons[x, y];
        List<Piece> pieces = GameManager.Instance.GetPiecesAt(x, y);
        
        // 清除当前显示
        Transform[] children = new Transform[cellButton.transform.childCount];
        for (int i = 0; i < children.Length; i++)
        {
            children[i] = cellButton.transform.GetChild(i);
        }
        foreach (Transform child in children)
        {
            if (child.name.Contains("Piece"))
                DestroyImmediate(child.gameObject);
        }
        
        // 显示棋子
        if (pieces != null && pieces.Count > 0)
        {
            DisplayPieces(cellButton, pieces);
        }
    }
    
    private void DisplayPieces(Button cellButton, List<Piece> pieces)
    {
        // 简单的棋子显示：用不同颜色的小圆点表示
        float pieceSize = 20f;
        float spacing = 25f;
        
        for (int i = 0; i < pieces.Count; i++)
        {
            GameObject pieceObj = new GameObject($"Piece_{i}");
            pieceObj.transform.SetParent(cellButton.transform);
            
            Image pieceImage = pieceObj.AddComponent<Image>();
            RectTransform rectTransform = pieceObj.GetComponent<RectTransform>();
            
            rectTransform.sizeDelta = new Vector2(pieceSize, pieceSize);
            rectTransform.anchoredPosition = new Vector2((i % 3 - 1) * spacing, (i / 3 - 1) * -spacing);
            
            // 设置颜色（根据棋子类型和玩家）
            Color baseColor = pieces[i].player == Player.White ? whitePieceColor : blackPieceColor;
            Color typeColor = GetPieceTypeColor(pieces[i].type);
            
            pieceImage.color = Color.Lerp(baseColor, typeColor, 0.7f);
        }
    }
    
    private Color GetPieceTypeColor(PieceType type)
    {
        switch (type)
        {
            case PieceType.Sword: return swordColor;
            case PieceType.Shield: return shieldColor;
            case PieceType.Soldier: return soldierColor;
            default: return Color.gray;
        }
    }
    
    private void UpdatePlayerInfo()
    {
        if (currentPlayerText != null)
        {
            currentPlayerText.text = $"当前玩家: {(GameManager.Instance.currentPlayer == Player.White ? "白方" : "黑方")}";
        }
    }
    
    private void UpdateScores()
    {
        if (whiteScoreText != null)
            whiteScoreText.text = $"白方: {GameManager.Instance.GetScore(Player.White)}";
        
        if (blackScoreText != null)
            blackScoreText.text = $"黑方: {GameManager.Instance.GetScore(Player.Black)}";
    }
    
    private void UpdateRemainingPieces()
    {
        // 更新白方剩余棋子
        if (whiteSwordCount != null)
            whiteSwordCount.text = GameManager.Instance.GetRemainingPieces(Player.White, PieceType.Sword).ToString();
        
        if (whiteShieldCount != null)
            whiteShieldCount.text = GameManager.Instance.GetRemainingPieces(Player.White, PieceType.Shield).ToString();
        
        if (whiteSoldierCount != null)
            whiteSoldierCount.text = GameManager.Instance.GetRemainingPieces(Player.White, PieceType.Soldier).ToString();
        
        // 更新黑方剩余棋子
        if (blackSwordCount != null)
            blackSwordCount.text = GameManager.Instance.GetRemainingPieces(Player.Black, PieceType.Sword).ToString();
        
        if (blackShieldCount != null)
            blackShieldCount.text = GameManager.Instance.GetRemainingPieces(Player.Black, PieceType.Shield).ToString();
        
        if (blackSoldierCount != null)
            blackSoldierCount.text = GameManager.Instance.GetRemainingPieces(Player.Black, PieceType.Soldier).ToString();
    }
    
    // 返回主菜单
    public void ReturnToMainMenu()
    {
        SceneController.Instance.LoadMainMenu();
    }
}