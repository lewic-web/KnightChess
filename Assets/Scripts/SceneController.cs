using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [Header("场景名称设置")]
    public string mainMenuScene = "MainMenu";
    public string gameScene = "Game";
    public string rulesScene = "Rules";
    public string teamScene = "Team";
    
    private static SceneController instance;
    
    public static SceneController Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("SceneController");
                instance = go.AddComponent<SceneController>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 加载主菜单场景
    /// </summary>
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
    
    /// <summary>
    /// 开始游戏
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene(gameScene);
    }
    
    /// <summary>
    /// 显示游戏规则
    /// </summary>
    public void ShowRules()
    {
        SceneManager.LoadScene(rulesScene);
    }
    
    /// <summary>
    /// 显示团队成员
    /// </summary>
    public void ShowTeam()
    {
        SceneManager.LoadScene(teamScene);
    }
    
    /// <summary>
    /// 退出游戏
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}