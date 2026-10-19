using UnityEngine;

public sealed class ExplosionEffect : MonoBehaviour
{
    private const float Duration = 0.55f;
    private static Sprite flashSprite;
    private static Sprite sparkSprite;

    private readonly SpriteRenderer[] sparks = new SpriteRenderer[12];
    private readonly Vector2[] sparkDirections = new Vector2[12];
    private Camera gameplayCamera;
    private Vector3 cameraStartPosition;
    private float elapsed;
    private SpriteRenderer flash;

    public static void Play(Vector3 position)
    {
        GameObject effectObject = new GameObject("Explosion Effect");
        effectObject.transform.position = position;
        effectObject.AddComponent<ExplosionEffect>();
    }

    private void Awake()
    {
        flash = CreateRenderer("Blast Flash", GetFlashSprite(), 100);
        flash.transform.localScale = Vector3.one * 120f;

        for (int i = 0; i < sparks.Length; i++)
        {
            float angle = Mathf.PI * 2f * i / sparks.Length + Random.Range(-0.15f, 0.15f);
            sparkDirections[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            SpriteRenderer spark = CreateRenderer("Blast Spark", GetSparkSprite(), 101);
            spark.transform.localScale = Vector3.one * Random.Range(9f, 16f);
            sparks[i] = spark;
        }

        gameplayCamera = Camera.main;
        if (gameplayCamera == null)
            gameplayCamera = FindFirstObjectByType<Camera>();
        if (gameplayCamera != null)
            cameraStartPosition = gameplayCamera.transform.position;
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        float progress = Mathf.Clamp01(elapsed / Duration);
        float fade = 1f - progress;

        flash.transform.localScale = Vector3.one * Mathf.Lerp(40f, 165f, progress);
        flash.color = new Color(1f, 0.55f + 0.4f * progress, 0.12f, fade);

        for (int i = 0; i < sparks.Length; i++)
        {
            float travel = Mathf.Lerp(20f, 105f, progress);
            sparks[i].transform.localPosition = sparkDirections[i] * travel;
            sparks[i].color = new Color(1f, Mathf.Lerp(0.9f, 0.25f, progress), 0.08f, fade);
        }

        if (gameplayCamera != null)
        {
            float shake = 7f * fade;
            Vector3 offset = new Vector3(Random.Range(-shake, shake), Random.Range(-shake, shake), 0f);
            gameplayCamera.transform.position = cameraStartPosition + offset;
        }

        if (elapsed >= Duration)
        {
            if (gameplayCamera != null)
                gameplayCamera.transform.position = cameraStartPosition;
            Destroy(gameObject);
        }
    }

    private SpriteRenderer CreateRenderer(string objectName, Sprite sprite, int sortingOrder)
    {
        GameObject visual = new GameObject(objectName);
        visual.transform.SetParent(transform, false);
        SpriteRenderer renderer = visual.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = sortingOrder;
        return renderer;
    }

    private static Sprite GetFlashSprite()
    {
        if (flashSprite != null)
            return flashSprite;

        const int size = 64;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        Color[] pixels = new Color[size * size];
        Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center) / (size * 0.5f);
                float alpha = Mathf.Clamp01((1f - distance) * 4f);
                pixels[y * size + x] = new Color(1f, 0.72f, 0.12f, alpha);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        flashSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
        return flashSprite;
    }

    private static Sprite GetSparkSprite()
    {
        if (sparkSprite != null)
            return sparkSprite;

        const int size = 8;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.white;

        texture.SetPixels(pixels);
        texture.Apply();
        sparkSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
        return sparkSprite;
    }
}
