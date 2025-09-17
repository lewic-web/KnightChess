using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    private Button backButton;
    
    private void Start()
    {
        backButton = GetComponent<Button>();
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClick);
        }
    }
    
    private void OnBackButtonClick()
    {
        SceneController.Instance.LoadMainMenu();
    }
}