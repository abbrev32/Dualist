using Mirror;
using UnityEngine;

public class EnemyBullet : NetworkBehaviour
{
    private Rigidbody2D rb;
    public float force;

    // This method is a Server-side function that gets called by the turret
    [Server]
    public void SetTarget(GameObject targetPlayer)
    {
        rb = GetComponent<Rigidbody2D>();

        // Calculate the direction from the bullet's current position to the target's position
        Vector3 direction = targetPlayer.transform.position - transform.position;

        // Apply the velocity on the server
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * force;

        // Also rotate the bullet to face the target on the server
        float rot = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot);
    }

    // This method handles collisions, but only on the server to prevent cheating
    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Only the server can apply damage
            if (collision.TryGetComponent<PlayerHealth>(out PlayerHealth health))
            {
                health.TakeDamage(1);
            }
            NetworkServer.Destroy(gameObject); // Destroy the bullet on all clients
        }
        else if (collision.CompareTag("Border"))
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}