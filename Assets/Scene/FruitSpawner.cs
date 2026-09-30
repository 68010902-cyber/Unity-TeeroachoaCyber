using UnityEngine;

public enum FruitSpeedLevel
{
    Easy,
    Normal,
    Hard
}

public class FruitSpawner : MonoBehaviour
{
    [Header("Fruit Prefabs")]
    public GameObject[] fruitPrefabs;

    [Header("Spawn Timing")]
    public float minSpawnInterval = 0.5f;
    public float maxSpawnInterval = 1.5f;

    [Header("Difficulty")]
    public FruitSpeedLevel speedLevel = FruitSpeedLevel.Normal;

    private Camera mainCamera;

    private void OnEnable()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        speedLevel = GameData.selectedLevel;
        CancelInvoke(nameof(SpawnNextFruit));
        InvokeRepeating(nameof(SpawnNextFruit), 0.2f, Mathf.Clamp((maxSpawnInterval + minSpawnInterval) * 0.5f, 0.4f, 1.2f));
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(SpawnNextFruit));
    }

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        speedLevel = GameData.selectedLevel;
    }

    private void SpawnNextFruit()
    {
        if (this == null || fruitPrefabs == null || fruitPrefabs.Length == 0)
            return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return;

        float x = Random.Range(0.15f, 0.85f);
        float y = 1.15f;

        Vector3 spawnPosition = mainCamera.ViewportToWorldPoint(new Vector3(x, y, 10f));
        spawnPosition.z = 0f;

        GameObject fruitPrefab = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];
        if (fruitPrefab == null)
            return;

        GameObject fruit = Instantiate(fruitPrefab, spawnPosition, Quaternion.identity);
        if (fruit == null)
            return;

        Rigidbody2D rb = fruit.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float gravity = speedLevel switch
            {
                FruitSpeedLevel.Easy => 1f,
                FruitSpeedLevel.Normal => 1.8f,
                FruitSpeedLevel.Hard => 2.6f,
                _ => 1.8f
            };

            rb.gravityScale = gravity;
        }

        Destroy(fruit, 8f);
    }
}
