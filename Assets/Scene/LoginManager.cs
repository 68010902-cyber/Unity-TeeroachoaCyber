using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoginManager : MonoBehaviour
{
    [Header("Login UI")]
    public TMP_InputField usernameInput;
    public TMP_Dropdown difficultyDropdown;
    public Button startButton;

    [Header("Scene")]
    public string gameSceneName = "Play scene";

    private void Reset()
    {
        AutoBindUi();
    }

    private void OnValidate()
    {
        AutoBindUi();
    }

    private void Awake()
    {
        AutoBindUi();

        if (startButton != null)
        {
            startButton.onClick.RemoveListener(OnPlayPressed);
            startButton.onClick.AddListener(OnPlayPressed);
        }
    }

    private void AutoBindUi()
    {
        if (usernameInput == null)
            usernameInput = FindComponentByName<TMP_InputField>("UsernameInput");

        if (difficultyDropdown == null)
            difficultyDropdown = FindComponentByName<TMP_Dropdown>("DifficultyDropdown");

        if (startButton == null)
            startButton = FindComponentByName<Button>("StartButton");
    }

    private T FindComponentByName<T>(string name) where T : Component
    {
        GameObject go = GameObject.Find(name);
        if (go == null)
            go = GameObject.Find($"{name}(Clone)");

        if (go == null)
            return null;

        return go.GetComponent<T>();
    }

    public void OnPlayPressed()
    {
        AutoBindUi();

        GameData.ResetForNewPlayer();

        if (usernameInput != null)
        {
            GameData.username = string.IsNullOrWhiteSpace(usernameInput.text)
                ? "Player"
                : usernameInput.text;
        }

        if (difficultyDropdown != null)
        {
            GameData.selectedLevel = (FruitSpeedLevel)Mathf.Clamp(difficultyDropdown.value, 0, 2);
        }

        GameData.score = 0;
        GameData.timeRemaining = GameData.MatchDurationSeconds;

        if (!string.IsNullOrEmpty(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
        }
    }
}
