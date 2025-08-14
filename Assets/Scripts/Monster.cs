using UnityEngine;
public class Monster : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Mirror.NetworkServer.active) return;
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }
        }
    }

    public void Kill()
    {
        // Called when ninja kills the monster
        Destroy(gameObject);
    }
}