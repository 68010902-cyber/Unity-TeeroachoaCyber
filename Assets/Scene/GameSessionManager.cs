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
    private float nextDamageTime;

    private Canvas hudCanvas;
    [SerializeField] private Text timerText;
    [SerializeField] private Sprite heartIcon;
    private Image[] lifeImages;
    private static Sprite proceduralHeartSprite;

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
        EnsureGameplayCamera();
        ApplySelectedBackground();
        remainingTime = GameData.timeRemaining > 0f
            ? Mathf.Min(GameData.timeRemaining, MatchDurationSeconds)
            : MatchDurationSeconds;
        GameData.timeRemaining = remainingTime;
        GameData.lives = GameData.StartingLives;
        gameEnded = false;
        nextDamageTime = 0f;

        Time.timeScale = 1f;

        EnsureHud();
        EnsureLivesHud();
        EnsureControlsHint();
        EnsureObstacleSpawner();
        UpdateTimerDisplay();
    }

    public static void RegisterBombHit()
    {
        GameSessionManager manager = FindFirstObjectByType<GameSessionManager>();
        if (manager != null)
            manager.TakeBombHit();
    }

    private void TakeBombHit()
    {
        if (gameEnded || Time.time < nextDamageTime)
            return;

        nextDamageTime = Time.time + 0.75f;
        GameData.lives = Mathf.Max(0, GameData.lives - 1);
        UpdateLivesHud();
        Debug.Log("Player hit by a bomb. Hearts remaining: " + GameData.lives);

        if (GameData.lives == 0)
            StartCoroutine(EndGameAfterExplosion());
    }

    private System.Collections.IEnumerator EndGameAfterExplosion()
    {
        gameEnded = true;
        ObstacleSpawner spawner = GetComponent<ObstacleSpawner>();
        if (spawner != null)
            spawner.enabled = false;

        yield return new WaitForSecondsRealtime(0.6f);
        CompleteGame();
    }

    private void EnsureObstacleSpawner()
    {
        if (GetComponent<ObstacleSpawner>() == null)
            gameObject.AddComponent<ObstacleSpawner>();
    }

    private void EnsureLivesHud()
    {
        GameObject canvasObject = new GameObject("LivesHudCanvas");
        Canvas livesCanvas = canvasObject.AddComponent<Canvas>();
        livesCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        livesCanvas.sortingOrder = 20;
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasObject.AddComponent<GraphicRaycaster>();

        lifeImages = new Image[GameData.StartingLives];
        for (int i = 0; i < lifeImages.Length; i++)
        {
            GameObject heartObject = new GameObject("Heart " + (i + 1), typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            heartObject.transform.SetParent(canvasObject.transform, false);

            RectTransform heartRect = heartObject.GetComponent<RectTransform>();
            heartRect.anchorMin = new Vector2(0.5f, 1f);
            heartRect.anchorMax = new Vector2(0.5f, 1f);
            heartRect.anchoredPosition = new Vector2((i - 1) * 78f, -58f);
            heartRect.sizeDelta = new Vector2(72f, 72f);

            Image heartImage = heartObject.GetComponent<Image>();
            heartImage.sprite = GetHeartSprite();
            heartImage.raycastTarget = false;
            lifeImages[i] = heartImage;
        }

        UpdateLivesHud();
    }

    private void UpdateLivesHud()
    {
        if (lifeImages == null)
            return;

        for (int i = 0; i < lifeImages.Length; i++)
            lifeImages[i].enabled = i < GameData.lives;
    }

    private void EnsureControlsHint()
    {
        GameObject canvasObject = new GameObject("ControlsHintCanvas");
        Canvas controlsCanvas = canvasObject.AddComponent<Canvas>();
        controlsCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        controlsCanvas.sortingOrder = 25;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        GameObject backgroundObject = new GameObject(
            "ControlsHintBackground",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image)
        );
        backgroundObject.transform.SetParent(canvasObject.transform, false);

        RectTransform backgroundRect = backgroundObject.GetComponent<RectTransform>();
        backgroundRect.anchorMin = new Vector2(0.5f, 0f);
        backgroundRect.anchorMax = new Vector2(0.5f, 0f);
        backgroundRect.anchoredPosition = new Vector2(0f, 76f);
        backgroundRect.sizeDelta = new Vector2(1040f, 92f);

        Image backgroundImage = backgroundObject.GetComponent<Image>();
        backgroundImage.color = new Color(0f, 0f, 0f, 0.68f);
        backgroundImage.raycastTarget = false;

        GameObject hintObject = new GameObject(
            "ControlsHintText",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Text)
        );
        hintObject.transform.SetParent(backgroundObject.transform, false);

        RectTransform hintRect = hintObject.GetComponent<RectTransform>();
        hintRect.anchorMin = Vector2.zero;
        hintRect.anchorMax = Vector2.one;
        hintRect.offsetMin = new Vector2(12f, 4f);
        hintRect.offsetMax = new Vector2(-12f, -4f);

        Text hintText = hintObject.GetComponent<Text>();
        hintText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        hintText.fontSize = 23;
        hintText.alignment = TextAnchor.MiddleCenter;
        hintText.color = Color.white;
        hintText.raycastTarget = false;
        hintText.text =
            "เดินซ้าย: A / ลูกศรซ้าย    เดินขวา: D / ลูกศรขวา\n" +
            "กระโดด: SPACE / W / ลูกศรขึ้น    พุ่ง: SHIFT + ปุ่มทิศทาง";
    }

    private Sprite GetHeartSprite()
    {
        if (heartIcon != null)
            return heartIcon;

        if (proceduralHeartSprite != null)
            return proceduralHeartSprite;

        const int size = 32;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        Color red = new Color(0.95f, 0.08f, 0.16f, 1f);
        texture.SetPixels(new Color[size * size]);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                bool leftLobe = (x - 9) * (x - 9) + (y - 21) * (y - 21) <= 81;
                bool rightLobe = (x - 23) * (x - 23) + (y - 21) * (y - 21) <= 81;
                bool lowerPoint = y <= 19 && x >= 4 + (19 - y) / 2 && x <= 28 - (19 - y) / 2;
                if (leftLobe || rightLobe || lowerPoint)
                    texture.SetPixel(x, y, red);
            }
        }

        texture.Apply();
        proceduralHeartSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
        return proceduralHeartSprite;
    }

    private void EnsureGameplayCamera()
    {
        Camera[] cameras = FindObjectsByType<Camera>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        Camera gameplayCamera = null;
        foreach (Camera candidate in cameras)
        {
            if (candidate.CompareTag("MainCamera"))
            {
                gameplayCamera = candidate;
                break;
            }

            if (gameplayCamera == null)
                gameplayCamera = candidate;
        }

        if (gameplayCamera != null)
        {
            if (!gameplayCamera.gameObject.activeSelf)
                gameplayCamera.gameObject.SetActive(true);

            gameplayCamera.enabled = true;
            return;
        }

        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(transform.position.x, transform.position.y, -10f);

        Camera fallbackCamera = cameraObject.AddComponent<Camera>();
        fallbackCamera.orthographic = true;
        fallbackCamera.orthographicSize = 200f;

        Debug.LogWarning("No scene camera was available. Created a fallback 2D camera.", cameraObject);
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
        CompleteGame();
    }

    private void CompleteGame()
    {
        ObstacleSpawner spawner = GetComponent<ObstacleSpawner>();
        if (spawner != null)
            spawner.enabled = false;
        GameData.PrepareCompletedRun();
        GameData.timeRemaining = 0f;
        Time.timeScale = 1f;
        SceneManager.LoadScene("RetryScene");
    }
}
