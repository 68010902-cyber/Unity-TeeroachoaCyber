using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Restartscene : MonoBehaviour
{
    private const string GameSceneName = "Play scene";

    [SerializeField] private Button continueButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        if (continueButton != null)
            continueButton.onClick.AddListener(ContinueGame);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (quitButton == null)
        {
            GameObject quitButtonObject = GameObject.Find("quitButton");
            if (quitButtonObject != null)
                quitButton = quitButtonObject.GetComponent<Button>();
        }

        if (quitButton != null)
            quitButton.onClick.AddListener(ReturnToLogin);
    }

    private void ContinueGame()
    {
        PauseMenu.ResumeGame();
    }

    private void RestartGame()
    {
        GameSessionManager.StopCurrentRound();
        Time.timeScale = 1f;
        GameData.score = 0;
        GameData.timeRemaining = GameData.MatchDurationSeconds;
        SceneManager.LoadScene(GameSceneName);
    }

    private void ReturnToLogin()
    {
        Time.timeScale = 1f;
        GameData.ResetForNewPlayer();
        SceneManager.LoadScene("LogINScene");
    }
}
