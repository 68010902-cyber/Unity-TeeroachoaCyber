using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private const string GameSceneName = "Play scene";
    private const string ContinueMenuSceneName = "restartscene";
    private static Camera gameplayCamera;
    private static EventSystem gameplayEventSystem;

    private void Awake()
    {
        if (gameObject.scene.name != GameSceneName)
            return;

        gameplayCamera = Camera.main;
        gameplayEventSystem = EventSystem.current;

        Button pauseButton = GetComponent<Button>();
        if (pauseButton != null)
            pauseButton.onClick.AddListener(OpenContinueMenu);
    }

    private void OpenContinueMenu()
    {
        if (SceneManager.GetSceneByName(ContinueMenuSceneName).isLoaded)
            return;

        Time.timeScale = 0f;
        if (gameplayCamera != null)
            gameplayCamera.enabled = false;
        if (gameplayEventSystem != null)
            gameplayEventSystem.enabled = false;

        SceneManager.LoadScene(ContinueMenuSceneName, LoadSceneMode.Additive);
    }

    public static void ResumeGame()
    {
        Scene pauseScene = SceneManager.GetSceneByName(ContinueMenuSceneName);
        if (pauseScene.isLoaded)
        {
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(pauseScene);
            if (unloadOperation != null)
            {
                unloadOperation.completed += _ => RestoreGameplay();
                return;
            }
        }

        RestoreGameplay();
    }

    private static void RestoreGameplay()
    {
        if (gameplayCamera != null)
            gameplayCamera.enabled = true;
        if (gameplayEventSystem != null)
            gameplayEventSystem.enabled = true;
        Time.timeScale = 1f;
    }

    public void QuitToLogin()
    {
        Time.timeScale = 1f;
        GameData.ResetForNewPlayer();
        SceneManager.LoadScene("LogINScene");
    }
}