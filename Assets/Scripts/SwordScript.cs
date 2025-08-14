using UnityEngine;
public class SwordScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Mirror.NetworkServer.active) return;
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Monster Hit");
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }
        }
        if (collision.CompareTag("Monster"))
        {
            Monster monster = collision.GetComponent<Monster>();
            if (monster != null)
            {
                monster.Kill();
            }
        }
    }
}
