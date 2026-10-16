using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RetrySceneButton : MonoBehaviour
{
    private enum ButtonAction
    {
        Retry,
        ReturnToLogin
    }

    [SerializeField] private ButtonAction action;

    private void Awake()
    {
        Button button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("RetrySceneButton requires a Button component on the same GameObject.", this);
            return;
        }

        button.onClick.AddListener(HandleClick);
    }

    private void HandleClick()
    {
        Time.timeScale = 1f;

        if (action == ButtonAction.Retry)
        {
            GameSessionManager.StopCurrentRound();
            SavePendingCompletedRun(true);
            GameData.MarkRetryUsed();
            GameData.score = 0;
            GameData.timeRemaining = GameData.MatchDurationSeconds;
            SceneManager.LoadScene("Play scene");
            return;
        }

        SavePendingCompletedRun(GameData.HasRetriedThisSession);
        GameData.ResetForNewPlayer();
        SceneManager.LoadScene("LogINScene");
    }

    private static void SavePendingCompletedRun(bool incrementPlayCount)
    {
        GameData.RunRecord completedRun = GameData.RecordPendingCompletedRun();
        if (completedRun != null)
            GoogleSheetsReporter.Submit(completedRun, incrementPlayCount);
    }
}