using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    [Header("Prefab วัตถุที่ต้องการสุ่มตก")]
    public GameObject[] objectsToSpawn;

    [Header("ตั้งค่าการเวลา")]
    public float spawnInterval = 1.0f;
    private float nextSpawnTime;

    [Header("พื้นที่ในการสุ่ม (แกน X และ Z)")]
    public float minX = -5f;
    public float maxX = 5f;
    public float minZ = -5f;
    public float maxZ = 5f;

    [Header("ความสูงที่ต้องการให้วัตถุเริ่มตก (แกน Y)")]
    public float spawnHeight = 10f;

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnRandomObject();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void SpawnRandomObject()
    {
        if (objectsToSpawn == null || objectsToSpawn.Length == 0)
        {
            Debug.LogWarning("กรุณาใส่ Prefab ใน Array objectsToSpawn ก่อนครับ!");
            return;
        }

        float randomX = Random.Range(minX, maxX);
        float randomZ = Random.Range(minZ, maxZ);
        Vector3 spawnPosition = new Vector3(randomX, spawnHeight, randomZ);

        int randomIndex = Random.Range(0, objectsToSpawn.Length);
        GameObject selectedPrefab = objectsToSpawn[randomIndex];

        Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
    }
}