using System.Collections;
using UnityEngine;

public sealed class ObstacleSpawner : MonoBehaviour
{
    private static Sprite bombSprite;
    private Camera gameplayCamera;

    private void Start()
    {
        gameplayCamera = Camera.main;
        if (gameplayCamera == null)
            gameplayCamera = FindFirstObjectByType<Camera>();

        StartCoroutine(SpawnBombs());
    }

    private IEnumerator SpawnBombs()
    {
        while (enabled)
        {
            float interval = GameData.selectedLevel switch
            {
                FruitSpeedLevel.Easy => 5f,
                FruitSpeedLevel.Normal => 3.5f,
                FruitSpeedLevel.Hard => 2.5f,
                _ => 3.5f
            };
            yield return new WaitForSeconds(Random.Range(interval * 0.8f, interval * 1.2f));

            if (!enabled)
                yield break;

            if (gameplayCamera == null)
            {
                gameplayCamera = Camera.main;
                if (gameplayCamera == null)
                    gameplayCamera = FindFirstObjectByType<Camera>();
            }

            if (gameplayCamera == null)
            {
                Debug.LogError("Cannot spawn bomb obstacle because the game camera is missing.", this);
                continue;
            }

            float x = Random.Range(0.15f, 0.85f);
            Vector3 position = gameplayCamera.ViewportToWorldPoint(new Vector3(x, 1.1f, 10f));
            position.z = 0f;

            GameObject bomb = new GameObject("Bomb Obstacle");
            bomb.transform.position = position;
            bomb.transform.localScale = Vector3.one * 45f;

            SpriteRenderer renderer = bomb.AddComponent<SpriteRenderer>();
            renderer.sprite = GetBombSprite();
            renderer.sortingOrder = 5;

            Rigidbody2D body = bomb.AddComponent<Rigidbody2D>();
            body.gravityScale = GameData.selectedLevel switch
            {
                FruitSpeedLevel.Easy => 1f,
                FruitSpeedLevel.Normal => 1.6f,
                FruitSpeedLevel.Hard => 2.2f,
                _ => 1.6f
            };
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            CircleCollider2D collider = bomb.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.48f;

            bomb.AddComponent<BombObstacle>();
            StartCoroutine(DestroyOffscreenBomb(bomb));
        }
    }

    private IEnumerator DestroyOffscreenBomb(GameObject bomb)
    {
        while (bomb != null)
        {
            if (gameplayCamera != null &&
                gameplayCamera.WorldToViewportPoint(bomb.transform.position).y < -0.15f)
            {
                Destroy(bomb);
                yield break;
            }

            yield return null;
        }
    }

    private static Sprite GetBombSprite()
    {
        if (bombSprite != null)
            return bombSprite;

        const int size = 32;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        Color outline = new Color(0.12f, 0.1f, 0.14f, 1f);
        Color body = new Color(0.28f, 0.25f, 0.3f, 1f);
        Color fuse = new Color(1f, 0.6f, 0.08f, 1f);
        texture.SetPixels(new Color[size * size]);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int dx = x - 15;
                int dy = y - 13;
                int distanceSquared = dx * dx + dy * dy;
                if (distanceSquared <= 121)
                    texture.SetPixel(x, y, distanceSquared >= 81 ? outline : body);
            }
        }

        texture.SetPixel(20, 23, outline);
        texture.SetPixel(21, 24, outline);
        texture.SetPixel(22, 25, fuse);
        texture.SetPixel(23, 26, fuse);
        texture.SetPixel(24, 27, fuse);
        texture.SetPixel(26, 29, fuse);
        texture.SetPixel(24, 29, fuse);
        texture.Apply();
        bombSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
        return bombSprite;
    }
}
