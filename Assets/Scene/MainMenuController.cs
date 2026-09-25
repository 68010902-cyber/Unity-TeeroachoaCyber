using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string playSceneName = "Play scene";

    public void PlayGame()
    {
        if (SceneManager.GetSceneByName(playSceneName).IsValid())
        {
            SceneManager.LoadScene(playSceneName);
            return;
        }

        int sceneIndex = SceneUtility.GetBuildIndexByScenePath($"Assets/Scene/{playSceneName}.unity");
        if (sceneIndex >= 0)
        {
            SceneManager.LoadScene(sceneIndex);
            return;
        }

        Debug.LogWarning($"Scene '{playSceneName}' not found. Make sure the scene is added to Build Settings.");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}