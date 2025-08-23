using UnityEngine;
using Mirror;
using System.Linq;

public class GhostDark : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float speed = 3f;

    private Transform target; // Dark player to follow

    [ServerCallback]
    void Update()
    {
        // If target not found yet, search for it
        if (target == null)
        {
            GameObject darkPlayer = GameObject.FindGameObjectsWithTag("Player")
                .FirstOrDefault(p => p.GetComponent<PlayerFaction>()?.faction == PlayerFaction.Faction.Dark);

            if (darkPlayer != null)
                target = darkPlayer.transform;
        }

        if (!isServer || target == null) return;

        // Move towards the Dark player
        Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Player touch
        if (collision.CompareTag("Player"))
        {
            PlayerFaction pf = collision.GetComponent<PlayerFaction>();
            if (pf != null && pf.faction == PlayerFaction.Faction.Dark)
            {
                PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.CmdTakeDamage(2); // deal 2 damage
                }

                NetworkServer.Destroy(gameObject); // Ghost disappears
            }
        }

        // Sword touch
        if (collision.CompareTag("Sword"))
        {
            NetworkServer.Destroy(gameObject); // Ghost killed instantly
        }
    }
}
