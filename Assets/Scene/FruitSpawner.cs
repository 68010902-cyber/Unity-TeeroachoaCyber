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
    public float minSpawnInterval = 0.8f;
    public float maxSpawnInterval = 1.8f;

    [Header("Difficulty")]
    public FruitSpeedLevel speedLevel = FruitSpeedLevel.Normal;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        InvokeRepeating(nameof(SpawnNextFruit), 0f, 0.7f);
    }

    private void SpawnNextFruit()
    {
        if (fruitPrefabs == null || fruitPrefabs.Length == 0)
            return;

        float x = Random.Range(0.15f, 0.85f);
        float y = 1.15f;

        Vector3 spawnPosition = mainCamera.ViewportToWorldPoint(new Vector3(x, y, 10f));
        spawnPosition.z = 0f;

        GameObject fruit = Instantiate(
            fruitPrefabs[Random.Range(0, fruitPrefabs.Length)],
            spawnPosition,
            Quaternion.identity
        );

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

        // The repeating call already handles timing, so we just let the next drop happen naturally.
    }
}
