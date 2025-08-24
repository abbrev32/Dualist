using Mirror;
using UnityEngine;
public class SwordScript : NetworkBehaviour
{
    public static bool pvp = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Mirror.NetworkServer.active) return;
        if (!isLocalPlayer && pvp)
        {
            if (collision.CompareTag("Player"))
            {
                PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(1);
                }
            }
        }
        if (collision.CompareTag("Monster"))
        {
            Debug.Log("Monster Hit");
            Monster monster = collision.GetComponent<Monster>();
            if (monster != null)
            {
                monster.Kill();
            }
        }
        if (collision.CompareTag("Turret"))
        {
            TurretHealth turretHealth = collision.GetComponent<TurretHealth>();
            if (turretHealth != null)
            {
                turretHealth.TakeDamage(1);
            }
        }
        if (collision.CompareTag("MonsterSpawner"))
        {
            MonsterSpawner spawnerHealth = collision.GetComponent<MonsterSpawner>();
            if (spawnerHealth != null)
            {
                spawnerHealth.TakeDamage(1);
            }
        }

    }
    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.collider.CompareTag("Monster"))
    //     {
    //         Debug.Log("Monster Hit");
    //         Monster monster = collision.collider.GetComponent<Monster>();
    //         if (monster != null)
    //         {
    //             monster.Kill();
    //         }
    //     }
    // }
}