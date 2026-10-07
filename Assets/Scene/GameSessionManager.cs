using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSessionManager : MonoBehaviour
{
    [Header("Game Session")]
    private const float MatchDurationSeconds = GameData.MatchDurationSeconds;
    private const string GameSceneName = "Play scene";

    private float remainingTime;
    private bool gameEnded;

    private Canvas hudCanvas;
    [SerializeField] private Text timerText;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureSessionManagerExists()
    {
        EnsureSessionManagerExistsInScene(SceneManager.GetActiveScene());
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RegisterSceneLoadHandler()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureSessionManagerExistsInScene(scene);
    }

    private static void EnsureSessionManagerExistsInScene(Scene scene)
    {
        if (scene.name != GameSceneName)
            return;

        if (FindObjectOfType<GameSessionManager>() != null)
            return;

        GameObject managerObject = new GameObject("GameSessionManager");
        managerObject.AddComponent<GameSessionManager>();
    }

    private void Start()
    {
        ApplySelectedBackground();
        remainingTime = GameData.timeRemaining > 0f
            ? Mathf.Min(GameData.timeRemaining, MatchDurationSeconds)
            : MatchDurationSeconds;
        GameData.timeRemaining = remainingTime;
        gameEnded = false;

        Time.timeScale = 1f;

        EnsureHud();
        UpdateTimerDisplay();
    }

    private void ApplySelectedBackground()
    {
        if (GameData.selectedBackgroundSprite == null)
            return;

        SpriteRenderer backgroundRenderer = GetComponent<SpriteRenderer>();
        if (backgroundRenderer != null)
            backgroundRenderer.sprite = GameData.selectedBackgroundSprite;
    }

    private void Update()
    {
        if (gameEnded)
            return;

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            EndGame();
            return;
        }

        GameData.timeRemaining = remainingTime;
        UpdateTimerDisplay();
    }

    public static void StopCurrentRound()
    {
        GameSessionManager[] managers = FindObjectsByType<GameSessionManager>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (GameSessionManager manager in managers)
            manager.gameEnded = true;
    }

    private void EnsureHud()
    {
        if (timerText != null)
            return;

        GameObject existingTimer = GameObject.Find("TimeText");
        if (existingTimer != null)
            timerText = existingTimer.GetComponent<Text>();

        if (timerText != null)
            return;

        GameObject canvasObject = new GameObject("GameSessionHudCanvas");
        Transform canvasTransform = canvasObject.transform;

        hudCanvas = canvasObject.AddComponent<Canvas>();
        hudCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        GameObject timerObject = new GameObject("TimeText");
        timerObject.transform.SetParent(canvasTransform, false);

        RectTransform timerRect = timerObject.AddComponent<RectTransform>();
        timerRect.anchorMin = new Vector2(0.02f, 0.93f);
        timerRect.anchorMax = new Vector2(0.3f, 1f);
        timerRect.offsetMin = Vector2.zero;
        timerRect.offsetMax = Vector2.zero;

        timerText = timerObject.AddComponent<Text>();
        timerText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        timerText.fontSize = 28;
        timerText.alignment = TextAnchor.MiddleLeft;
        timerText.color = Color.white;
        timerText.text = "TIME: " + Mathf.CeilToInt(MatchDurationSeconds);
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int totalSeconds = Mathf.CeilToInt(Mathf.Max(0f, remainingTime));
            timerText.text = "TIME: " + (totalSeconds / 60).ToString("00") + ":" + (totalSeconds % 60).ToString("00");
        }
    }

    private void EndGame()
    {
        if (gameEnded)
            return;

        gameEnded = true;
        GameData.RecordCompletedRun();
        GameData.timeRemaining = 0f;
        Time.timeScale = 1f;
        SceneManager.LoadScene("RetryScene");
    }
}
