using UnityEngine;

public class FruitPickup : MonoBehaviour
{
    public int scoreValue = 10;
    private bool collected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryCollect(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryCollect(collision.gameObject);
    }

    private void TryCollect(GameObject other)
    {
        if (collected || other == null)
            return;

        if (!other.CompareTag("Player"))
            return;

        collected = true;
        GameData.score += scoreValue;
        GameData.UpdateBestScore();
        Debug.Log("Picked fruit: +" + scoreValue + " score");

        Destroy(gameObject);
    }
}
