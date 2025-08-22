using UnityEngine;

public class autobullet : MonoBehaviour
{
    public float lifetime = 3f;   // How long before bullet auto-destroys
    public int damage = 1;        // Damage to player (optional)

    void Start()
    {
        // Destroy bullet after lifetime seconds
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Example: detect if it hits the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // If your player has a health script, you can reduce HP
             collision.gameObject.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        }

        // Destroy the bullet on any collision
        Destroy(gameObject);
    }
}