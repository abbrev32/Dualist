using Mirror;
using UnityEngine;
public class Monster : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!NetworkServer.active) return;
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.CmdTakeDamage(1);
            }
        }
    }
    [Server]
    public void Kill()
    {
        // Called when ninja kills the monster
        NetworkServer.Destroy(gameObject);
    }
}