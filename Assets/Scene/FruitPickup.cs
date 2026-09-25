using UnityEngine;

public class FruitPickup : MonoBehaviour
{
    public int scoreValue = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryCollect(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryCollect(collision.collider);
    }

    private void TryCollect(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        GameData.score += scoreValue;
        Debug.Log("Picked fruit: +" + scoreValue + " score");
        Destroy(gameObject);
    }
}
