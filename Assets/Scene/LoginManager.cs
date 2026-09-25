using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [Header("Login UI")]
    public InputField usernameInput;
    public Dropdown difficultyDropdown;
    public Button startButton;

    [Header("Scene")]
    public string gameSceneName = "Play scene";

    private void Awake()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnPlayPressed);
        }
    }

    public void OnPlayPressed()
    {
        if (usernameInput != null)
        {
            GameData.username = string.IsNullOrWhiteSpace(usernameInput.text)
                ? "Player"
                : usernameInput.text;
        }

        if (difficultyDropdown != null)
        {
            GameData.selectedLevel = (FruitSpeedLevel)difficultyDropdown.value;
        }

        GameData.score = 0;

        if (!string.IsNullOrEmpty(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
        }
    }
}
