using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("主菜单UI组件")]
    public Button startGameButton;
    public Button rulesButton;
    public Button teamButton;
    public Button quitButton;
    
    [Header("动画设置")]
    public float buttonAnimationDuration = 0.3f;
    public float buttonHoverScale = 1.1f;
    
    [Header("音效设置")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    public AudioClip buttonHoverSound;
    
    private void Start()
    {
        SetupButtons();
        PlayIntroAnimation();
    }
    
    private void SetupButtons()
    {
        // 设置按钮点击事件
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(() => OnButtonClick("StartGame"));
            AddButtonEffects(startGameButton);
        }
        
        if (rulesButton != null)
        {
            rulesButton.onClick.AddListener(() => OnButtonClick("Rules"));
            AddButtonEffects(rulesButton);
        }
        
        if (teamButton != null)
        {
            teamButton.onClick.AddListener(() => OnButtonClick("Team"));
            AddButtonEffects(teamButton);
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(() => OnButtonClick("Quit"));
            AddButtonEffects(quitButton);
        }
    }
    
    private void AddButtonEffects(Button button)
    {
        // 添加鼠标悬停效果
        var eventTrigger = button.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        }
        
        // 鼠标进入事件
        var pointerEnter = new UnityEngine.EventSystems.EventTrigger.Entry();
        pointerEnter.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) => OnButtonHover(button, true));
        eventTrigger.triggers.Add(pointerEnter);
        
        // 鼠标离开事件
        var pointerExit = new UnityEngine.EventSystems.EventTrigger.Entry();
        pointerExit.eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) => OnButtonHover(button, false));
        eventTrigger.triggers.Add(pointerExit);
    }
    
    private void OnButtonHover(Button button, bool isHovering)
    {
        if (buttonHoverSound != null && audioSource != null && isHovering)
        {
            audioSource.PlayOneShot(buttonHoverSound);
        }
        
        float targetScale = isHovering ? buttonHoverScale : 1f;
        StartCoroutine(ScaleButton(button.transform, targetScale));
    }
    
    private void OnButtonClick(string buttonType)
    {
        // 播放点击音效
        if (buttonClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
        
        // 根据按钮类型执行相应操作
        switch (buttonType)
        {
            case "StartGame":
                SceneController.Instance.StartGame();
                break;
            case "Rules":
                SceneController.Instance.ShowRules();
                break;
            case "Team":
                SceneController.Instance.ShowTeam();
                break;
            case "Quit":
                SceneController.Instance.QuitGame();
                break;
        }
    }
    
    private void PlayIntroAnimation()
    {
        // 为所有按钮播放入场动画
        Button[] buttons = { startGameButton, rulesButton, teamButton, quitButton };
        
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
            {
                buttons[i].transform.localScale = Vector3.zero;
                StartCoroutine(IntroAnimateButton(buttons[i].transform, i * 0.1f));
            }
        }
    }
    
    private IEnumerator ScaleButton(Transform buttonTransform, float targetScale)
    {
        Vector3 startScale = buttonTransform.localScale;
        Vector3 endScale = Vector3.one * targetScale;
        float elapsed = 0f;
        
        while (elapsed < buttonAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / buttonAnimationDuration;
            
            // 使用EaseOutBack效果的近似
            t = 1f - Mathf.Pow(1f - t, 3f);
            if (targetScale > 1f) // hover效果
            {
                t = t * (1f + 0.3f * Mathf.Sin(t * Mathf.PI));
            }
            
            buttonTransform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        
        buttonTransform.localScale = endScale;
    }
    
    private IEnumerator IntroAnimateButton(Transform buttonTransform, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;
        float elapsed = 0f;
        
        while (elapsed < buttonAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / buttonAnimationDuration;
            
            // EaseOutBack效果
            t = 1f + (--t) * t * ((1.7f + 1f) * t + 1.7f);
            
            buttonTransform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        
        buttonTransform.localScale = endScale;
    }
}