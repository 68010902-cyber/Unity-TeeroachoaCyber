using UnityEngine;

public sealed class BombObstacle : MonoBehaviour
{
    private bool hitPlayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryHitPlayer(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryHitPlayer(collision.gameObject);
    }

    private void TryHitPlayer(GameObject other)
    {
        if (hitPlayer || !other.CompareTag("Player"))
            return;

        hitPlayer = true;
        ExplosionEffect.Play(transform.position);
        GameSessionManager.RegisterBombHit();
        Destroy(gameObject);
    }
}
