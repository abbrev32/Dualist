using Mirror;
using UnityEngine;

public class CollectableItem : NetworkBehaviour
{
    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerHealth health = collision.GetComponent<PlayerHealth>();
        if (health != null)
            health.HealthReset();

        NetworkServer.Destroy(gameObject);
    }
}

