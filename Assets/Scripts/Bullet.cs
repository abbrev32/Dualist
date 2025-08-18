using Mirror;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!NetworkServer.active) return;

        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Player Hit!");
            PlayerHealth playerHealth = collision.collider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
                Kill();
            }
        }

        if (collision.collider.CompareTag("Border"))
        {
            Debug.Log("Wall Hit!");
            Kill();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!NetworkServer.active) return;

        if (collision.CompareTag("Sword"))
        {
            Debug.Log("Sword Blocked!");
            Kill();
        }
    }

    [Server]
    void Kill()
    {
        NetworkServer.Destroy(gameObject); // destroys across all clients
    }
}
