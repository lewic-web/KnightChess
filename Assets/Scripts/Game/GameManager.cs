using UnityEngine;
using System.Collections.Generic;

public enum PieceType
{
    Sword,  // 长剑
    Shield, // 木盾
    Soldier // 士兵
}

public enum Player
{
    White,
    Black
}

[System.Serializable]
public class Piece
{
    public PieceType type;
    public Player player;
    
    public Piece(PieceType type, Player player)
    {
        this.type = type;
        this.player = player;
    }
}

public class GameManager : MonoBehaviour
{
    [Header("游戏设置")]
    public int boardSize = 5;
    public int piecesPerType = 7;
    
    [Header("当前游戏状态")]
    public Player currentPlayer = Player.White;
    public PieceType? lastPlacedPieceType = null;
    public Vector2Int? lastPlacedPosition = null;
    public bool isFirstMove = true;
    
    // 棋盘：每个位置包含多个棋子的列表
    private List<Piece>[,] board;
    
    // 玩家剩余棋子数量
    private Dictionary<Player, Dictionary<PieceType, int>> remainingPieces;
    
    // 分数
    private Dictionary<Player, int> scores;
    
    public static GameManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeGame()
    {
        // 初始化棋盘
        board = new List<Piece>[boardSize, boardSize];
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                board[x, y] = new List<Piece>();
            }
        }
        
        // 初始化剩余棋子数量
        remainingPieces = new Dictionary<Player, Dictionary<PieceType, int>>();
        foreach (Player player in System.Enum.GetValues(typeof(Player)))
        {
            remainingPieces[player] = new Dictionary<PieceType, int>();
            foreach (PieceType pieceType in System.Enum.GetValues(typeof(PieceType)))
            {
                remainingPieces[player][pieceType] = piecesPerType;
            }
        }
        
        // 初始化分数
        scores = new Dictionary<Player, int>();
        scores[Player.White] = 0;
        scores[Player.Black] = 0;
    }
    
    /// <summary>
    /// 尝试在指定位置放置棋子
    /// </summary>
    public bool TryPlacePiece(int x, int y, PieceType pieceType)
    {
        // 检查位置是否有效
        if (!IsValidPosition(x, y))
            return false;
        
        // 检查是否有剩余棋子
        if (remainingPieces[currentPlayer][pieceType] <= 0)
            return false;
        
        // 检查该位置是否已有相同类型的棋子
        if (HasPieceOfType(x, y, pieceType, currentPlayer))
            return false;
        
        // 检查放置规则
        if (!IsValidPlacement(x, y, pieceType))
            return false;
        
        // 放置棋子
        PlacePiece(x, y, pieceType);
        
        return true;
    }
    
    private bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < boardSize && y >= 0 && y < boardSize;
    }
    
    private bool HasPieceOfType(int x, int y, PieceType pieceType, Player player)
    {
        foreach (Piece piece in board[x, y])
        {
            if (piece.type == pieceType && piece.player == player)
                return true;
        }
        return false;
    }
    
    private bool IsValidPlacement(int x, int y, PieceType pieceType)
    {
        // 第一步可以在任意位置放置
        if (isFirstMove)
            return true;
        
        // 根据上一个棋子的类型和位置判断
        if (lastPlacedPieceType == null || lastPlacedPosition == null)
            return true;
        
        Vector2Int lastPos = lastPlacedPosition.Value;
        PieceType lastType = lastPlacedPieceType.Value;
        
        // 根据游戏规则：长剑后必须在上下左右两格内放置木盾
        if (lastType == PieceType.Sword && pieceType == PieceType.Shield)
        {
            int distance = Mathf.Abs(x - lastPos.x) + Mathf.Abs(y - lastPos.y);
            return distance <= 2 && (x == lastPos.x || y == lastPos.y);
        }
        
        // 可以根据需要添加更多规则
        return true;
    }
    
    private void PlacePiece(int x, int y, PieceType pieceType)
    {
        // 添加棋子到棋盘
        Piece newPiece = new Piece(pieceType, currentPlayer);
        board[x, y].Add(newPiece);
        
        // 减少剩余棋子数量
        remainingPieces[currentPlayer][pieceType]--;
        
        // 更新游戏状态
        lastPlacedPieceType = pieceType;
        lastPlacedPosition = new Vector2Int(x, y);
        isFirstMove = false;
        
        // 计算分数
        CalculateScores(x, y);
        
        // 切换玩家
        currentPlayer = currentPlayer == Player.White ? Player.Black : Player.White;
        
        // 检查游戏结束条件
        CheckGameEnd();
    }
    
    private void CalculateScores(int x, int y)
    {
        List<Piece> piecesAtPosition = board[x, y];
        
        // 统计每个玩家的棋子类型
        Dictionary<Player, HashSet<PieceType>> playerPieces = new Dictionary<Player, HashSet<PieceType>>();
        playerPieces[Player.White] = new HashSet<PieceType>();
        playerPieces[Player.Black] = new HashSet<PieceType>();
        
        foreach (Piece piece in piecesAtPosition)
        {
            playerPieces[piece.player].Add(piece.type);
        }
        
        // 检查骑士（某一方有三种棋子）
        foreach (Player player in playerPieces.Keys)
        {
            if (playerPieces[player].Count == 3)
            {
                scores[player] += 3;
                return; // 骑士优先，不再计算其他分数
            }
        }
        
        // 检查混合情况（不同方的三种棋子）
        HashSet<PieceType> allTypes = new HashSet<PieceType>();
        foreach (var types in playerPieces.Values)
        {
            foreach (var type in types)
            {
                allTypes.Add(type);
            }
        }
        
        if (allTypes.Count == 3)
        {
            // 找出占多数的玩家
            Player majorityPlayer = playerPieces[Player.White].Count > playerPieces[Player.Black].Count ? 
                Player.White : Player.Black;
            scores[majorityPlayer] += 1;
        }
    }
    
    private void CheckGameEnd()
    {
        // 检查是否所有棋子都用完
        bool gameEnd = true;
        foreach (var playerPieces in remainingPieces.Values)
        {
            foreach (var count in playerPieces.Values)
            {
                if (count > 0)
                {
                    gameEnd = false;
                    break;
                }
            }
            if (!gameEnd) break;
        }
        
        if (gameEnd)
        {
            EndGame();
        }
    }
    
    private void EndGame()
    {
        Player winner = scores[Player.White] > scores[Player.Black] ? Player.White : Player.Black;
        Debug.Log($"游戏结束！获胜者：{winner}，分数：白方{scores[Player.White]} - 黑方{scores[Player.Black]}");
        
        // 可以在这里触发游戏结束UI
    }
    
    // 公共访问方法
    public List<Piece> GetPiecesAt(int x, int y)
    {
        if (IsValidPosition(x, y))
            return board[x, y];
        return null;
    }
    
    public int GetRemainingPieces(Player player, PieceType pieceType)
    {
        return remainingPieces[player][pieceType];
    }
    
    public int GetScore(Player player)
    {
        return scores[player];
    }
}