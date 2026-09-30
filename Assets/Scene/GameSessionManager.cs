using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSessionManager : MonoBehaviour
{
    [Header("Game Session")]
    private const float MatchDurationSeconds = GameData.MatchDurationSeconds;
    [SerializeField] private string loginSceneName = "LogINScene";

    private const string GameSceneName = "Play scene";

    private float remainingTime;
    private bool gameEnded;

    private Canvas hudCanvas;
    private Text timerText;
    private Text resultText;
    private Text bestScoreText;
    private GameObject gameOverPanel;
    private Button retryButton;
    private Button homeButton;

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
        remainingTime = GameData.timeRemaining > 0f
            ? Mathf.Min(GameData.timeRemaining, MatchDurationSeconds)
            : MatchDurationSeconds;
        GameData.timeRemaining = remainingTime;
        gameEnded = false;

        Time.timeScale = 1f;

        EnsureHud();
        EnsureGameOverPanel();
        UpdateTimerDisplay();
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

    private void EnsureHud()
    {
        if (hudCanvas != null)
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
        timerText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        timerText.fontSize = 28;
        timerText.alignment = TextAnchor.MiddleLeft;
        timerText.color = Color.white;
        timerText.text = "TIME: " + Mathf.CeilToInt(MatchDurationSeconds);
    }

    private void EnsureGameOverPanel()
    {
        if (gameOverPanel != null)
            return;

        if (hudCanvas == null)
            EnsureHud();

        GameObject panelObject = new GameObject("GameOverPanel");
        panelObject.transform.SetParent(hudCanvas.transform, false);

        RectTransform panelRect = panelObject.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.2f, 0.2f);
        panelRect.anchorMax = new Vector2(0.8f, 0.8f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = panelObject.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.72f);
        gameOverPanel = panelObject;
        gameOverPanel.SetActive(false);

        resultText = CreateText(panelObject.transform, "ResultText", new Vector2(0.15f, 0.45f), new Vector2(0.85f, 0.75f), "TIME'S UP!\nSCORE: 0\nBEST SCORE: 0", 30, Color.white, TextAnchor.MiddleCenter);
        bestScoreText = CreateText(panelObject.transform, "BestScoreText", new Vector2(0.2f, 0.25f), new Vector2(0.8f, 0.4f), "BEST SCORE: 0", 28, Color.yellow, TextAnchor.MiddleCenter);

        retryButton = CreateButton(panelObject.transform, "RetryButton", new Vector2(0.2f, 0.08f), new Vector2(0.45f, 0.2f), "RETRY", new Color(0.2f, 0.7f, 0.3f), 26);
        homeButton = CreateButton(panelObject.transform, "HomeButton", new Vector2(0.55f, 0.08f), new Vector2(0.8f, 0.2f), "HOME", new Color(0.75f, 0.25f, 0.25f), 26);

        retryButton.onClick.AddListener(RetryGame);
        homeButton.onClick.AddListener(ReturnToLogin);
    }

    private Text CreateText(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, string content, int fontSize, Color color, TextAnchor alignment)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);

        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = anchorMin;
        textRect.anchorMax = anchorMax;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text text = textObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.text = content;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;
        text.resizeTextForBestFit = false;
        return text;
    }

    private Button CreateButton(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, string label, Color color, int fontSize)
    {
        GameObject buttonObject = new GameObject(name);
        buttonObject.transform.SetParent(parent, false);

        RectTransform buttonRect = buttonObject.AddComponent<RectTransform>();
        buttonRect.anchorMin = anchorMin;
        buttonRect.anchorMax = anchorMax;
        buttonRect.offsetMin = Vector2.zero;
        buttonRect.offsetMax = Vector2.zero;

        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = color;

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonImage;

        GameObject textObject = new GameObject("ButtonText");
        textObject.transform.SetParent(buttonObject.transform, false);

        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text buttonText = textObject.AddComponent<Text>();
        buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        buttonText.text = label;
        buttonText.fontSize = fontSize;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;

        return button;
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
            timerText.text = "TIME: " + Mathf.CeilToInt(Mathf.Max(0f, remainingTime));

        if (bestScoreText != null)
            bestScoreText.text = "BEST SCORE: " + GameData.bestScore;
    }

    private void EndGame()
    {
        if (gameEnded)
            return;

        gameEnded = true;
        GameData.bestScore = Mathf.Max(GameData.bestScore, GameData.score);
        GameData.timeRemaining = 0f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (resultText != null)
            resultText.text = "TIME'S UP!\nSCORE: " + GameData.score + "\nBEST SCORE: " + GameData.bestScore;

        if (bestScoreText != null)
            bestScoreText.text = "BEST SCORE: " + GameData.bestScore;

        Time.timeScale = 0f;
    }

    private void RetryGame()
    {
        Time.timeScale = 1f;
        GameData.score = 0;
        GameData.timeRemaining = MatchDurationSeconds;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ReturnToLogin()
    {
        Time.timeScale = 1f;
        GameData.ResetForNewPlayer();
        SceneManager.LoadScene(loginSceneName);
    }
}
