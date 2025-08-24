using UnityEngine;
public class SwordScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Mirror.NetworkServer.active) return;
        //if (collision.CompareTag("Player"))
        //{
        //    PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
        //    if (playerHealth != null)
        //    {
        //        playerHealth.TakeDamage(1);
        //    }
        //}
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
}