using UnityEngine;
using Mirror;

public class Ghost : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float speed = 3f;

    private Transform target; // Nearest player

    [ServerCallback]
    void Update()
    {
        if (!isServer) return;

        // Always look for nearest player
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float nearestDistance = Mathf.Infinity;
        Transform nearest = null;

        foreach (GameObject p in players)
        {
            float dist = Vector2.Distance(transform.position, p.transform.position);
            if (dist < nearestDistance)
            {
                nearestDistance = dist;
                nearest = p.transform;
            }
        }

        target = nearest;

        if (target == null) return;

        // Move towards nearest player
        Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Player touch → deal damage
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.CmdTakeDamage(2); // deal 2 damage
            }

            NetworkServer.Destroy(gameObject); // Ghost disappears
        }

        // Sword/Knife touch → ghost dies
        if (collision.CompareTag("Sword") || collision.CompareTag("Knife"))
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}
