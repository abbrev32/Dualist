using UnityEngine;
using Mirror;
using System.Linq; // Needed for FirstOrDefault

public class Ghost : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float speed = 3f;

    private Transform target; // Light player to follow

    public override void OnStartServer()
    {
        // Find the Light player once at server start
        GameObject lightPlayer = GameObject.FindGameObjectsWithTag("Player")
            .FirstOrDefault(p => p.GetComponent<PlayerFaction>()?.faction == PlayerFaction.Faction.Light);

        if (lightPlayer != null)
            target = lightPlayer.transform;
    }

    [ServerCallback]
    void Update()
    {
        if (!isServer || target == null) return;

        // Move towards the Light player
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
            if (pf != null && pf.faction == PlayerFaction.Faction.Light)
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
